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
    using OpenTibia.Scheduling;
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
        public static Location NewbieStart = new Location { X = 32097, Y = 32219, Z = 7 };

        public static Location VeteranStart = new Location { X = 32369, Y = 32241, Z = 7 };

        public static Location HellsGate = new Location { X = 32675, Y = 31648, Z = 10 };

        /// <summary>
        /// Default delay for scripts.
        /// </summary>
        private static readonly TimeSpan DefaultDelayForScripts = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Default delat for player actions.
        /// </summary>
        private static readonly TimeSpan DefaultPlayerActionDelay = TimeSpan.FromMilliseconds(100);

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
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> containing the <see cref="IEventRule"/>s of the game.
        /// </summary>
        private readonly IDictionary<EventRuleType, ISet<IEventRule>> eventRulesCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="map">A reference to the map to use.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="eventRulesLoader">A reference to the event rules loader.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="scriptApi">A reference to the script api in use.</param>
        public Game(
            ILogger logger,
            IMap map,
            IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            IEventRulesLoader eventRulesLoader,
            // IMonsterLoader monsterLoader,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IScriptApi scriptApi)
        {
            this.Logger = logger.ForContext<Game>();
            this.ConnectionManager = connectionManager;
            this.CreatureManager = creatureManager;
            this.CreatureFactory = creatureFactory;
            this.ItemFactory = itemFactory;

            this.scheduler = new Scheduler(logger);

            this.map = map;

            // TODO: This is a hack. Need to fix this indirection.
            scriptApi.Game = this;

            this.eventRulesCatalog = eventRulesLoader.LoadEventRules();

            // this.monsterLoader = monsterLoader;

            // Initialize game vars.
            this.Status = WorldState.Loading;
            this.WorldLightColor = (byte)LightColors.White;
            this.WorldLightLevel = (byte)LightLevels.World;

            this.scheduler.OnEventFired += this.ProcessEvent;
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
        /// Gets the game's current time.
        /// </summary>
        public DateTimeOffset CurrentTime => this.scheduler.CurrentTime;

        /// <summary>
        /// Attempts to schedule a player's auto walk movements.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="directions">The directions to walk to.</param>
        /// <returns>True if the auto walk request was accepted, false otherwise.</returns>
        public bool PlayerRequest_AutoWalk(IPlayer player, Direction[] directions)
        {
            player.ThrowIfNull(nameof(player));

            Location fromLoc = player.Location;

            if (!this.map.GetTileAt(fromLoc, out ITile fromTile))
            {
                return false;
            }

            var playerStackPosition = fromTile.GetStackPositionOfThing(player);

            if (playerStackPosition == byte.MaxValue)
            {
                return false;
            }

            TimeSpan delay = player.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.CurrentTime);
            DateTimeOffset eventRequestTime = this.CurrentTime;

            // validate each tile of the suggested locations.
            for (int i = 0; i < directions.Length; i++)
            {
                var toLoc = fromLoc.LocationAt(directions[i]);

                if ((i > 0 && !this.map.GetTileAt(fromLoc, out _)) || !this.map.GetTileAt(toLoc, out ITile toTile))
                {
                    return false;
                }

                this.scheduler.ScheduleEvent(new OnMapCreatureMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, toLoc, deferEvaluation: true), eventRequestTime + delay);

                var movementPenalty = (toTile.Ground?.MovementPenalty ?? 200) * (directions[i].IsDiagonal() ? 2 : 1);

                // TODO: centralize walk penalty formula.
                delay = TimeSpan.FromMilliseconds(1000 * movementPenalty / (double)Math.Max(1, (int)player.Speed));
                eventRequestTime += delay;

                // update last location to this one.
                fromLoc = toLoc;
            }

            return true;
        }

        /// <summary>
        /// Attempts to cancel all of a player's pending movements.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool PlayerRequest_CancelPendingMovements(IPlayer player)
        {
            player.ThrowIfNull(nameof(player));

            this.scheduler.CancelAllFor(player.Id, typeof(OnMapCreatureMovementEvent));

            return true;
        }

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

            this.ConnectionManager.Register(connection, player.Id);

            // TODO: check if map.CanAddCreature(playerRecord.location);
            // playerRecord.location
            IThing playerThing = player;

            playerThing.Location = Game.HellsGate;

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
            player.ThrowIfNull(nameof(player));

            if (!this.map.GetTileAt(fromLocation, out ITile sourceTile))
            {
                return false;
            }

            var thingAtStackPos = sourceTile.GetTopThingByOrder(this.CreatureManager, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != thingId)
            {
                return false;
            }

            // At this point it seems like a valid movement, let's enqueue it.
            IEvent movementEvent = null;

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
                    toLocation,
                    fromStackPos);
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
                    toLocation,
                    fromStackPos);
            }

            if (movementEvent != null)
            {
                this.scheduler.ScheduleEvent(movementEvent, this.CurrentTime + DefaultPlayerActionDelay);
            }

            return movementEvent != null;
        }

        /// <summary>
        /// Attempts to use an item on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="itemClientId">The id of the item attempting to be used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        /// <returns>True if the use item request was accepted, false otherwise.</returns>
        public bool PlayerRequest_UseItem(IPlayer player, ushort itemClientId, Location fromLocation, byte fromStackPos, byte index)
        {
            player.ThrowIfNull(nameof(player));

            if (!this.map.GetTileAt(fromLocation, out ITile sourceTile))
            {
                return false;
            }

            var thingAtStackPos = sourceTile.GetTopThingByOrder(this.CreatureManager, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != itemClientId)
            {
                return false;
            }

            // At this point it seems like a valid usage request, let's enqueue it.
            IEvent useItemEvent = new UseItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    player.Id,
                    itemClientId,
                    fromLocation,
                    fromStackPos,
                    index);

            this.scheduler.ScheduleEvent(useItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
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

            if (!this.map.GetTileAt(fromLoc, out ITile fromTile))
            {
                return false;
            }

            var playerStackPosition = fromTile.GetStackPositionOfThing(player);

            if (playerStackPosition == byte.MaxValue)
            {
                return false;
            }

            var toLoc = fromLoc.LocationAt(direction);

            // Seems like a valid movement for now, enqueue it.
            var movementEvent = new OnMapCreatureMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, toLoc, playerStackPosition);
            var cooldownRemaining = player.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.CurrentTime);

            this.scheduler.ScheduleEvent(movementEvent, this.CurrentTime + cooldownRemaining);

            return true;
        }

        /// <summary>
        /// Inmediately attempts to perform a thing movement between two tiles.
        /// </summary>
        /// <param name="thing">The thing being moved.</param>
        /// <param name="fromTileLocation">The tile from which the movement is being performed.</param>
        /// <param name="toTileLocation">The tile to which the movement is being performed.</param>
        /// <param name="fromTileStackPos">Optional. The position in the stack of the tile from which the movement is being performed. Defaults to <see cref="byte.MaxValue"/> which signals to attempt to find the thing from the source location.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the move is considered a teleportation. Defaults to false.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformThingMovementBetweenTiles(IThing thing, Location fromTileLocation, Location toTileLocation, byte fromTileStackPos = byte.MaxValue, byte amountToMove = 1, bool isTeleport = false)
        {
            if (thing == null || !this.map.GetTileAt(fromTileLocation, out ITile fromTile) || !this.map.GetTileAt(toTileLocation, out ITile toTile))
            {
                return false;
            }

            if (fromTileStackPos == byte.MaxValue)
            {
                // try to figure it out on our own.
                fromTileStackPos = fromTile.GetStackPositionOfThing(thing);

                if (fromTileStackPos == byte.MaxValue)
                {
                    // couldn't find this thing...
                    return false;
                }
            }

            // Do the actual move first.
            if (!fromTile.RemoveThing(this.ItemFactory, ref thing, amountToMove))
            {
                return false;
            }

            toTile.AddThing(this.ItemFactory, ref thing, amountToMove);

            thing.Location = toTileLocation;

            var clientSafeMoveDirection = fromTileLocation.DirectionTo(toTileLocation);

            // Then deal with the consequences of the move.
            if (thing is ICreature creature)
            {
                creature.TurnToDirection(clientSafeMoveDirection);

                var trueMoveDirection = fromTileLocation.DirectionTo(toTileLocation, true);

                var tilePenalty = toTile.Ground?.MovementPenalty;
                var totalPenalty = (tilePenalty ?? 200) * (trueMoveDirection.IsDiagonal() ? 2 : 1);

                var exhaustionTime = TimeSpan.FromMilliseconds(1000 * totalPenalty / (double)Math.Max(1, (int)creature.Speed));

                creature.AddExhaustion(ExhaustionType.Movement, this.CurrentTime, exhaustionTime);

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

            return true;
        }

        /// <summary>
        /// Inmediately attempts to perform an item use in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="itemId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the use is happening.</param>
        /// <param name="fromStackPos">The position in the stack of the item at the location.</param>
        /// <param name="index">The index of the item to use.</param>
        /// <param name="requestor">Optional. The creature requesting the use.</param>
        /// <returns>True if the item was successfully used, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemUse(ushort itemId, Location fromLocation, byte fromStackPos, byte index, ICreature requestor = null)
        {
            if (fromLocation.Type != LocationType.Ground)
            {
                // not supported at the moment.
                return false;
            }

            // Using an item from the ground (map).
            if (!this.map.GetTileAt(fromLocation, out ITile fromTile))
            {
                return false;
            }

            var item = fromTile.FindItemWithId(itemId);

            if (item == null || item.ThingId != itemId)
            {
                return false;
            }

            var useItemEventRules = this.eventRulesCatalog[EventRuleType.Use].Cast<IUseItemEventRule>();

            // TODO: there is a potential problem here: multiple calls here will Setup different values if this is not thread safe.
            var rulesThatCanBeExecuted = useItemEventRules.Where(e => e.ItemToUseId == item.Type.TypeId && e.Setup(item, null, requestor as IPlayer) && e.CanBeExecuted);

            // Execute all actions.
            if (rulesThatCanBeExecuted.Any())
            {
                foreach (var rule in rulesThatCanBeExecuted)
                {
                    rule.Execute();
                }

                return true;
            }
            else if (item.ChangesOnUse)
            {
                var changeToType = item.ChangeOnUseTo;

                // change this item into the target.
                return this.PerformItemChange(itemId, changeToType, fromLocation, fromStackPos, index, requestor);
            }

            return false;
        }

        /// <summary>
        /// Inmediately attempts to perform an item change in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="fromTypeId">The type id of the item being changed.</param>
        /// <param name="toTypeId">The type id of the item being changed to.</param>
        /// <param name="fromLocation">The location from which the changed is happening.</param>
        /// <param name="fromStackPos">The position in the stack of the item at the location.</param>
        /// <param name="index">The index of the item to changed.</param>
        /// <param name="requestor">Optional. The creature requesting the use.</param>
        /// <returns>True if the item was successfully changed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemChange(ushort fromTypeId, ushort toTypeId, Location fromLocation, byte fromStackPos, byte index, ICreature requestor = null)
        {
            if (fromLocation.Type != LocationType.Ground)
            {
                // not supported at the moment.
                return false;
            }

            // Using an item from the ground (map).
            if (!this.map.GetTileAt(fromLocation, out ITile targetTile))
            {
                return false;
            }

            IThing potentialThing = targetTile.FindItemWithId(fromTypeId);

            if (potentialThing == null || !(potentialThing is IItem item) || item.ThingId != fromTypeId)
            {
                return false;
            }

            IThing newItemAsThing = this.ItemFactory.Create(toTypeId);

            if (newItemAsThing == null)
            {
                return false;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            if (!targetTile.RemoveThing(this.ItemFactory, ref potentialThing, item.Amount))
            {
                return false;
            }

            targetTile.AddThing(this.ItemFactory, ref newItemAsThing, item.Amount);

            newItemAsThing.Location = fromLocation;

            this.RequestNofitication(
                new TileUpdatedNotification(
                        this.Logger,
                        this.CreatureManager,
                        () => this.GetConnectionsOfPlayersThatCanSee(fromLocation),
                        new TileUpdatedNotificationArguments(
                            fromLocation,
                            this.GetDescriptionOfTile)));

            return true;
        }

        /// <summary>
        /// Inmediately attempts to perform an item creation in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="typeId">The id of the item being being created.</param>
        /// <param name="atLocation">The location at which the item is being created.</param>
        /// <param name="requestor">Optional. The creature requesting the creation.</param>
        /// <returns>True if the item was successfully created, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemCreation(ushort typeId, Location atLocation, ICreature requestor)
        {
            if (atLocation.Type != LocationType.Ground)
            {
                // not supported at the moment.
                return false;
            }

            // Using an item from the ground (map).
            if (!this.map.GetTileAt(atLocation, out ITile targetTile))
            {
                return false;
            }

            IThing newItemAsThing = this.ItemFactory.Create(typeId);

            if (newItemAsThing == null)
            {
                return false;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            targetTile.AddThing(this.ItemFactory, ref newItemAsThing);

            newItemAsThing.Location = atLocation;

            this.RequestNofitication(
                new TileUpdatedNotification(
                        this.Logger,
                        this.CreatureManager,
                        () => this.GetConnectionsOfPlayersThatCanSee(atLocation),
                        new TileUpdatedNotificationArguments(
                            atLocation,
                            this.GetDescriptionOfTile)));

            return true;
        }

        /// <summary>
        /// Inmediately attempts to perform an item deletion in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="typeId">The id of the item being being deleted.</param>
        /// <param name="atLocation">The location at which the item is being deleted.</param>
        /// <param name="requestor">Optional. The creature requesting the deletion.</param>
        /// <returns>True if the item was successfully deleted, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemDeletion(ushort typeId, Location atLocation, ICreature requestor)
        {
            if (atLocation.Type != LocationType.Ground)
            {
                // not supported at the moment.
                return false;
            }

            // Using an item from the ground (map).
            if (!this.map.GetTileAt(atLocation, out ITile targetTile))
            {
                return false;
            }

            IThing potentialThing = targetTile.FindItemWithId(typeId);

            if (potentialThing == null || !(potentialThing is IItem item) || item.ThingId != typeId)
            {
                return false;
            }

            // At this point, we have an item to remove, let's proceed.
            var successfulRemoval = targetTile.RemoveThing(this.ItemFactory, ref potentialThing, item.Amount);

            if (successfulRemoval)
            {
                this.RequestNofitication(
                    new TileUpdatedNotification(
                            this.Logger,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(atLocation),
                            new TileUpdatedNotificationArguments(
                                atLocation,
                                this.GetDescriptionOfTile)));
            }

            return successfulRemoval;
        }

        /// <summary>
        /// Evaluates separation event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="location">The location at which the events take place.</param>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        public bool EvaluateSeparationEventRules(Location location, IThing thingMoving, ICreature requestor)
        {
            if (this.map.GetTileAt(location, out ITile fromTile) && fromTile.HasSeparationEvents)
            {
                foreach (var item in fromTile.ItemsWithSeparation)
                {
                    var separationEvents = this.eventRulesCatalog[EventRuleType.Separation].Cast<ISeparationEventRule>();

                    // TODO: there is a potential problem here: multiple calls here will Setup different values if this is not thread safe.
                    var rulesThatCanBeExecuted = separationEvents.Where(e => e.ThingIdOfSeparation == item.Type.TypeId && e.Setup(item, thingMoving, requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    if (rulesThatCanBeExecuted.Any())
                    {
                        foreach (var rule in rulesThatCanBeExecuted)
                        {
                            rule.Execute();
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates collision event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="location">The location at which the events take place.</param>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        public bool EvaluateCollisionEventRules(Location location, IThing thingMoving, ICreature requestor)
        {
            if (this.map.GetTileAt(location, out ITile toTile) && toTile.HasCollisionEvents)
            {
                foreach (var item in toTile.ItemsWithCollision)
                {
                    var collisionEvents = this.eventRulesCatalog[EventRuleType.Collision].Cast<ICollisionEventRule>();

                    // TODO: there is a potential problem here: multiple calls here will Setup different values if this is not thread safe.
                    var rulesThatCanBeExecuted = collisionEvents.Where(e => e.ThingIdOfCollision == item.Type.TypeId && e.Setup(item, thingMoving, requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    if (rulesThatCanBeExecuted.Any())
                    {
                        foreach (var rule in rulesThatCanBeExecuted)
                        {
                            rule.Execute();
                        }

                        return true;
                    }
                }
            }

            return false;
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

            return this.map.DescribeForPlayer(player, startX, (ushort)(startX + windowSizeX), startY, (ushort)(startY + windowSizeY), startZ, endZ, startingZOffset);
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
                var connectionSweepperTask = Task.Factory.StartNew(this.ConnectionSweeper, cancellationToken, TaskCreationOptions.LongRunning);

                var miscellaneusEventsTask = Task.Factory.StartNew(this.MiscellaneousEventsLoop, cancellationToken, TaskCreationOptions.LongRunning);

                // start the scheduler.
                var schedulerTask = this.scheduler.RunAsync(cancellationToken);

                // Open the game world!
                this.Status = WorldState.Open;

                await Task.WhenAll(connectionSweepperTask, miscellaneusEventsTask, schedulerTask);
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
        /// Attempts to display an animated efect on the given location.
        /// </summary>
        /// <param name="location">The location at which to display the effect.</param>
        /// <param name="animatedEffect">The effect to display.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_AddAnimatedEffectAt(Location location, AnimatedEffect animatedEffect)
        {
            if (animatedEffect == AnimatedEffect.None)
            {
                return false;
            }

            this.scheduler.ImmediateEvent(
                new AnimatedEffectNotification(
                    this.Logger,
                    () => this.GetConnectionsOfPlayersThatCanSee(location),
                    new AnimatedEffectNotificationArguments(location, animatedEffect)));

            return true;
        }

        /// <summary>
        /// Attempts to change a given item to the supplied id.
        /// </summary>
        /// <param name="thing">The thing to change.</param>
        /// <param name="toTypeId">The id of the item type to change to.</param>
        /// <param name="animatedEffect">An optional effect to send as part of the change.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_ChangeItem(ref IThing thing, ushort toTypeId, AnimatedEffect animatedEffect)
        {
            if (thing == null)
            {
                return false;
            }

            if (!this.map.GetTileAt(thing.Location, out _))
            {
                return false;
            }

            // At this point it seems like a valid usage request, let's enqueue it.
            IEvent changeItemEvent = new ChangeItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    thing.ThingId,
                    thing.Location,
                    toTypeId);

            this.scheduler.ScheduleEvent(changeItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
        }

        /// <summary>
        /// Attempts to change a given item to the supplied id at a given location.
        /// </summary>
        /// <param name="location">The location at which the change will happen.</param>
        /// <param name="fromTypeId">The id of the item from which the change is happening.</param>
        /// <param name="toTypeId">The id of the item to which the change is happening.</param>
        /// <param name="animatedEffect">An optional effect to send as part of the change.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_ChangeItemAt(Location location, ushort fromTypeId, ushort toTypeId, AnimatedEffect animatedEffect)
        {
            IEvent changeItemEvent = new ChangeItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    fromTypeId,
                    location,
                    toTypeId);

            this.scheduler.ScheduleEvent(changeItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
        }

        /// <summary>
        /// Attempts to create an item at a given location.
        /// </summary>
        /// <param name="location">The location at which to create the item.</param>
        /// <param name="itemType">The type of the item to create.</param>
        /// <param name="effect">An effect to use when the creation takes place.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_CreateItemAt(Location location, ushort itemType, byte effect)
        {
            IEvent createItemEvent = new CreateItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    itemType,
                    location);

            this.scheduler.ScheduleEvent(createItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
        }

        /// <summary>
        /// Attempts to delete an item.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_DeleteItem(IItem item)
        {
            if (item == null)
            {
                return false;
            }

            IEvent deleteItemEvent = new DeleteItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    item.ThingId,
                    item.Location);

            this.scheduler.ScheduleEvent(deleteItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
        }

        /// <summary>
        /// Attempts to delete an item at a given location.
        /// </summary>
        /// <param name="location">The location at which to delete the item.</param>
        /// <param name="itemType">The type of the item to delete.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_DeleteItemAt(Location location, ushort itemType)
        {
            IEvent deleteItemEvent = new DeleteItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    itemType,
                    location);

            this.scheduler.ScheduleEvent(deleteItemEvent, this.CurrentTime + DefaultPlayerActionDelay);

            return true;
        }

        /// <summary>
        /// Attempts to move a creature to a given location.
        /// </summary>
        /// <param name="creature">The creature to move.</param>
        /// <param name="targetLocation">The location to move the creature to.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_MoveCreature(ICreature creature, Location targetLocation)
        {
            creature.ThrowIfNull(nameof(creature));

            if (!this.map.GetTileAt(creature.Location, out ITile fromTile) || !this.map.GetTileAt(targetLocation, out _))
            {
                return false;
            }

            var creatureStackPosition = fromTile.GetStackPositionOfThing(creature);

            this.scheduler.ScheduleEvent(
                new OnMapCreatureMovementEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    creature,
                    creature.Location,
                    targetLocation,
                    creatureStackPosition),
                this.CurrentTime + DefaultDelayForScripts);

            return true;
        }

        /// <summary>
        /// Attempts to move all items and creatures in a location to a given location.
        /// </summary>
        /// <param name="fromLocation">The location from which to move everything.</param>
        /// <param name="targetLocation">The location to move everything to.</param>
        /// <param name="exceptTypeIds">Optional. Any type ids to explicitly exclude.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_MoveEverythingTo(Location fromLocation, Location targetLocation, params ushort[] exceptTypeIds)
        {
            if (!this.map.GetTileAt(fromLocation, out ITile fromTile) || !this.map.GetTileAt(targetLocation, out _))
            {
                return false;
            }

            foreach (var item in fromTile.Items)
            {
                if (exceptTypeIds.Length > 0 && exceptTypeIds.Contains(item.ThingId))
                {
                    continue;
                }

                var fromStackPos = fromTile.GetStackPositionOfThing(item);

                this.scheduler.ScheduleEvent(
                    new OnMapMovementEvent(
                        this.Logger,
                        this,
                        this.ConnectionManager,
                        this.map,
                        this.CreatureManager,
                        0,
                        item,
                        fromLocation,
                        targetLocation,
                        fromStackPos),
                    this.CurrentTime + DefaultDelayForScripts);
            }

            foreach (var creatureId in fromTile.CreatureIds)
            {
                var creature = this.CreatureManager.FindCreatureById(creatureId);

                if (creature == null)
                {
                    continue;
                }

                var creatureStackPosition = fromTile.GetStackPositionOfThing(creature);

                this.scheduler.ScheduleEvent(
                    new OnMapCreatureMovementEvent(
                        this.Logger,
                        this,
                        this.ConnectionManager,
                        this.map,
                        this.CreatureManager,
                        0,
                        creature,
                        fromLocation,
                        targetLocation,
                        creatureStackPosition),
                    this.CurrentTime + DefaultDelayForScripts);
            }

            return true;
        }

        /// <summary>
        /// Attempts to move an item to a given location.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="targetLocation">The location to move the item to.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_MoveItemTo(IItem item, Location targetLocation)
        {
            item.ThrowIfNull(nameof(item));

            if (!this.map.GetTileAt(item.Location, out ITile fromTile) || !this.map.GetTileAt(targetLocation, out _))
            {
                return false;
            }

            var itemStackPosition = fromTile.GetStackPositionOfThing(item);

            this.scheduler.ScheduleEvent(
                new OnMapMovementEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    item,
                    item.Location,
                    targetLocation,
                    itemStackPosition),
                this.CurrentTime + DefaultDelayForScripts);

            return true;
        }

        /// <summary>
        /// Attempts to move an item of the given type from the given location to another location.
        /// </summary>
        /// <param name="itemType">The type of the item to move.</param>
        /// <param name="fromLocation">The location from which to move the item.</param>
        /// <param name="toLocation">The location to which to move the item.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_MoveItemOfTypeTo(ushort itemType, Location fromLocation, Location toLocation)
        {
            if (!this.map.GetTileAt(fromLocation, out ITile fromTile) || !this.map.GetTileAt(toLocation, out _))
            {
                return false;
            }

            var item = fromTile.FindItemWithId(itemType);

            if (item == null)
            {
                return false;
            }

            var itemStackPosition = fromTile.GetStackPositionOfThing(item);

            this.scheduler.ScheduleEvent(
                new OnMapMovementEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    0,
                    item,
                    item.Location,
                    toLocation,
                    itemStackPosition),
                this.CurrentTime + DefaultDelayForScripts);

            return true;
        }

        /// <summary>
        /// Attempts to log out a player.
        /// </summary>
        /// <param name="player">The player to log out.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_Logout(IPlayer player)
        {
            // TODO: should this one be different pipeline?
            return this.PlayerRequest_Logout(player);
        }

        /// <summary>
        /// Attempts to place a new monster at the given location.
        /// </summary>
        /// <param name="location">The location at which to place the monster.</param>
        /// <param name="monsterType">The type of the monster to place.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_PlaceMonsterAt(Location location, ushort monsterType)
        {
            throw new NotImplementedException();
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
                Thread.Sleep(TimeSpan.FromMinutes(1));

                const int NightLightLevel = 30;
                const int DuskDawnLightLevel = 130;
                const int DayLightLevel = 255;

                // A day is roughly an hour in real time, and night lasts roughly 1/3 of the day in real time
                // Dusk and Dawns last for 30 minutes roughly, so les aproximate that to 2 minutes.
                var currentMinute = this.CurrentTime.Minute;

                var currentLevel = this.WorldLightLevel;
                var currentColor = this.WorldLightColor;

                if (currentMinute >= 0 && currentMinute <= 37)
                {
                    // Day time: [0, 37] minutes on the hour.
                    this.WorldLightLevel = DayLightLevel;
                    this.WorldLightColor = (byte)LightColors.White;
                }
                else if (currentMinute == 38 || currentMinute == 39 || currentMinute == 58 || currentMinute == 59)
                {
                    // Dusk: [38, 39] minutes on the hour.
                    // Dawn: [58, 59] minutes on the hour.
                    this.WorldLightLevel = DuskDawnLightLevel;
                    this.WorldLightColor = (byte)LightColors.Orange;
                }
                else if (currentMinute >= 40 && currentMinute <= 57)
                {
                    // Night time: [40, 57] minutes on the hour.
                    this.WorldLightLevel = NightLightLevel;
                    this.WorldLightColor = (byte)LightColors.White;
                }

                if (this.WorldLightLevel != currentLevel || this.WorldLightColor != currentColor)
                {
                    this.scheduler.ImmediateEvent(
                        new WorldLightChangedNotification(
                            this.Logger,
                            () => this.ConnectionManager.GetAllActive(),
                            new WorldLightChangedNotificationArguments(this.WorldLightLevel, this.WorldLightColor)));
                }
            }
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

                    // player.SetAttackTarget(0);

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
        private void ProcessEvent(object sender, EventFiredEventArgs eventArgs)
        {
            if (sender != this.scheduler || eventArgs?.Event == null)
            {
                return;
            }

            IEvent evt = eventArgs.Event;

            try
            {
                evt.Process();
                this.Logger.Debug($"Processed event {evt.EventId}, current game time: {this.CurrentTime.ToUnixTimeMilliseconds()}.");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Error in event {evt.EventId}: {ex.Message}.");
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
