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
        private const int GameStepSizeInMilliseconds = 500;

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
        public IPlayer PlayerLogin(CharacterEntity character, IConnection connection)
        {
            var player = this.CreatureFactory.Create(CreatureType.Player, new PlayerCreationMetadata(character.Id, character.Name, 100, 100, 100, 100, 4240)) as Player;

            this.CreatureManager.RegisterCreature(player);

            this.ConnectionManager.Register(connection);

            // TODO: check if map.CanAddCreature(playerRecord.location);
            // playerRecord.location
            IThing playerThing = player;

            playerThing.Location = Map.VeteranStart;

            if (this.map.GetTileAt(player.Location, out Tile targetTile))
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
        public bool PlayerLogout(IPlayer player)
        {
            if (!player.IsLogoutAllowed)
            {
                return false;
            }

            if (!this.map.GetTileAt(player.Location, out Tile tile))
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
                        () =>
                        {
                            var targetConnections = new List<IConnection>();

                            foreach (var connection in this.ConnectionManager.GetAllActive())
                            {
                                var p = this.CreatureManager.FindCreatureById(connection.PlayerId);

                                if (p == null || !p.CanSee(player.Location))
                                {
                                    continue;
                                }

                                targetConnections.Add(connection);
                            }

                            return targetConnections;
                        },
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
        /// Attempts to schedule a creature's walk in the provided direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to walk to.</param>
        /// <returns>True if the walk was possible and scheduled, false otherwise.</returns>
        public bool PlayerWalkToDirection(IPlayer player, Direction direction)
        {
            player.ThrowIfNull(nameof(player));

            var cooldownRemaining = player.CalculateRemainingCooldownTime(ExhaustionType.Movement, DateTimeOffset.UtcNow);

            if (cooldownRemaining > TimeSpan.Zero)
            {
                return false;
            }

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

            if (!this.map.GetTileAt(fromLoc, out Tile fromTile))
            {
                return false;
            }

            var playerStackPosition = fromTile.GetStackPositionOfThing(player);

            var movementEvent = new OnMapCreatureMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, playerStackPosition, toLoc);

            this.scheduler.ImmediateEvent(movementEvent);

            return true;
        }

        public bool PlayerMoveThing(IPlayer player, ushort clientId, Location fromLocation, byte fromStackPos, Location toLocation, byte count)
        {
            if (!this.map.GetTileAt(fromLocation, out Tile sourceTile))
            {
                return false;
            }

            var thingAtStackPos = sourceTile.GetThingAtStackPosition(this.CreatureManager, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != clientId)
            {
                return false;
            }

            if (player != thingAtStackPos)
            {
                var directionToThing = player.Location.DirectionTo(fromLocation);

                player.TurnToDirection(directionToThing);

                if (this.map.GetTileAt(player.Location, out Tile playerTile))
                {
                    var playerStackPos = playerTile.GetStackPositionOfThing(player);

                    this.RequestNofitication(
                        new CreatureTurnedNotification(
                            this.Logger,
                            () => this.GetConnectionsOfPlayersThatCanSee(player.Location),
                            new CreatureTurnedNotificationArguments(player, playerStackPos)));
                }
            }

            var locationDiff = fromLocation - toLocation;

            bool moveSuccessful = this.MoveThingBetweenTiles(thingAtStackPos, fromLocation, fromStackPos, toLocation, count, thingAtStackPos is ICreature ? locationDiff.MaxValueIn2D > 1 : false);

            if (moveSuccessful && player != thingAtStackPos)
            {
                var directionToDestination = player.Location.DirectionTo(toLocation);

                player.TurnToDirection(directionToDestination);

                if (this.map.GetTileAt(player.Location, out Tile playerTile))
                {
                    var playerStackPos = playerTile.GetStackPositionOfThing(player);

                    this.RequestNofitication(
                        new CreatureTurnedNotification(
                            this.Logger,
                            () => this.GetConnectionsOfPlayersThatCanSee(player.Location),
                            new CreatureTurnedNotificationArguments(player, playerStackPos)));
                }
            }

            return moveSuccessful;
        }

        public void PlayerTurnToDirection(IPlayer player, Direction direction)
        {
            player.TurnToDirection(direction);

            if (this.map.GetTileAt(player.Location, out Tile playerTile))
            {
                var playerStackPos = playerTile.GetStackPositionOfThing(player);

                this.RequestNofitication(
                    new CreatureTurnedNotification(
                        this.Logger,
                        () => this.GetConnectionsOfPlayersThatCanSee(player.Location),
                        new CreatureTurnedNotificationArguments(player, playerStackPos)));
            }
        }

        public bool MoveThingBetweenTiles(IThing thing, Location fromTileLocation, byte fromTileStackPos, Location toTileLocation, byte amountToMove = 1, bool isTeleport = false)
        {
            if (thing == null || !this.map.GetTileAt(fromTileLocation, out Tile fromTile) || !this.map.GetTileAt(toTileLocation, out Tile toTile))
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

        public ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, location);
        }

        public ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, ushort startX, ushort startY, sbyte startZ, sbyte endZ, byte windowSizeX = IMap.DefaultWindowSizeX, byte windowSizeY = IMap.DefaultWindowSizeY, sbyte startingZOffset = 0)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, startX, (ushort)(startX + windowSizeX), startY, (ushort)(startY + windowSizeY), startZ, endZ);
        }

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

            // var eventsToSchedule =

            // // handle all creature thinking and decision making.
            //    this.AdvanceTimeForThinking(timeStep)

            // // handle speech for everything.
            //    .Union(this.AdvanceTimeForSpeech(timeStep))

            // // handle all creature moving and actions.
            //    .Union(this.AdvanceTimeForMoving(timeStep))

            // // handle combat.
            //    .Union(this.AdvanceTimeForCombat(timeStep))

            // // handle miscellaneous things like day cycle.
            //    .Union(this.AdvanceTimeForMiscellaneous(timeStep));

            // foreach (var (evt, delay) in eventsToSchedule)
            // {
            //    this.ScheduleEvent(evt, delay - (DateTimeOffset.UtcNow - this.CurrentTime));
            // }
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
                        this.PlayerLogout(player);
                    }
                }
            }
        }

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
