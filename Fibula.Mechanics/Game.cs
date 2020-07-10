// -----------------------------------------------------------------
// <copyright file="Game.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
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

        /// <summary>
        /// The current map descriptor.
        /// </summary>
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
        /// The pathfinder algorithm in use.
        /// </summary>
        private readonly IPathFinder pathFinder;

        /// <summary>
        /// The predefined item set declared.
        /// </summary>
        private readonly IPredefinedItemSet predefinedItemSet;

        /// <summary>
        /// Stores the world information.
        /// </summary>
        private readonly WorldInformation worldInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="map">A reference to the map.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A referecne to the operation factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="pathFinderAlgo">A reference to the path finding algorithm in use.</param>
        /// <param name="predefinedItemSet">A reference to the predefined item set declared.</param>
        /// <param name="scheduler">A reference to the global scheduler instance.</param>
        public Game(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            IMap map,
            ICreatureManager creatureManager,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IOperationFactory operationFactory,
            IContainerManager containerManager,
            IPathFinder pathFinderAlgo,
            IPredefinedItemSet predefinedItemSet,
            IScheduler scheduler)
        {
            logger.ThrowIfNull(nameof(logger));
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            map.ThrowIfNull(nameof(map));
            creatureManager.ThrowIfNull(nameof(creatureManager));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            operationFactory.ThrowIfNull(nameof(operationFactory));
            containerManager.ThrowIfNull(nameof(containerManager));
            pathFinderAlgo.ThrowIfNull(nameof(pathFinderAlgo));
            predefinedItemSet.ThrowIfNull(nameof(predefinedItemSet));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.logger = logger.ForContext<Game>();
            this.mapDescriptor = mapDescriptor;
            this.map = map;
            this.creatureManager = creatureManager;
            this.itemFactory = itemFactory;
            this.creatureFactory = creatureFactory;
            this.operationFactory = operationFactory;
            this.containerManager = containerManager;
            this.pathFinder = pathFinderAlgo;
            this.predefinedItemSet = predefinedItemSet;
            this.scheduler = scheduler;

            // Initialize game vars.
            this.worldInfo = new WorldInformation()
            {
                Status = WorldState.Loading,
                LightColor = (byte)LightColors.White,
                LightLevel = (byte)LightLevels.World,
            };

            // Hook some event handlers.
            this.scheduler.EventFired += this.ProcessFiredEvent;

            // this.map.Loader.WindowLoaded += this.OnMapWindowLoaded;
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
        /// Creates item at the specified location.
        /// </summary>
        /// <param name="location">The location at which to create the item.</param>
        /// <param name="itemType">The type of item to create.</param>
        /// <param name="additionalAttributes">Optional. Additional item attributes to set on the new item.</param>
        public void CreateItemAtLocation(Location location, IItemType itemType, params (ItemAttribute, IConvertible)[] additionalAttributes)
        {
            if (itemType == null)
            {
                return;
            }

            var attributesToSet = itemType.DefaultAttributes.Select(kvp => (kvp.Key, kvp.Value));

            if (additionalAttributes != null)
            {
                attributesToSet.Union(additionalAttributes);
            }

            this.DispatchOperation(new CreateItemOperationCreationArguments(requestorId: 0, itemType.TypeId, location, attributesToSet.ToArray()));
        }

        /// <summary>
        /// Creates a new item at the specified location.
        /// </summary>
        /// <param name="location">The location at which to create the item.</param>
        /// <param name="itemTypeId">The type id of the item to create.</param>
        /// <param name="additionalAttributes">Optional. Additional item attributes to set on the new item.</param>
        public void CreateItemAtLocation(Location location, ushort itemTypeId, params (ItemAttribute, IConvertible)[] additionalAttributes)
        {
            this.DispatchOperation(new CreateItemOperationCreationArguments(requestorId: 0, itemTypeId, location, additionalAttributes));
        }

        /// <summary>
        /// Handles a health change from a combatant.
        /// </summary>
        /// <param name="combatant">The combatant who's health changed.</param>
        /// <param name="oldHealthValue">The old value of the combatant's health.</param>
        public void CombatantHealthChanged(ICombatant combatant, ushort oldHealthValue)
        {
            if (combatant is IPlayer playerCombatant)
            {
                // This updates the health as seen by others.
                this.scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => this.map.PlayersThatCanSee(playerCombatant.Location),
                        new GenericNotificationArguments(new CreatureHealthPacket(playerCombatant))));

                // And this updates the health of the player.
                this.scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => playerCombatant.YieldSingleItem(),
                        new GenericNotificationArguments(new PlayerStatsPacket(playerCombatant))));
            }
        }

        /// <summary>
        /// Handles a death from a combatant.
        /// </summary>
        /// <param name="combatant">The combatant that died.</param>
        public void CombatantDeath(ICombatant combatant)
        {
            if (combatant is IPlayer playerCombatant)
            {
                this.CancelPlayerActions(playerCombatant);
            }

            var rng = new Random();
            var deathDelay = TimeSpan.FromMilliseconds(rng.Next(2000));

            this.DispatchOperation(new DeathOperationCreationArguments(requestorId: 0, combatant), deathDelay);
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
        /// <param name="strategy">Optional. The strategy to follow in the plan.</param>
        public void SetCreatureStaticWalkPlan(ICreature creature, Direction[] directions, WalkStrategy strategy = WalkStrategy.DoNotRecalculate)
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

            creature.WalkPlan = new WalkPlan(strategy, () => lastLoc, waypoints.ToArray());

            this.scheduler.CancelAllFor(creature.Id, typeof(AutoWalkOrchestratorOperation));

            this.DispatchOperation(new AutoWalkOrchestratorOperationCreationArguments(creature));
        }

        /// <summary>
        /// Re-sets a given creature's walk plan and kicks it off.
        /// </summary>
        /// <param name="creature">The creature to reset the walk plan of.</param>
        /// <param name="targetCreature">The creature towards which the walk plan will be generated to.</param>
        /// <param name="strategy">Optional. The strategy to follow in the plan.</param>
        public void SetCreatureDynamicWalkPlan(ICreature creature, ICreature targetCreature, WalkStrategy strategy = WalkStrategy.ConservativeRecalculation)
        {
            if (creature == null || targetCreature == null)
            {
                return;
            }

            var directions = this.pathFinder.FindBetween(creature.Location, targetCreature.Location, out Location endLocation, creature);

            var waypoints = new List<Location>()
            {
                creature.Location,
            };

            var lastLoc = creature.Location;

            // Calculate and add the waypoints.
            foreach (var dir in directions)
            {
                var nextLoc = lastLoc.LocationAt(dir);

                waypoints.Add(nextLoc);

                lastLoc = nextLoc;
            }

            creature.WalkPlan = new WalkPlan(strategy, () => targetCreature.Location, waypoints.ToArray());

            this.scheduler.CancelAllFor(creature.Id, typeof(AutoWalkOrchestratorOperation));

            this.DispatchOperation(new AutoWalkOrchestratorOperationCreationArguments(creature));
        }

        /// <summary>
        /// Cancels all actions that a player has pending.
        /// </summary>
        /// <param name="player">The player to cancel actions for.</param>
        /// <param name="typeOfActionToCancel">Optional. The specific type of action to cancel.</param>
        /// <param name="async">Optional. A value indicating whether to execute the cancellation asynchronously.</param>
        public void CancelPlayerActions(IPlayer player, Type typeOfActionToCancel = null, bool async = false)
        {
            if (player == null || (typeOfActionToCancel != null && !typeof(IOperation).IsAssignableFrom(typeOfActionToCancel)))
            {
                return;
            }

            var cancelOp = this.operationFactory.Create(new CancelOperationsOperationCreationArguments(player, typeOfActionToCancel));

            if (async)
            {
                this.scheduler.ScheduleEvent(cancelOp);
                return;
            }

            cancelOp.Execute(this.GetContextForEventType(cancelOp.GetType()));
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
        /// Describes a thing for a player that is looking at it.
        /// </summary>
        /// <param name="thingId">The id of the thing to describe.</param>
        /// <param name="location">The location of the thing to describe.</param>
        /// <param name="stackPosition">The position in the stack within the location of the thing to describe.</param>
        /// <param name="player">The player for which to describe the thing for.</param>
        public void LookAt(ushort thingId, Location location, byte stackPosition, IPlayer player)
        {
            this.DispatchOperation(new LookAtOperationCreationArguments(thingId, location, stackPosition, player));
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
            var scheduleDelay = TimeSpan.Zero;

            var creature = this.creatureManager.FindCreatureById(fromCreatureId);

            var movementOp = this.operationFactory.Create(
                new MovementOperationCreationArguments(
                    requestorId,
                    clientThingId,
                    fromLocation,
                    fromIndex,
                    fromCreatureId,
                    toLocation,
                    toCreatureId,
                    amount));

            // Add delay from current exhaustion of the requestor if it's a creature, and this is a movement in the map.
            if (requestorId == fromCreatureId &&
                fromLocation.Type == LocationType.Map &&
                toLocation.Type == LocationType.Map &&
                creature is ICreatureWithExhaustion creatureWithExhaustion)
            {
                // The scheduling delay becomes any cooldown debt for this operation.
                scheduleDelay = creatureWithExhaustion.CalculateRemainingCooldownTime(movementOp.ExhaustionType, this.scheduler.CurrentTime);
            }

            this.scheduler.ScheduleEvent(movementOp, scheduleDelay);
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
                        if (player.Client.Information.Type == AgentType.Linux ||
                            player.Client.Information.Type == AgentType.OtClientLinux ||
                            player.Client.Information.Type == AgentType.OtClientWindows)
                        {
                            this.SendHeartbeat(player);
                        }

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
                    this,
                    this,
                    this.pathFinder,
                    this.predefinedItemSet,
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
                    this,
                    this,
                    this.pathFinder,
                    this.predefinedItemSet,
                    this.scheduler);
            }
            else if (typeof(INotification).IsAssignableFrom(type))
            {
                return new NotificationContext(
                    this.logger,
                    this.mapDescriptor,
                    this.creatureManager);
            }

            return new EventContext(this.logger);
        }
    }
}
