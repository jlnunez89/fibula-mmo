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

namespace Fibula.Server.Mechanics
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Notifications;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Delegates;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Contracts.Enumerations;
    using Fibula.Server.Notifications;
    using Fibula.Server.Notifications.Arguments;
    using Fibula.Server.Notifications.Contracts.Abstractions;
    using Fibula.Server.Operations;
    using Fibula.Server.Operations.Arguments;
    using Fibula.Server.Operations.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents the game instance.
    /// </summary>
    public class Game : IGame// , IEventRulesApi, ICombatApi
    {
        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks for idle players and connections.
        /// </summary>
        private static readonly TimeSpan IdleCheckDelay = TimeSpan.FromMinutes(1);

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
        /// The current <see cref="ITileAccessor"/> instance.
        /// </summary>
        private readonly ITileAccessor tileAccessor;

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
        /// <param name="mapDescriptor">A reference to the map descriptor to use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor to use.</param>
        /// <param name="pathFinder">A reference to the pathfinder algorithm helper instance.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="eventRulesLoader">A reference to the event rules loader.</param>
        /// <param name="monsterSpawnsLoader">A reference to the monster spawns loader.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="scheduler">A reference to the global scheduler instance.</param>
        public Game(
            ILogger logger,
            IMapLoader mapLoader,
            IMapDescriptor mapDescriptor,
            ITileAccessor tileAccessor,
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
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            creatureManager.ThrowIfNull(nameof(creatureManager));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            operationFactory.ThrowIfNull(nameof(operationFactory));
            containerManager.ThrowIfNull(nameof(containerManager));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.logger = logger.ForContext<Game>();
            this.mapDescriptor = mapDescriptor;
            this.tileAccessor = tileAccessor;
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
                var creatureThinkingTask = Task.Factory.StartNew(this.CreatureThinkingLoop, cancellationToken, TaskCreationOptions.LongRunning);

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

        public void CreatureSpeech(uint creatureId, SpeechType speechType, ChatChannelType channelType, string content, string receiver = "")
        {
            this.DispatchOperation(new SpeechOperationCreationArguments(creatureId, speechType, channelType, content, receiver));
        }

        public void LogPlayerIn(IPlayerCreationMetadata playerCreationMetadata)
        {
            playerCreationMetadata.ThrowIfNull(nameof(playerCreationMetadata));

            this.DispatchOperation(
                new LogInOperationCreationArguments(
                    playerCreationMetadata,
                    this.WorldInfo.LightLevel,
                    this.WorldInfo.LightColor));
        }

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

        private void DispatchOperation(IOperationCreationArguments operationArguments, TimeSpan withDelay = default)
        {
            IOperation newOperation = this.operationFactory.Create(operationArguments);

            if (newOperation == null)
            {
                return;
            }

            // Normalize delay to protect against negative time spans.
            var operationDelay = withDelay < TimeSpan.Zero ? TimeSpan.Zero : withDelay;

            //// Add delay from current exhaustion of the requestor, if any.
            //if (operationArguments.RequestorId > 0 && this.creatureManager.FindCreatureById(operationArguments.RequestorId) is ISuffersExhaustion creatureWithExhaustion)
            //{
            //    TimeSpan cooldownRemaining = creatureWithExhaustion.CalculateRemainingCooldownTime(newOperation.ExhaustionType, this.scheduler.CurrentTime);

            //    operationDelay += cooldownRemaining;
            //}

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
                        continue;
                    }

                    //if (player is ICombatant playerAsCombatant)
                    //{
                    //    playerAsCombatant.SetAttackTarget(null);
                    //}

                    //this.DispatchOperation(OperationType.LogOut, new LogOutOperationCreationArguments(player));
                }
            }
        }

        /// <summary>
        /// Handles creature 'thinking' in the game.
        /// </summary>
        /// <param name="tokenState">The state object which gets casted into a <see cref="CancellationToken"/>.</param>.
        private void CreatureThinkingLoop(object tokenState)
        {
            var cancellationToken = (tokenState as CancellationToken?).Value;

            // TODO: seed pseudo-RNGs.
            var rng = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Thread.Sleep here is OK because CreatureThinkingLoop runs on it's own thread.
                Thread.Sleep(TimeSpan.FromSeconds(1));

                // Get all the creatures in the game that are 'thinking' and randomize the order to make it fair.
                var allActiveCreatures = this.creatureManager.FindAllCreatures().Where(c => c.IsThinking).OrderBy(c => rng.Next());

                //foreach (var creature in allActiveCreatures)
                //{
                //    // Dispatch a thinking operation for them.
                //    this.DispatchOperation(OperationType.Thinking, new ThinkingOperationCreationArguments(creature.Id, creature));
                //}
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

                this.logger.Verbose($"Processed event {evt.GetType().Name} with id: {evt.EventId}, current game time: {this.scheduler.CurrentTime.ToUnixTimeMilliseconds()}.");
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error in event {evt.GetType().Name} with id: {evt.EventId}: {ex.Message}.");
                this.logger.Error(ex.StackTrace);
            }

            // TODO: this should be added to the BaseOperation rather than check the type here...
            //finally
            //{
            //    if (evt is IOperation operation)
            //    {
            //        // Add any exhaustion for the requestor of the operation, if any.
            //        if (this.creatureManager.FindCreatureById(operation.RequestorId) is ICreature requestor)
            //        {
            //            requestor.AddExhaustion(operation.ExhaustionType, this.scheduler.CurrentTime, operation.ExhaustionCost);
            //        }
            //    }
            //}
        }

        private IEventContext GetContextForEventType(Type type)
        {
            if (typeof(IElevatedOperation).IsAssignableFrom(type))
            {
                return new ElevatedOperationContext(
                    this.logger,
                    this.mapDescriptor,
                    this.tileAccessor,
                    this.creatureManager,
                    //this.pathFinder,
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
                    this.tileAccessor,
                    this.creatureManager,
                    //this.pathFinder,
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
