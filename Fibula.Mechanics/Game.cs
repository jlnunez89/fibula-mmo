// -----------------------------------------------------------------
// <copyright file="Game.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Operations;
    using Fibula.Mechanics.Operations.Arguments;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;
    using Fibula.Notifications.Contracts.Abstractions;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Delegates;
    using Serilog;

    /// <summary>
    /// Class that represents the game instance.
    /// </summary>
    public class Game : IGame
    {
        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks for idle players and connections.
        /// </summary>
        private static readonly TimeSpan IdleCheckDelay = TimeSpan.FromSeconds(30);

        /// <summary>
        /// A reference to the logger in use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Stores the scheduler used by the game.
        /// </summary>
        private readonly IScheduler scheduler;

        private readonly IMapDescriptor mapDescriptor;

        /// <summary>
        /// The current map instance.
        /// </summary>
        private readonly IMap map;

        /// <summary>
        /// The creature manager instance.
        /// </summary>
        private readonly ICreatureManager creatureManager;

        /// <summary>
        /// Gets the item factory instance.
        /// </summary>
        private readonly IItemFactory itemFactory;

        /// <summary>
        /// Gets the creature factory instance.
        /// </summary>
        private readonly ICreatureFactory creatureFactory;

        /// <summary>
        /// The operation factory instance.
        /// </summary>
        private readonly IOperationFactory operationFactory;

        /// <summary>
        /// The container manager instance.
        /// </summary>
        private readonly IContainerManager containerManager;

        /// <summary>
        /// Stores the world information.
        /// </summary>
        private readonly WorldInformation worldInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapLoader">A reference to the map loader in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="map">A reference to the map.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A referecne to the operation factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="scheduler">A reference to the global scheduler instance.</param>
        public Game(
            ILogger logger,
            IMapLoader mapLoader,
            IMapDescriptor mapDescriptor,
            IMap map,
            ICreatureManager creatureManager,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IOperationFactory operationFactory,
            IContainerManager containerManager,
            IScheduler scheduler)
        {
            logger.ThrowIfNull(nameof(logger));
            mapLoader.ThrowIfNull(nameof(mapLoader));
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            map.ThrowIfNull(nameof(map));
            creatureManager.ThrowIfNull(nameof(creatureManager));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            operationFactory.ThrowIfNull(nameof(operationFactory));
            containerManager.ThrowIfNull(nameof(containerManager));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.logger = logger.ForContext<Game>();
            this.mapDescriptor = mapDescriptor;
            this.map = map;
            this.creatureManager = creatureManager;
            this.itemFactory = itemFactory;
            this.creatureFactory = creatureFactory;
            this.operationFactory = operationFactory;
            this.containerManager = containerManager;
            this.scheduler = scheduler;

            // Load some catalogs.

            // Initialize game vars.
            this.worldInfo = new WorldInformation()
            {
                Status = WorldState.Loading,
                LightColor = (byte)LightColors.White,
                LightLevel = (byte)LightLevels.World,
            };

            // Hook some event handlers.
            this.scheduler.EventFired += this.ProcessFiredEvent;

            // mapLoader.WindowLoaded += this.OnMapWindowLoaded;
        }

        /// <summary>
        /// Gets the game world's information.
        /// </summary>
        public IWorldInformation WorldInfo => this.worldInfo;

        /// <summary>
        /// Runs the main game processing thread which begins advancing time on the game engine.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var connectionSweepperTask = Task.Factory.StartNew(this.IdlePlayerSweep, cancellationToken, TaskCreationOptions.LongRunning);
                var miscellaneusEventsTask = Task.Factory.StartNew(this.MiscellaneousEventsLoop, cancellationToken, TaskCreationOptions.LongRunning);

                // start the scheduler.
                var schedulerTask = this.scheduler.RunAsync(cancellationToken);

                // Open the game world!
                this.worldInfo.Status = WorldState.Open;

                await Task.WhenAll(connectionSweepperTask, miscellaneusEventsTask, schedulerTask);
            });

            // return this to allow other IHostedService-s to start.
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.Warning($"Cancellation requested on game instance, beginning shut-down...");

            // TODO: probably save game state here.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Re-sets the combat target of the attacker and it's (possibly new) target.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="target">The new target.</param>
        public void ResetCombatTarget(ICombatant attacker, ICombatant target)
        {
            if (attacker == null)
            {
                return;
            }

            if (attacker.SetAttackTarget(target))
            {
                this.scheduler.CancelAllFor(attacker.Id, typeof(AutoAttackOrchestratorOperation));

                if (attacker.AutoAttackTarget != null)
                {
                    this.DispatchOperation(new AutoAttackOrchestratorOperationCreationArguments(attacker));
                }
            }
        }

        /// <summary>
        /// Re-sets a given creature's walk plan and kicks it off.
        /// </summary>
        /// <param name="creature">The creature to reset the walk plan of.</param>
        /// <param name="directions">The directions for the new plan.</param>
        public void ResetCreatureWalkPlan(ICreature creature, Direction[] directions)
        {
            if (creature == null || !directions.Any())
            {
                return;
            }

            var lastLoc = creature.Location;
            var waypoints = new List<Location>()
            {
                creature.Location,
            };

            // Build the waypoints.
            for (int i = 0; i < directions.Length; i++)
            {
                var nextLoc = lastLoc.LocationAt(directions[i]);

                waypoints.Add(nextLoc);
                lastLoc = nextLoc;
            }

            creature.WalkPlan = new WalkPlan(WalkStrategy.GiveUpOnInterruption, () => lastLoc, waypoints.ToArray());

            this.DispatchOperation(new AutoWalkOrchestratorOperationCreationArguments(creature));
        }

        /// <summary>
        /// Cancels all actions that a player has pending.
        /// </summary>
        /// <param name="player">The player to cancel actions for.</param>
        /// <param name="typeOfActionToCancel">Optional. The specific type of action to cancel.</param>
        public void CancelPlayerActions(IPlayer player, Type typeOfActionToCancel = null)
        {
            if (player == null || (typeOfActionToCancel != null && !typeof(IOperation).IsAssignableFrom(typeOfActionToCancel)))
            {
                return;
            }

            this.DispatchOperation(new CancelActionsOperationCreationArguments(player, typeOfActionToCancel));
        }

        /// <summary>
        /// Changes the fight, chase and safety modes of a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="fightMode">The fight mode to change to.</param>
        /// <param name="chaseMode">The chase mode to change to.</param>
        /// <param name="safeModeOn">A value indicating whether the attack safety lock is on.</param>
        public void CreatureChangeModes(uint creatureId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn)
        {
            this.DispatchOperation(new ChangeModesOperationCreationArguments(creatureId, fightMode, chaseMode, safeModeOn));
        }

        /// <summary>
        /// Handles creature speech.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="channelType">The type of channel of the speech.</param>
        /// <param name="content">The content of the speech.</param>
        /// <param name="receiver">Optional. The receiver of the speech, if any.</param>
        public void CreatureSpeech(uint creatureId, SpeechType speechType, ChatChannelType channelType, string content, string receiver = "")
        {
            this.DispatchOperation(new SpeechOperationCreationArguments(creatureId, speechType, channelType, content, receiver));
        }

        /// <summary>
        /// Turns a creature to a direction.
        /// </summary>
        /// <param name="requestorId">The id of the creature.</param>
        /// <param name="creature">The creature to turn.</param>
        /// <param name="direction">The direction to turn to.</param>
        public void CreatureTurn(uint requestorId, ICreature creature, Direction direction)
        {
            this.DispatchOperation(new TurnToDirectionOperationCreationArguments(requestorId, creature, direction));
        }

        /// <summary>
        /// Describes a thing for a player.
        /// </summary>
        /// <param name="thingId">The id of the thing to describe.</param>
        /// <param name="location">The location of the thing to describe.</param>
        /// <param name="stackPosition">The position in the stack within the location of the thing to describe.</param>
        /// <param name="player">The player for which to describe the thing for.</param>
        public void DescribeFor(ushort thingId, Location location, byte stackPosition, IPlayer player)
        {
            this.DispatchOperation(new DescribeThingOperationCreationArguments(thingId, location, stackPosition, player));
        }

        /// <summary>
        /// Logs a player into the game.
        /// </summary>
        /// <param name="client">The client from which the player is connecting.</param>
        /// <param name="creatureCreationMetadata">The metadata for the player's creation.</param>
        public void LogPlayerIn(IClient client, ICreatureCreationMetadata creatureCreationMetadata)
        {
            client.ThrowIfNull(nameof(client));
            creatureCreationMetadata.ThrowIfNull(nameof(creatureCreationMetadata));

            this.DispatchOperation(
                new LogInOperationCreationArguments(
                    client,
                    creatureCreationMetadata,
                    this.WorldInfo.LightLevel,
                    this.WorldInfo.LightColor));
        }

        /// <summary>
        /// Logs a player out of the game.
        /// </summary>
        /// <param name="player">The player to log out.</param>
        public void LogPlayerOut(IPlayer player)
        {
            if (player == null)
            {
                return;
            }

            this.DispatchOperation(new LogOutOperationCreationArguments(player));
        }

        /// <summary>
        /// Moves a thing.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the move.</param>
        /// <param name="clientThingId">The id of the thing being moved.</param>
        /// <param name="fromLocation">The location from which the thing is being moved.</param>
        /// <param name="fromIndex">The index within the location from which the thing is being moved.</param>
        /// <param name="fromCreatureId">The id of the creature from which the thing is being moved, if any.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="toCreatureId">The id of the creature to which the thing is being moved.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Defaults to 1.</param>
        public void Movement(uint requestorId, ushort clientThingId, Location fromLocation, byte fromIndex, uint fromCreatureId, Location toLocation, uint toCreatureId, byte amount = 1)
        {
            this.DispatchOperation(
                new MovementOperationCreationArguments(
                    requestorId,
                    clientThingId,
                    fromLocation,
                    fromIndex,
                    fromCreatureId,
                    toLocation,
                    toCreatureId,
                    amount));
        }

        /// <summary>
        /// Sends a heartbeat to the player's client.
        /// </summary>
        /// <param name="player">The player which to send the heartbeat to.</param>
        public void SendHeartbeat(IPlayer player)
        {
            if (player == null)
            {
                return;
            }

            this.scheduler.ScheduleEvent(new GenericNotification(() => player.YieldSingleItem(), new GenericNotificationArguments(new HeartbeatPacket())));
        }

        /// <summary>
        /// Sends a heartbeat response to the player's client.
        /// </summary>
        /// <param name="player">The player which to send the heartbeat response to.</param>
        public void SendHeartbeatResponse(IPlayer player)
        {
            if (player == null)
            {
                return;
            }

            this.scheduler.ScheduleEvent(new GenericNotification(() => player.YieldSingleItem(), new GenericNotificationArguments(new HeartbeatResponsePacket())));
        }

        /// <summary>
        /// Dispatches a new operation created using the supplied parameters.
        /// </summary>
        /// <param name="operationArguments">The parameters used to create the operation.</param>
        /// <param name="withDelay">Optional. A delay to dispatch the operation with.</param>
        private void DispatchOperation(IOperationCreationArguments operationArguments, TimeSpan withDelay = default)
        {
            IOperation newOperation = this.operationFactory.Create(operationArguments);

            if (newOperation == null)
            {
                return;
            }

            // Normalize delay to protect against negative time spans.
            var operationDelay = withDelay < TimeSpan.Zero ? TimeSpan.Zero : withDelay;

            // Add delay from current exhaustion of the requestor, if any.
            if (operationArguments.RequestorId > 0 && this.creatureManager.FindCreatureById(operationArguments.RequestorId) is ICreatureWithExhaustion creatureWithExhaustion)
            {
                TimeSpan cooldownRemaining = creatureWithExhaustion.CalculateRemainingCooldownTime(newOperation.ExhaustionType, this.scheduler.CurrentTime);

                operationDelay += cooldownRemaining;
            }

            this.scheduler.ScheduleEvent(newOperation, operationDelay);
        }

        /// <summary>
        /// Cleans up players who's connections are ophaned, or (TODO) have been idle for some time.
        /// </summary>
        /// <param name="tokenState">The state object which gets casted into a <see cref="CancellationToken"/>.</param>.
        private void IdlePlayerSweep(object tokenState)
        {
            var cancellationToken = (tokenState as CancellationToken?).Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Thread.Sleep is OK here because this runs on it's own thread.
                Thread.Sleep(IdleCheckDelay);

                // Now let's clean up and try to log out all orphaned ones.
                foreach (var player in this.creatureManager.FindAllPlayers())
                {
                    if (player.Client != null && !player.Client.IsIdle)
                    {
                        this.SendHeartbeat(player);

                        continue;
                    }

                    this.DispatchOperation(new LogOutOperationCreationArguments(player));

                    if (player is ICombatant playerAsCombatant)
                    {
                        playerAsCombatant.SetAttackTarget(null);
                    }
                }
            }
        }

        /// <summary>
        /// Handles miscellaneous stuff on the game world, such as world light.
        /// </summary>
        /// <param name="tokenState">The state object which gets casted into a <see cref="CancellationToken"/>.</param>.
        private void MiscellaneousEventsLoop(object tokenState)
        {
            var cancellationToken = (tokenState as CancellationToken?).Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Thread.Sleep here is OK because MiscellaneousEventsLoop runs on it's own thread.
                Thread.Sleep(TimeSpan.FromMinutes(1));

                const int NightLightLevel = 30;
                const int DuskDawnLightLevel = 130;
                const int DayLightLevel = 255;

                // A day is roughly an hour in real time, and night lasts roughly 1/3 of the day in real time
                // Dusk and Dawns last for 30 minutes roughly, so les aproximate that to 2 minutes.
                var currentMinute = this.scheduler.CurrentTime.Minute;

                var currentLevel = this.worldInfo.LightLevel;
                var currentColor = this.worldInfo.LightColor;

                if (currentMinute >= 0 && currentMinute <= 37)
                {
                    // Day time: [0, 37] minutes on the hour.
                    this.worldInfo.LightLevel = DayLightLevel;
                    this.worldInfo.LightColor = (byte)LightColors.White;
                }
                else if (currentMinute == 38 || currentMinute == 39 || currentMinute == 58 || currentMinute == 59)
                {
                    // Dusk: [38, 39] minutes on the hour.
                    // Dawn: [58, 59] minutes on the hour.
                    this.worldInfo.LightLevel = DuskDawnLightLevel;
                    this.worldInfo.LightColor = (byte)LightColors.Orange;
                }
                else if (currentMinute >= 40 && currentMinute <= 57)
                {
                    // Night time: [40, 57] minutes on the hour.
                    this.worldInfo.LightLevel = NightLightLevel;
                    this.worldInfo.LightColor = (byte)LightColors.White;
                }

                if (this.worldInfo.LightLevel != currentLevel || this.worldInfo.LightColor != currentColor)
                {
                    this.scheduler.ScheduleEvent(
                        new WorldLightChangedNotification(
                            () => this.creatureManager.FindAllPlayers(),
                            new WorldLightChangedNotificationArguments(this.worldInfo.LightLevel, this.worldInfo.LightColor)));
                }
            }
        }

        /// <summary>
        /// Handles a signal from the scheduler that an event has been fired and begins processing it.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The arguments of the event.</param>
        private void ProcessFiredEvent(object sender, EventFiredEventArgs eventArgs)
        {
            if (sender != this.scheduler || eventArgs?.Event == null)
            {
                return;
            }

            IEvent evt = eventArgs.Event;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                evt.Execute(this.GetContextForEventType(evt.GetType()));

                sw.Stop();

                this.logger.Verbose($"Processed {evt.GetType().Name} with id: {evt.EventId}, current game time: {this.scheduler.CurrentTime.ToUnixTimeMilliseconds()}.");
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error in {evt.GetType().Name} with id: {evt.EventId}: {ex.Message}.");
                this.logger.Error(ex.StackTrace);
            }
        }

        private IEventContext GetContextForEventType(Type type)
        {
            if (typeof(IElevatedOperation).IsAssignableFrom(type))
            {
                return new ElevatedOperationContext(
                    this.logger,
                    this.mapDescriptor,
                    this.map,
                    this.creatureManager,
                    this.itemFactory,
                    this.creatureFactory,
                    this.operationFactory,
                    this.containerManager,
                    this.scheduler);
            }
            else if (typeof(IOperation).IsAssignableFrom(type))
            {
                return new OperationContext(
                    this.logger,
                    this.mapDescriptor,
                    this.map,
                    this.creatureManager,
                    this.itemFactory,
                    this.creatureFactory,
                    this.operationFactory,
                    this.containerManager,
                    this.scheduler);
            }
            else if (typeof(INotification).IsAssignableFrom(type))
            {
                return new NotificationContext(
                    this.logger,
                    this.mapDescriptor,
                    this.creatureManager,
                    this.scheduler);
            }

            return new EventContext(this.logger);
        }
    }
}
