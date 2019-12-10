// -----------------------------------------------------------------
// <copyright file="Game.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Data.Entities;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.MovementEvents;
    using OpenTibia.Server.Notifications;
    using Serilog;

    /// <summary>
    /// Class that represents the game instance.
    /// </summary>
    public class Game : IGame
    {
        /// <summary>
        /// The default adavance time step span.
        /// </summary>
        private const int GameStepSizeInMilliseconds = 100;

        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks for orphaned conections.
        /// </summary>
        private static readonly TimeSpan CheckOrphanConnectionsDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets the current <see cref="IMap"/> instance.
        /// </summary>
        private readonly IMap map;

        /// <summary>
        /// Stores a reference to the global scheduler.
        /// </summary>
        private readonly IScheduler scheduler;

        /// <summary>
        /// Stores a reference to the pathfinder helper algorithm.
        /// </summary>
        private readonly IPathFinder pathFinder;

        private readonly Queue<(IEvent, TimeSpan)> requestedMovementEventsQueue;

        private readonly object requestedMovementEventsQueueLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="map">A reference to the map to use.</param>
        /// <param name="scheduler">A reference to the scheduler in use.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        public Game(
            ILogger logger,
            IMap map,
            IScheduler scheduler,
            IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            // IItemEventLoader itemEventLoader,
            // IMonsterLoader monsterLoader,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory)
        {
            this.Logger = logger.ForContext<Game>();
            this.ConnectionManager = connectionManager;
            this.CreatureManager = creatureManager;
            this.CreatureFactory = creatureFactory;
            this.ItemFactory = itemFactory;

            // this.NotificationFactory = notificationFactory;

            // this.CombatQueue = new ConcurrentQueue<ICombatOperation>();
            this.requestedMovementEventsQueue = new Queue<(IEvent, TimeSpan)>();
            this.requestedMovementEventsQueueLock = new object();

            this.map = map;
            this.scheduler = scheduler;

            // this.eventLoader = itemEventLoader;
            // this.itemLoader = itemLoader;
            // this.monsterLoader = monsterLoader;

            // Initialize game vars.
            this.Status = WorldState.Loading;
            this.WorldLightColor = (byte)LightColors.White;
            this.WorldLightLevel = (byte)LightLevels.World;

            // this.eventsCatalog = this.eventLoader.Load(ServerConfiguration.MoveUseFileName);

            this.scheduler.OnEventFired += this.ProcessSchedulerFiredEvent;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the connection manager instance.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Gets the creature manager instance.
        /// </summary>
        public ICreatureManager CreatureManager { get; }

        /// <summary>
        /// Gets the creature factory instance.
        /// </summary>
        public ICreatureFactory CreatureFactory { get; }

        /// <summary>
        /// Gets the item factory instance.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets the game's current time.
        /// </summary>
        public DateTimeOffset CurrentTime { get; private set; }

        /// <summary>
        /// Gets the game world's light color.
        /// </summary>
        public byte WorldLightColor { get; private set; }

        /// <summary>
        /// Gets the game world's light level.
        /// </summary>
        public byte WorldLightLevel { get; private set; }

        /// <summary>
        /// Gets the game world's status.
        /// </summary>
        public WorldState Status { get; private set; }

        /// <summary>
        /// Attempts to log a player in to the game.
        /// </summary>
        /// <param name="character">The character that the player is logging in to.</param>
        /// <param name="connection">The connection that the player uses.</param>
        /// <returns>An instance of the new <see cref="IPlayer"/> in the game.</returns>
        public IPlayer PlayerRequest_Login(CharacterEntity character, IConnection connection)
        {
            var player = this.CreatureFactory.Create(CreatureType.Player, new PlayerCreationMetadata(character.Id, character.Name, 100, 100, 100, 100, 4240)) as Player;

            this.CreatureManager.RegisterCreature(player);

            this.ConnectionManager.Register(connection);

            // TODO: check if map.CanAddCreature(playerRecord.location);
            // playerRecord.location
            IThing playerThing = player;

            playerThing.Location = Map.VeteranStart;

            if (this.map.GetTileAt(player.Location, out ITile targetTile))
            {
                targetTile.AddThing(this.ItemFactory, ref playerThing);
            }

            return player;
        }

        /// <summary>
        /// Attempts to log a player out of the game.
        /// </summary>
        /// <param name="player">The player to attempt to attempt log out.</param>
        /// <returns>True if the log-out was successful, false otherwise.</returns>
        public bool PlayerRequest_Logout(IPlayer player)
        {
            if (!player.IsLogoutAllowed)
            {
                return false;
            }

            if (!this.map.GetTileAt(player.Location, out ITile tile))
            {
                return false;
            }

            // TODO: stuff missing?
            // At this point, we're allowed to log this player out, so go for it.
            var oldStackpos = tile.GetStackPositionOfThing(player);

            IThing playerThing = player as IThing;

            if (tile.RemoveThing(this.ItemFactory, ref playerThing))
            {
                this.RequestNofitication(
                    new CreatureRemovedNotification(
                        this.Logger,
                        this.CreatureManager,
                        () => this.GetConnectionsOfPlayersThatCanSee(player.Location),
                        new CreatureRemovedNotificationArguments(player, oldStackpos, AnimatedEffect.Puff)));

                this.CreatureManager.UnregisterCreature(player);

                var currentConnection = this.ConnectionManager.FindByPlayerId(player.Id);

                if (currentConnection != null)
                {
                    this.ConnectionManager.Unregister(currentConnection);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to move a thing on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="fromLocation">The location from which the thing is being moved.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="count">The amount of the thing that is being moved.</param>
        /// <returns>True if the thing movement was accepted, false otherwise.</returns>
        public bool PlayerRequest_MoveThing(IPlayer player, ushort thingId, Location fromLocation, byte fromStackPos, Location toLocation, byte count)
        {
            if (!this.map.GetTileAt(fromLocation, out ITile sourceTile))
            {
                return false;
            }

            var thingAtStackPos = sourceTile.GetThingAtStackPosition(this.CreatureManager, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != thingId)
            {
                return false;
            }

            IEvent movementEvent = null;

            // Seems like a valid movement for now, enqueue it.
            if (thingAtStackPos is ICreature creature)
            {
                movementEvent = new OnMapCreatureMovementEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    player.Id,
                    creature,
                    fromLocation,
                    fromStackPos,
                    toLocation);
            }
            else if (thingAtStackPos is IItem item)
            {
                movementEvent = new OnMapMovementEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    player.Id,
                    item,
                    fromLocation,
                    fromStackPos,
                    toLocation);
            }

            if (movementEvent != null)
            {
                lock (this.requestedMovementEventsQueueLock)
                {
                    this.requestedMovementEventsQueue.Enqueue((movementEvent, TimeSpan.FromMilliseconds(100)));
                }
            }

            return movementEvent != null;
        }

        /// <summary>
        /// Attempts to turn a player to the requested direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to turn to.</param>
        /// <returns>True if the turn request was accepted, false otherwise.</returns>
        public bool PlayerRequest_TurnToDirection(IPlayer player, Direction direction)
        {
            player.ThrowIfNull(nameof(player));

            // TODO: should this be scheduled too?
            player.TurnToDirection(direction);

            if (this.map.GetTileAt(player.Location, out ITile playerTile))
            {
                var playerStackPos = playerTile.GetStackPositionOfThing(player);

                this.RequestNofitication(
                    new CreatureTurnedNotification(
                        this.Logger,
                        () => this.GetConnectionsOfPlayersThatCanSee(player.Location),
                        new CreatureTurnedNotificationArguments(player, playerStackPos)));
            }

            return true;
        }

        /// <summary>
        /// Attempts to schedule a player's walk in the provided direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to walk to.</param>
        /// <returns>True if the walk was possible and scheduled, false otherwise.</returns>
        public bool PlayerRequest_WalkToDirection(IPlayer player, Direction direction)
        {
            player.ThrowIfNull(nameof(player));

            var fromLoc = player.Location;
            var toLoc = fromLoc;

            switch (direction)
            {
                case Direction.North:
                    toLoc.Y -= 1;
                    break;
                case Direction.South:
                    toLoc.Y += 1;
                    break;
                case Direction.East:
                    toLoc.X += 1;
                    break;
                case Direction.West:
                    toLoc.X -= 1;
                    break;
                case Direction.NorthEast:
                    toLoc.X += 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLoc.X -= 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLoc.X += 1;
                    toLoc.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLoc.X -= 1;
                    toLoc.Y += 1;
                    break;
            }

            if (!this.map.GetTileAt(fromLoc, out ITile fromTile))
            {
                return false;
            }

            var playerStackPosition = fromTile.GetStackPositionOfThing(player);

            if (playerStackPosition == byte.MaxValue)
            {
                return false;
            }

            // Seems like a valid movement for now, enqueue it.
            var movementEvent = new OnMapCreatureMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, playerStackPosition, toLoc);
            var cooldownRemaining = player.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.CurrentTime);

            lock (this.requestedMovementEventsQueueLock)
            {
                this.requestedMovementEventsQueue.Enqueue((movementEvent, cooldownRemaining > TimeSpan.Zero ? cooldownRemaining : TimeSpan.Zero));
            }

            return true;
        }

        /// <summary>
        /// Inmediately attempts to perform a thing movement between two tiles.
        /// </summary>
        /// <param name="thing">The thing being moved.</param>
        /// <param name="fromTileLocation">The tile from which the movement is being performed.</param>
        /// <param name="fromTileStackPos">The position in the stack of the tile from which the movement is being performed.</param>
        /// <param name="toTileLocation">The tile to which the movement is being performed.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the move is considered a teleportation. Defaults to false.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformThingMovementBetweenTiles(IThing thing, Location fromTileLocation, byte fromTileStackPos, Location toTileLocation, byte amountToMove = 1, bool isTeleport = false)
        {
            if (thing == null || !this.map.GetTileAt(fromTileLocation, out ITile fromTile) || !this.map.GetTileAt(toTileLocation, out ITile toTile))
            {
                return false;
            }

            // Do the actual move first.
            if (!fromTile.RemoveThing(this.ItemFactory, ref thing, amountToMove))
            {
                return false;
            }

            toTile.AddThing(this.ItemFactory, ref thing, amountToMove);

            thing.Location = toTileLocation;

            var moveDirection = fromTileLocation.DirectionTo(toTileLocation);

            // Then deal with the consequences of the move.
            if (thing is ICreature creature)
            {
                creature.TurnToDirection(moveDirection);

                var thingStackPosition = toTile.GetStackPositionOfThing(thing);

                if (thingStackPosition != byte.MaxValue)
                {
                    this.RequestNofitication(
                        new CreatureMovedNotification(
                            this.Logger,
                            this,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(fromTileLocation, toTileLocation),
                            new CreatureMovedNotificationArguments(
                                (thing as ICreature).Id,
                                fromTileLocation,
                                fromTileStackPos,
                                toTileLocation,
                                thingStackPosition,
                                isTeleport)));
                }
            }
            else
            {
                // TODO: see if we can save network bandwith here:
                // this.Game.RequestNofitication(
                //        new ItemMovedNotification(
                //            (IItem)this.Thing,
                //            this.FromLocation,
                //            oldStackpos,
                //            this.ToLocation,
                //            destinationTile.GetStackPosition(this.Thing),
                //            false
                //        ),
                //        this.FromLocation,
                //        this.ToLocation
                //    );

                this.RequestNofitication(
                    new TileUpdatedNotification(
                            this.Logger,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(fromTileLocation),
                            new TileUpdatedNotificationArguments(
                                fromTileLocation,
                                this.GetDescriptionOfTile)));

                this.RequestNofitication(
                    new TileUpdatedNotification(
                            this.Logger,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(toTileLocation),
                            new TileUpdatedNotificationArguments(
                                toTileLocation,
                                this.GetDescriptionOfTile)));
            }

            //if (fromTile.HasSeparationEvents)
            //{
            //    foreach (var itemWithSeparation in fromTile.ItemsWithSeparation)
            //    {
            //        var separationEvents = this.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

            //        var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, this.Requestor as IPlayer) && e.CanBeExecuted);

            //        // Execute all actions.
            //        candidate?.Execute();
            //    }
            //}

            //if (toTile.HasCollisionEvents)
            //{
            //    foreach (var itemWithCollision in toTile.ItemsWithCollision)
            //    {
            //        var collisionEvents = this.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

            //        var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, this.Requestor as IPlayer) && e.CanBeExecuted);

            //        // Execute all actions.
            //        candidate?.Execute();
            //    }
            //}

            //if (thing is ICreature creature && !isTeleport)
            //{
            //    // update both creature's to face the push direction... a *real* push!
            //    if (this.Requestor != this.Thing)
            //    {
            //        this.Requestor?.TurnToDirection(this.Requestor.Location.DirectionTo(this.Thing.Location));
            //    }

            //    var attemptedDirection = fromLocation.DirectionTo(toLocation, true);
            //    ((Creature)this.Thing)?.TurnToDirection(attemptedDirection);

            //    if (this.Requestor != null && this.Requestor == this.Thing)
            //    {
            //        this.Requestor.UpdateLastStepInfo(this.Requestor.NextStepId, wasDiagonal: attemptedDirection > Direction.West);
            //    }
            //}

            return true;
        }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The center location from which the description is being retrieved.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        public ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, location);
        }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player for the specified window.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="startX">The starting X coordinate of the window.</param>
        /// <param name="startY">The starting Y coordinate of the window.</param>
        /// <param name="startZ">The starting Z coordinate of the window.</param>
        /// <param name="endZ">The ending Z coordinate of the window.</param>
        /// <param name="windowSizeX">The size of the window in X.</param>
        /// <param name="windowSizeY">The size of the window in Y.</param>
        /// <param name="startingZOffset">Optional. A starting offset for Z.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        public ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, ushort startX, ushort startY, sbyte startZ, sbyte endZ, byte windowSizeX = IMap.DefaultWindowSizeX, byte windowSizeY = IMap.DefaultWindowSizeY, sbyte startingZOffset = 0)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, startX, (ushort)(startX + windowSizeX), startY, (ushort)(startY + windowSizeY), startZ, endZ);
        }

        /// <summary>
        /// Gets the description bytes of a single tile of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The location from which the description of the tile is being retrieved.</param>
        /// <returns>A sequence of bytes representing the tile's description.</returns>
        public ReadOnlySequence<byte> GetDescriptionOfTile(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            var firstSegment = new MapDescriptionSegment(ReadOnlyMemory<byte>.Empty);

            MapDescriptionSegment lastSegment = firstSegment;

            var segmentsFromTile = this.map.DescribeTileForPlayer(player, location);

            // See if we actually have segments to append.
            if (segmentsFromTile != null && segmentsFromTile.Any())
            {
                foreach (var segment in segmentsFromTile)
                {
                    lastSegment.Append(segment);
                    lastSegment = segment;
                }
            }

            // HACK: add a last segment to seal the deal.
            lastSegment = lastSegment.Append(ReadOnlyMemory<byte>.Empty);

            return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, 0);
        }

        /// <summary>
        /// Runs the main game processing thread which begins advancing time on the game engine.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var stepSize = TimeSpan.FromMilliseconds(GameStepSizeInMilliseconds);

                var connectionSweepperTask = Task.Factory.StartNew(this.ConnectionSweeper, cancellationToken, TaskCreationOptions.LongRunning);

                // Leave this at the very end, when everything is ready...
                this.Status = WorldState.Open;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var timeAtStart = DateTimeOffset.UtcNow;

                    this.AdvanceTime(stepSize);

                    var timeThatCheckTook = DateTimeOffset.UtcNow - timeAtStart;
                    var actualDelay = stepSize - timeThatCheckTook;

                    if (actualDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(actualDelay, cancellationToken);
                    }
                    else
                    {
                        this.Logger.Warning($"Time is slipping: It took {timeThatCheckTook} to advance time with a step size of {stepSize}.");
                    }
                }

                await Task.WhenAll(connectionSweepperTask);
            });

            // return this to allow other IHostedService-s to start.
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Requests a notification be scheduled to be sent.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void RequestNofitication(INotification notification)
        {
            notification.ThrowIfNull(nameof(notification));

            this.scheduler.ImmediateEvent(notification);
        }

        /// <summary>
        /// Advances Time in the game by the supplied span.
        /// </summary>
        /// <param name="timeStep">The span of time to advance the game for.</param>
        private void AdvanceTime(TimeSpan timeStep)
        {
            // store the current time for global reference and actual calculations.
            this.CurrentTime = DateTimeOffset.UtcNow;

            var eventsToSchedule =

               // handle all creature thinking and decision making.
               this.AdvanceTimeForThinking(timeStep)

               // handle speech for everything.
               .Union(this.AdvanceTimeForSpeech(timeStep))

               // handle all creature moving and actions.
               .Union(this.AdvanceTimeForMoving(timeStep))

               // handle combat.
               .Union(this.AdvanceTimeForCombat(timeStep))

               // handle miscellaneous things like day cycle.
               .Union(this.AdvanceTimeForMiscellaneous(timeStep));

            foreach (var (evt, delay) in eventsToSchedule)
            {
                this.scheduler.ScheduleEvent(evt, this.CurrentTime + delay);
            }
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForThinking(TimeSpan stepSize)
        {
            var eventsToSchedule = new List<(IEvent Event, TimeSpan Delay)>();

            foreach (var creature in this.CreatureManager.FindAllCreatures())
            {
                // TODO: make creatures think here
                // Schedule any actions that the creature would take here.
                var decisionsAndActions = creature.Think();

                if (decisionsAndActions != null && decisionsAndActions.Any())
                {
                    eventsToSchedule.AddRange(decisionsAndActions);
                }
            }

            return eventsToSchedule;
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForSpeech(TimeSpan timeStep)
        {
            var eventsToSchedule = new List<(IEvent Event, TimeSpan Delay)>();

            return eventsToSchedule;
        }

        /// <summary>
        /// Advances time for the movement queues, handling move requests from all sources.
        /// </summary>
        /// <param name="timeStep">The size of the time span to calculate with.</param>
        /// <returns>A collection of events with delay times that should be scheduled.</returns>
        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForMoving(TimeSpan timeStep)
        {
            var eventsToSchedule = new List<(IEvent Event, TimeSpan Delay)>();

            // Check the movement requests queue and schedule these applying any corresponding delay/penalties as needed.
            lock (this.requestedMovementEventsQueueLock)
            {
                while (this.requestedMovementEventsQueue.Count > 0)
                {
                    var (movementEvent, delay) = this.requestedMovementEventsQueue.Dequeue();

                    eventsToSchedule.Add((movementEvent, delay));
                }
            }

            return eventsToSchedule;
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForCombat(TimeSpan timeStep)
        {
            var eventsToSchedule = new List<(IEvent Event, TimeSpan Delay)>();

            return eventsToSchedule;
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForMiscellaneous(TimeSpan timeStep)
        {
            var events = new List<(IEvent, TimeSpan)>();

            const int NightLightLevel = 30;
            const int DuskDawnLightLevel = 130;
            const int DayLightLevel = 255;

            // A day is roughly an hour in real time, and night lasts roughly 1/3 of the day in real time
            // Dusk and Dawns last for 30 minutes roughly, so les aproximate that to 2 minutes.
            var currentMinute = this.CurrentTime.Minute;

            if (currentMinute >= 0 && currentMinute <= 37)
            {
                // Day time: [0, 37] minutes on the hour.
                if (this.WorldLightLevel != DayLightLevel)
                {
                    this.WorldLightLevel = DayLightLevel;
                    this.WorldLightColor = (byte)LightColors.White;

                    //events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.WorldLightLevel, this.WorldLightColor)) as IEvent, TimeSpan.Zero));
                }
            }
            else if (currentMinute == 38 || currentMinute == 39 || currentMinute == 58 || currentMinute == 59)
            {
                // Dusk: [38, 39] minutes on the hour.
                // Dawn: [58, 59] minutes on the hour.
                if (this.WorldLightLevel != DuskDawnLightLevel)
                {
                    this.WorldLightLevel = DuskDawnLightLevel;
                    this.WorldLightColor = (byte)LightColors.Orange;

                    //events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.WorldLightLevel, this.WorldLightColor)) as IEvent, TimeSpan.Zero));
                }
            }
            else if (currentMinute >= 40 && currentMinute <= 57)
            {
                // Night time: [40, 57] minutes on the hour.
                if (this.WorldLightLevel != NightLightLevel)
                {
                    this.WorldLightLevel = NightLightLevel;
                    this.WorldLightColor = (byte)LightColors.White;

                    //events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.WorldLightLevel, this.WorldLightColor)) as IEvent, TimeSpan.Zero));
                }
            }

            return events;
        }

        /// <summary>
        /// Cleans up stale connections.
        /// </summary>
        /// <param name="tokenState">The state object which gets casted into a <see cref="CancellationToken"/>.</param>.
        private void ConnectionSweeper(object tokenState)
        {
            var cancellationToken = (tokenState as CancellationToken?).Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(CheckOrphanConnectionsDelay);

                foreach (var orphanedConnection in this.ConnectionManager.GetAllOrphaned())
                {
                    if (!(this.CreatureManager.FindCreatureById(orphanedConnection.PlayerId) is IPlayer player))
                    {
                        continue;
                    }

                    //player.SetAttackTarget(0);

                    if (player.IsLogoutAllowed)
                    {
                        this.PlayerRequest_Logout(player);
                    }
                }
            }
        }

        /// <summary>
        /// Handles a signal from the scheduler that an event has been fired and begins processing it.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The arguments of the event.</param>
        private void ProcessSchedulerFiredEvent(object sender, EventFiredEventArgs eventArgs)
        {
            if (sender != this.scheduler || eventArgs?.Event == null)
            {
                return;
            }

            IEvent evt = eventArgs.Event;

            this.Logger.Verbose($"Processing event {evt.EventId}.");

            try
            {
                evt.Process();
                this.Logger.Verbose($"Processed event {evt.EventId}.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex.Message);
                this.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Gets the connections of any players that can see the given locations.
        /// </summary>
        /// <param name="locations">The locations to check if players can see.</param>
        /// <returns>A collection of connections.</returns>
        private IEnumerable<IConnection> GetConnectionsOfPlayersThatCanSee(params Location[] locations)
        {
            var activeConnections = this.ConnectionManager.GetAllActive();

            foreach (var connection in activeConnections)
            {
                var player = this.CreatureManager.FindCreatureById(connection.PlayerId);

                if (player == null || !locations.Any(loc => player.CanSee(loc)))
                {
                    continue;
                }

                yield return connection;
            }
        }
    }
}
