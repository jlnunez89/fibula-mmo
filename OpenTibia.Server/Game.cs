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
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Entities;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
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
        private const int NoCreatureId = 0;

        private const int DefaultGroundMovementPenalty = 200;

        public static Location NewbieStart = new Location { X = 32097, Y = 32219, Z = 7 };

        public static Location VeteranStart = new Location { X = 32369, Y = 32241, Z = 7 };

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
        /// Gets the monster spawns in the game.
        /// </summary>
        private readonly IEnumerable<Spawn> monsterSpawns;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="map">A reference to the map to use.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="eventRulesLoader">A reference to the event rules loader.</param>
        /// <param name="monsterSpawnsLoader">A reference to the monster spawns loader.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="scriptApi">A reference to the script api in use.</param>
        public Game(
            ILogger logger,
            IMap map,
            IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            IEventRulesLoader eventRulesLoader,
            IMonsterSpawnLoader monsterSpawnsLoader,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IScriptApi scriptApi)
        {
            logger.ThrowIfNull(nameof(logger));
            map.ThrowIfNull(nameof(map));
            connectionManager.ThrowIfNull(nameof(connectionManager));
            creatureManager.ThrowIfNull(nameof(creatureManager));
            eventRulesLoader.ThrowIfNull(nameof(eventRulesLoader));
            monsterSpawnsLoader.ThrowIfNull(nameof(monsterSpawnsLoader));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            scriptApi.ThrowIfNull(nameof(scriptApi));

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

            this.monsterSpawns = monsterSpawnsLoader.LoadSpawns();

            // Initialize game vars.
            this.Status = WorldState.Loading;
            this.WorldLightColor = (byte)LightColors.White;
            this.WorldLightLevel = (byte)LightLevels.World;

            this.scheduler.OnEventFired += this.ProcessEvent;

            this.map.WindowLoaded += this.HandleMapWindowLoaded;
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

            DateTimeOffset eventRequestTime = this.CurrentTime + player.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.CurrentTime);

            // validate each tile of the suggested locations.
            for (int i = 0; i < directions.Length; i++)
            {
                var toLoc = fromLoc.LocationAt(directions[i]);

                if ((i > 0 && !this.map.GetTileAt(fromLoc, out fromTile)) || !this.map.GetTileAt(toLoc, out _))
                {
                    return false;
                }

                this.scheduler.ScheduleEvent(new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, toLoc, evaluationTime: EvaluationTime.OnExecute), eventRequestTime);

                eventRequestTime += this.CalculateStepDuration(player, directions[i], fromTile);

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

            this.scheduler.CancelAllFor(player.Id, typeof(BaseMovementEvent));

            return true;
        }

        /// <summary>
        /// Attempts to close a player's container.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="containerId">The id of the container to close.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool PlayerRequest_CloseContainer(IPlayer player, byte containerId)
        {
            player.ThrowIfNull(nameof(player));

            var item = player.GetContainerById(containerId);

            if (item == null)
            {
                return false;
            }

            this.PerformPlayerContainerClose(player, item, containerId);

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
            // TODO: should be something like character.location
            var targetLocation = Game.VeteranStart;

            var player = this.CreatureFactory.Create(CreatureType.Player, new PlayerCreationMetadata(character.Id, character.Name, 100, 100, 100, 100, 4240)) as Player;

            if (!this.PlaceCreatureOnMap(targetLocation, player))
            {
                return null;
            }

            this.ConnectionManager.Register(connection, player.Id);

            player.Inventory.OnSlotChanged += this.HandlePlayerInventoryChanged;

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

            (bool removeSuccessful, IThing remainder) = tile.RemoveContent(this.ItemFactory, ref playerThing);

            if (removeSuccessful)
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

                player.Inventory.OnSlotChanged -= this.HandlePlayerInventoryChanged;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to move a thing from the map on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="fromLocation">The location from which the thing is being moved.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="count">The amount of the thing that is being moved.</param>
        /// <returns>True if the thing movement was accepted, false otherwise.</returns>
        public bool PlayerRequest_MoveThingFromMap(IPlayer player, ushort thingId, Location fromLocation, byte fromStackPos, Location toLocation, byte count)
        {
            player.ThrowIfNull(nameof(player));

            if (fromLocation.Type != LocationType.Map)
            {
                return false;
            }

            if (!this.map.GetTileAt(fromLocation, out ITile sourceTile))
            {
                return false;
            }

            var thingAtStackPos = sourceTile.GetTopThingByOrder(this.CreatureManager, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != thingId)
            {
                return false;
            }

            IEvent movementEvent = null;
            var movementDelay = DefaultPlayerActionDelay;

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    if (thingAtStackPos is ICreature)
                    {
                        // Override delay moving a creature in the map.
                        movementDelay = TimeSpan.FromSeconds(1);
                    }

                    movementEvent = new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingAtStackPos, fromLocation, toLocation, fromStackPos, count);
                    break;
                case LocationType.InsideContainer:
                    movementEvent = new MapToContainerMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingAtStackPos, fromLocation, player.Id, toLocation.ContainerId, toLocation.ContainerIndex, count);
                    break;
                case LocationType.InventorySlot:
                    movementEvent = new MapToBodyMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingAtStackPos, fromLocation, player.Id, toLocation.Slot, count);
                    break;
            }

            // At this point we proabbly have enough information, let's enqueue it.
            if (movementEvent != null)
            {
                this.scheduler.ScheduleEvent(movementEvent, this.CurrentTime + movementDelay);
            }

            return movementEvent != null;
        }

        /// <summary>
        /// Attempts to move a thing from a specific container on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="containerId">The id of the container from which the thing is being moved.</param>
        /// <param name="containerIndex">The index within the container from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="amount">The amount of the thing that is being moved.</param>
        /// <returns>True if the thing movement was accepted, false otherwise.</returns>
        public bool PlayerRequest_MoveThingFromContainer(IPlayer player, ushort thingId, byte containerId, byte containerIndex, Location toLocation, byte amount)
        {
            player.ThrowIfNull(nameof(player));

            var sourceContainer = player.GetContainerById(containerId);

            var thingMoving = sourceContainer?[containerIndex];

            if (thingMoving == null || thingMoving.ThingId != thingId)
            {
                return false;
            }

            IEvent movementEvent = null;

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    movementEvent = new ContainerToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, containerId, containerIndex, toLocation, amount);
                    break;
                case LocationType.InsideContainer:
                    movementEvent = new ContainerToContainerMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, containerId, containerIndex, toLocation.ContainerId, toLocation.ContainerIndex, amount);
                    break;
                case LocationType.InventorySlot:
                    movementEvent = new ContainerToBodyMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, containerId, containerIndex, toLocation.Slot, amount);
                    break;
            }

            // At this point we proabbly have enough information, let's enqueue it.
            if (movementEvent != null)
            {
                this.scheduler.ScheduleEvent(movementEvent, this.CurrentTime + DefaultPlayerActionDelay);
            }

            return movementEvent != null;
        }

        /// <summary>
        /// Attempts to move a thing from an inventory slot on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="slot">The inventory slot from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="amount">The amount of the thing that is being moved.</param>
        /// <returns>True if the thing movement was accepted, false otherwise.</returns>
        public bool PlayerRequest_MoveThingFromInventory(IPlayer player, ushort thingId, Slot slot, Location toLocation, byte amount)
        {
            player.ThrowIfNull(nameof(player));

            var sourceContainer = player.Inventory[slot] as IContainerItem;

            var thingMoving = sourceContainer?.Content.FirstOrDefault();

            if (thingMoving == null || thingMoving.ThingId != thingId)
            {
                return false;
            }

            IEvent movementEvent = null;

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    movementEvent = new BodyToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, slot, toLocation, amount: amount);
                    break;
                case LocationType.InsideContainer:
                    movementEvent = new BodyToContainerMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, slot, toLocation.ContainerId, toLocation.ContainerIndex, amount);
                    break;
                case LocationType.InventorySlot:
                    movementEvent = new BodyToBodyMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, thingMoving, player.Id, slot, toLocation.Slot, amount);
                    break;
            }

            // At this point we proabbly have enough information, let's enqueue it.
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
            var movementEvent = new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, player.Id, player, fromLoc, toLoc, playerStackPosition);
            var cooldownRemaining = player.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.CurrentTime);

            this.scheduler.ScheduleEvent(movementEvent, this.CurrentTime + cooldownRemaining);

            return true;
        }

        /// <summary>
        /// Immediately attempts to perform a creature movement to a tile on the map.
        /// </summary>
        /// <param name="creature">The creature being moved.</param>
        /// <param name="toLocation">The tile to which the movement is being performed.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the movement is considered a teleportation. Defaults to false.</param>
        /// <param name="requestorCreature">Optional. The creature that this movement is being performed in behalf of, if any.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformCreatureMovement(ICreature creature, Location toLocation, bool isTeleport = false, ICreature requestorCreature = null)
        {
            if (creature == null || !(creature.ParentCylinder is ITile fromTile) || !this.map.GetTileAt(toLocation, out ITile toTile))
            {
                return false;
            }

            var moveDirection = fromTile.Location.DirectionTo(toLocation, true);

            // Try to figure out the position in the stack of the creature.
            var fromTileStackPos = fromTile.GetStackPositionOfThing(creature);

            if (fromTileStackPos == byte.MaxValue)
            {
                // couldn't find this creature in this tile...
                return false;
            }

            IThing creatureAsThing = creature as IThing;

            // Do the actual move first.
            (bool removeSuccessful, IThing removeRemainder) = fromTile.RemoveContent(this.ItemFactory, ref creatureAsThing);

            if (!removeSuccessful)
            {
                return false;
            }

            (bool addSuccessful, IThing addRemainder) = toTile.AddContent(this.ItemFactory, creature);

            if (!addSuccessful)
            {
                // attempt to rollback state.
                (bool rollbackSuccessful, IThing rollbackRemainder) = fromTile.RemoveContent(this.ItemFactory, ref creatureAsThing);

                if (!rollbackSuccessful)
                {
                    // Leaves us in a really bad spot.
                    throw new ApplicationException("Unable to rollback state after filing to move creature. Game state is altered and inconsistent now.");
                }
            }

            var toStackPosition = toTile.GetStackPositionOfThing(creature);

            // Then deal with the consequences of the move.
            creature.TurnToDirection(moveDirection.GetClientSafeDirection());

            var stepDurationTime = this.CalculateStepDuration(creature, moveDirection, fromTile);

            creature.AddExhaustion(ExhaustionType.Movement, this.CurrentTime, stepDurationTime);

            if (toStackPosition != byte.MaxValue)
            {
                this.RequestNofitication(
                    new CreatureMovedNotification(
                        this.Logger,
                        this,
                        this.CreatureManager,
                        () => this.GetConnectionsOfPlayersThatCanSee(fromTile.Location, toLocation),
                        new CreatureMovedNotificationArguments(creature.Id, fromTile.Location, fromTileStackPos, toTile.Location, toStackPosition, isTeleport)));
            }

            if (creature is IPlayer player)
            {
                // If the creature is a player, we must check if the movement caused it to walk away from any open containers.
                foreach (var container in player.OpenContainers)
                {
                    if (container == null)
                    {
                        continue;
                    }

                    var locationDiff = container.Location - player.Location;

                    if ((locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0) && container.IsTracking(player.Id, out byte containerId))
                    {
                        this.PerformPlayerContainerClose(player, container, containerId);
                    }
                }
            }

            this.EvaluateSeparationEventRules(fromTile.Location, creature, requestorCreature);
            this.EvaluateCollisionEventRules(toTile.Location, creature, requestorCreature);

            return true;
        }

        /// <summary>
        /// Immediately attempts to perform an item movement between two cylinders.
        /// </summary>
        /// <param name="item">The item being moved.</param>
        /// <param name="fromCylinder">The cylinder from which the movement is being performed.</param>
        /// <param name="toCylinder">The cylinder to which the movement is being performed.</param>
        /// <param name="fromIndex">Optional. The index within the cylinder to move the item from.</param>
        /// <param name="toIndex">Optional. The index within the cylinder to move the item to.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="requestorCreature">Optional. The creature that this movement is being performed in behalf of, if any.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemMovement(IItem item, ICylinder fromCylinder, ICylinder toCylinder, byte fromIndex = 0xFF, byte toIndex = 0xFF, byte amountToMove = 1, ICreature requestorCreature = null)
        {
            const byte FallbackIndex = 0xFF;

            if (item == null || fromCylinder == null || toCylinder == null)
            {
                return false;
            }

            var sameCylinder = fromCylinder == toCylinder;

            if (sameCylinder && fromIndex == toIndex)
            {
                // no change at all.
                return true;
            }

            // Edge case, check if the moving item is the target container.
            if (item is IContainerItem containerItem && toCylinder is IContainerItem targetContainer && targetContainer.IsChildOf(containerItem))
            {
                return false;
            }

            IThing itemAsThing = item as IThing;

            (bool removeSuccessful, IThing removeRemainder) = fromCylinder.RemoveContent(this.ItemFactory, ref itemAsThing, fromIndex, amount: amountToMove);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return false;
            }

            if (fromCylinder is ITile fromTile)
            {
                this.RequestNofitication(
                    new TileUpdatedNotification(
                        this.Logger,
                        this.CreatureManager,
                        () => this.GetConnectionsOfPlayersThatCanSee(fromTile.Location),
                        new TileUpdatedNotificationArguments(fromTile.Location, this.GetDescriptionOfTile)));
            }

            this.EvaluateSeparationEventRules(fromCylinder.Location, item, requestorCreature);

            IThing addRemainder = itemAsThing;

            if (sameCylinder && removeRemainder == null && fromIndex < toIndex)
            {
                // If the move happens within the same cylinder, we need to adjust the index of where we're adding, depending if it is before or after.
                toIndex--;
            }

            if (!this.AddContentToCylinderChain(toCylinder.GetCylinderHierarchy(includeTiles: false), toIndex, ref addRemainder, requestorCreature) || addRemainder != null)
            {
                // There is some rollback to do, as we failed to add the entire thing.
                IThing rollbackRemainder = addRemainder ?? item;

                if (!this.AddContentToCylinderChain(fromCylinder.GetCylinderHierarchy(), FallbackIndex, ref rollbackRemainder, requestorCreature))
                {
                    this.Logger.Error($"Rollback failed on {nameof(this.PerformItemMovement)}. Thing: {rollbackRemainder.DescribeForLogger()}");
                }
            }

            return true;
        }

        /// <summary>
        /// Immediately attempts to perform an item use in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being used.</param>
        /// <param name="fromCylinder">The cylinder from which the use is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to use the item.</param>
        /// <param name="requestor">Optional. The creature requesting the use.</param>
        /// <returns>True if the item was successfully used, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemUse(IItem item, ICylinder fromCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            if (item == null || fromCylinder == null)
            {
                return false;
            }

            var useItemEventRules = this.eventRulesCatalog[EventRuleType.Use].Cast<IUseItemEventRule>();

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
                // change this item into the target.
                return this.PerformItemChange(item, item.ChangeOnUseTo, fromCylinder, index, requestor);
            }
            else if (item is IContainerItem container && requestor is IPlayer player)
            {
                var containerId = player.GetContainerId(container);

                if (containerId < 0)
                {
                    // Player doesn't have this container open, so open.
                    this.PerformPlayerContainerOpen(player, container, index);
                }
                else
                {
                    // Close the container for this player.
                    this.PerformPlayerContainerClose(player, container, (byte)containerId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Immediately attempts to perform an item change in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being changed.</param>
        /// <param name="toTypeId">The type id of the item being changed to.</param>
        /// <param name="atCylinder">The cylinder at which the change is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to change the item.</param>
        /// <param name="requestor">Optional. The creature requesting the change.</param>
        /// <returns>True if the item was successfully changed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemChange(IItem item, ushort toTypeId, ICylinder atCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            const byte FallbackIndex = 0xFF;

            if (item == null || atCylinder == null)
            {
                return false;
            }

            IThing newItem = this.ItemFactory.Create(toTypeId);

            if (newItem == null)
            {
                return false;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = atCylinder.ReplaceContent(this.ItemFactory, item, newItem, index, item.Amount);

            if (!replaceSuccessful || replaceRemainder != null)
            {
                this.AddContentToCylinderChain(atCylinder.GetCylinderHierarchy(), FallbackIndex, ref replaceRemainder, requestor);
            }

            if (replaceSuccessful)
            {
                if (atCylinder is ITile atTile)
                {
                    this.RequestNofitication(
                        new TileUpdatedNotification(
                            this.Logger,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(atTile.Location),
                            new TileUpdatedNotificationArguments(atTile.Location, this.GetDescriptionOfTile)));
                }

                this.EvaluateCollisionEventRules(atCylinder.Location, item, requestor);
                this.EvaluateMovementEventRules(item, requestor);
            }

            return true;
        }

        /// <summary>
        /// Immediately attempts to perform an item creation in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="typeId">The id of the item being being created.</param>
        /// <param name="atCylinder">The cylinder from which the use is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to use the item.</param>
        /// <param name="requestor">Optional. The creature requesting the creation.</param>
        /// <returns>True if the item was successfully created, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>>
        public bool PerformItemCreation(ushort typeId, ICylinder atCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            if (atCylinder == null)
            {
                return false;
            }

            IThing newItem = this.ItemFactory.Create(typeId);

            if (newItem == null)
            {
                return false;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            return this.AddContentToCylinderChain(atCylinder.GetCylinderHierarchy(), index, ref newItem, requestor);
        }

        /// <summary>
        /// Immediately attempts to perform an item deletion in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being deleted.</param>
        /// <param name="fromCylinder">The cylinder from which the deletion is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to delete the item.</param>
        /// <param name="requestor">Optional. The creature requesting the deletion.</param>
        /// <returns>True if the item was successfully deleted, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        public bool PerformItemDeletion(IItem item, ICylinder fromCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            if (item == null || fromCylinder == null)
            {
                return false;
            }

            IThing itemAsThing = item as IThing;

            // At this point, we have an item to remove, let's proceed.
            (bool removeSuccessful, IThing remainder) = fromCylinder.RemoveContent(this.ItemFactory, ref itemAsThing, index, amount: item.Amount);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return false;
            }

            if (fromCylinder is ITile fromTile)
            {
                this.RequestNofitication(
                    new TileUpdatedNotification(
                            this.Logger,
                            this.CreatureManager,
                            () => this.GetConnectionsOfPlayersThatCanSee(fromTile.Location),
                            new TileUpdatedNotificationArguments(
                                fromTile.Location,
                                this.GetDescriptionOfTile)));
            }

            this.EvaluateSeparationEventRules(fromCylinder.Location, item, requestor);

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

            if (location.Type != LocationType.Map)
            {
                throw new ArgumentException($"Invalid location {location}.", nameof(location));
            }

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
            // TODO: probably save game state here.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Requests a notification be scheduled to be sent immediately.
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
        public bool ScriptRequest_ChangeItem(IThing thing, ushort toTypeId, AnimatedEffect animatedEffect)
        {
            if (thing == null)
            {
                return false;
            }

            IEvent changeItemEvent = new ChangeItemEvent(
                    this.Logger,
                    this,
                    this.ConnectionManager,
                    this.map,
                    this.CreatureManager,
                    NoCreatureId,
                    thing.ThingId,
                    thing.CarryLocation ?? thing.Location,
                    toTypeId,
                    carrierCreature: (thing is IItem item) ? item.Carrier : null);

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
                    NoCreatureId,
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
                    NoCreatureId,
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
                    NoCreatureId,
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
                    NoCreatureId,
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
                new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, NoCreatureId, creature, creature.Location, targetLocation, creatureStackPosition),
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
                    new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, NoCreatureId, item, fromLocation, targetLocation, fromStackPos, amount: item.Amount),
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
                    new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, NoCreatureId, creature, fromLocation, targetLocation, creatureStackPosition),
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
                new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, NoCreatureId, item, item.Location, targetLocation, itemStackPosition, amount: item.Amount),
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
                new MapToMapMovementEvent(this.Logger, this, this.ConnectionManager, this.map, this.CreatureManager, NoCreatureId, item, item.Location, toLocation, itemStackPosition, amount: item.Amount),
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
        /// <param name="monsterRace">The race of the monster to place.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        public bool ScriptRequest_PlaceMonsterAt(Location location, ushort monsterRace)
        {
            var newMonster = this.CreatureFactory.Create(CreatureType.Monster, new MonsterCreationMetadata(monsterRace));

            return this.PlaceCreatureOnMap(location, newMonster);
        }

        /// <summary>
        /// Gets a cylinder from a location.
        /// </summary>
        /// <param name="fromLocation">The location from which to decode the cylinder information.</param>
        /// <param name="index">The index within the cyclinder to target.</param>
        /// <param name="subIndex">The sub-index within the cylinder to target.</param>
        /// <param name="creature">Optional. The creature that owns the target cylinder to target.</param>
        /// <returns>An instance of the target <see cref="ICylinder"/> of the location.</returns>
        public ICylinder GetCyclinder(Location fromLocation, ref byte index, ref byte subIndex, ICreature creature = null)
        {
            if (fromLocation.Type == LocationType.Map && this.map.GetTileAt(fromLocation, out ITile fromTile))
            {
                return fromTile;
            }
            else if (fromLocation.Type == LocationType.InsideContainer)
            {
                index = fromLocation.ContainerId;
                subIndex = fromLocation.ContainerIndex;

                return creature?.GetContainerById(fromLocation.ContainerId);
            }
            else if (fromLocation.Type == LocationType.InventorySlot)
            {
                index = 0;
                subIndex = 0;

                return creature?.Inventory[fromLocation.Slot] as IContainerItem;
            }

            return null;
        }

        /// <summary>
        /// Attempts to find an item at the given location.
        /// </summary>
        /// <param name="typeId">The type id of the item to look for.</param>
        /// <param name="location">The location at which to look for the item.</param>
        /// <param name="creature">Optional. The creature that the location's cyclinder targets, if any.</param>
        /// <returns>An item instance, if found at the location.</returns>
        public IItem FindItemByIdAtLocation(ushort typeId, Location location, ICreature creature = null)
        {
            switch (location.Type)
            {
                case LocationType.Map:
                    // Using an item from the ground (map).
                    if (!this.map.GetTileAt(location, out ITile tile))
                    {
                        return null;
                    }

                    return tile.FindItemWithId(typeId);
                case LocationType.InventorySlot:
                    var fromBodyContainer = creature?.Inventory[location.Slot] as IContainerItem;

                    return fromBodyContainer?.Content.FirstOrDefault();
                case LocationType.InsideContainer:
                    var fromContainer = creature.GetContainerById(location.ContainerId);

                    return fromContainer?[location.ContainerIndex];
            }

            return null;
        }

        /// <summary>
        /// Checks if a throw between two map locations is valid.
        /// </summary>
        /// <param name="fromLocation">The first location.</param>
        /// <param name="toLocation">The second location.</param>
        /// <param name="checkLineOfSight">Optional. A value indicating whether to consider line of sight.</param>
        /// <returns>True if the throw is valid, false otherwise.</returns>
        public bool CanThrowBetweenMapLocations(Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            if (fromLocation.Type != LocationType.Map || toLocation.Type != LocationType.Map)
            {
                return false;
            }

            if (fromLocation == toLocation)
            {
                return true;
            }

            // Cannot throw across the surface boundary (floor 7).
            if ((fromLocation.Z >= 8 && toLocation.Z <= 7) || (toLocation.Z >= 8 && fromLocation.Z <= 7))
            {
                return false;
            }

            var deltaX = Math.Abs(fromLocation.X - toLocation.X);
            var deltaY = Math.Abs(fromLocation.Y - toLocation.Y);
            var deltaZ = Math.Abs(fromLocation.Z - toLocation.Z);

            // distance checks
            if (deltaX - deltaZ >= (IMap.DefaultWindowSizeX / 2) || deltaY - deltaZ >= (IMap.DefaultWindowSizeY / 2))
            {
                return false;
            }

            return !checkLineOfSight || this.InLineOfSight(fromLocation, toLocation);
        }

        /// <summary>
        /// Checks if a map location is in the line of sight of another map location.
        /// </summary>
        /// <param name="firstLocation">The first location.</param>
        /// <param name="secondLocation">The second location.</param>
        /// <returns>True if the second location is considered within the line of sight of the first location, false otherwise.</returns>
        private bool InLineOfSight(Location firstLocation, Location secondLocation)
        {
            if (firstLocation.Type != LocationType.Map || secondLocation.Type != LocationType.Map)
            {
                return false;
            }

            if (firstLocation == secondLocation)
            {
                return true;
            }

            // Normalize so that the check always happens from 'high to low' floors.
            var origin = firstLocation.Z > secondLocation.Z ? secondLocation : firstLocation;
            var target = firstLocation.Z > secondLocation.Z ? firstLocation : secondLocation;

            // Define positive or negative steps, depending on where the target location is wrt the origin location.
            var stepX = (sbyte)(origin.X < target.X ? 1 : origin.X == target.X ? 0 : -1);
            var stepY = (sbyte)(origin.Y < target.Y ? 1 : origin.Y == target.Y ? 0 : -1);

            var a = target.Y - origin.Y;
            var b = origin.X - target.X;
            var c = -((a * target.X) + (b * target.Y));

            while ((origin - target).MaxValueIn2D != 0)
            {
                var moveHorizontal = Math.Abs((a * (origin.X + stepX)) + (b * origin.Y) + c);
                var moveVertical = Math.Abs((a * origin.X) + (b * (origin.Y + stepY)) + c);
                var moveCross = Math.Abs((a * (origin.X + stepX)) + (b * (origin.Y + stepY)) + c);

                if (origin.Y != target.Y && (origin.X == target.X || moveHorizontal > moveVertical || moveHorizontal > moveCross))
                {
                    origin.Y += stepY;
                }

                if (origin.X != target.X && (origin.Y == target.Y || moveVertical > moveHorizontal || moveVertical > moveCross))
                {
                    origin.X += stepX;
                }

                if (!this.map.GetTileAt(origin, out ITile tile) || tile.BlocksThrow)
                {
                    return false;
                }
            }

            while (origin.Z != target.Z)
            {
                // now we need to perform a jump between floors to see if everything is clear (literally)
                if (this.map.GetTileAt(origin, out ITile tile) && tile.Ground != null)
                {
                    return false;
                }

                origin.Z++;
            }

            return true;
        }

        /// <summary>
        /// Evaluates separation event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="location">The location at which the events take place.</param>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        /// <remarks>This operation is not thread-safe.</remarks>
        private bool EvaluateSeparationEventRules(Location location, IThing thingMoving, ICreature requestor)
        {
            if (this.map.GetTileAt(location, out ITile fromTile) && fromTile.HasSeparationEvents)
            {
                foreach (var item in fromTile.ItemsWithSeparation)
                {
                    var separationEvents = this.eventRulesCatalog[EventRuleType.Separation].Cast<ISeparationEventRule>();

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
        /// <remarks>This operation is not thread-safe.</remarks>
        private bool EvaluateCollisionEventRules(Location location, IThing thingMoving, ICreature requestor)
        {
            if (this.map.GetTileAt(location, out ITile toTile) && toTile.HasCollisionEvents)
            {
                foreach (var item in toTile.ItemsWithCollision)
                {
                    var collisionEvents = this.eventRulesCatalog[EventRuleType.Collision].Cast<ICollisionEventRule>();

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
        /// Evaluates movement event rules on for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        private bool EvaluateMovementEventRules(IThing thingMoving, ICreature requestor)
        {
            var movementEventRules = this.eventRulesCatalog[EventRuleType.Movement].Cast<IThingMovementEventRule>();

            var rulesThatCanBeExecuted = movementEventRules.Where(e => e.Setup(thingMoving, null, requestor as IPlayer) && e.CanBeExecuted);

            // Execute all actions.
            if (rulesThatCanBeExecuted.Any())
            {
                foreach (var rule in rulesThatCanBeExecuted)
                {
                    rule.Execute();
                }

                return true;
            }

            return false;
        }

        private bool AddContentToCylinderChain(IEnumerable<ICylinder> cylinderChain, byte firstAttemptIndex, ref IThing remainder, ICreature requestorCreature = null)
        {
            cylinderChain.ThrowIfNull(nameof(cylinderChain));

            const byte FallbackIndex = 0xFF;

            bool success = false;
            bool firstAttempt = true;

            foreach (var targetCylinder in cylinderChain)
            {
                IThing lastAddedThing = remainder;

                if (!success)
                {
                    (success, remainder) = targetCylinder.AddContent(this.ItemFactory, remainder, firstAttempt ? firstAttemptIndex : FallbackIndex);
                }
                else if (remainder != null)
                {
                    (success, remainder) = targetCylinder.AddContent(this.ItemFactory, remainder);
                }

                firstAttempt = false;

                if (success)
                {
                    if (targetCylinder is ITile targetTile)
                    {
                        this.RequestNofitication(
                            new TileUpdatedNotification(
                                this.Logger,
                                this.CreatureManager,
                                () => this.GetConnectionsOfPlayersThatCanSee(targetTile.Location),
                                new TileUpdatedNotificationArguments(targetTile.Location, this.GetDescriptionOfTile)));
                    }

                    this.EvaluateCollisionEventRules(targetCylinder.Location, lastAddedThing, requestorCreature);
                    this.EvaluateMovementEventRules(lastAddedThing, requestorCreature);
                }

                if (success && remainder == null)
                {
                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="player">The player for which the container is being opened.</param>
        /// <param name="containerItem">The container.</param>
        /// <param name="asContainerId">The id as which to open the container.</param>
        private void PerformPlayerContainerOpen(IPlayer player, IContainerItem containerItem, byte asContainerId = 0xFF)
        {
            player.ThrowIfNull(nameof(player));
            containerItem.ThrowIfNull(nameof(containerItem));

            var currentContainer = player.GetContainerById(asContainerId);

            // Start tracking this container if we're not doing it.
            if (currentContainer != containerItem && containerItem.OpenedBy.Count == 0)
            {
                containerItem.OnContentAdded += this.HandleContainerContentAdded;
                containerItem.OnContentRemoved += this.HandleContainerContentRemoved;
                containerItem.OnContentUpdated += this.HandleContainerContentUpdated;

                containerItem.OnThingChanged += this.HandleContainerChanged;
            }

            player.OpenContainerAt(containerItem, asContainerId);

            // Close the event listeners on the container that was closed, if any.
            if (currentContainer != null && currentContainer.OpenedBy.Count == 0)
            {
                currentContainer.OnContentAdded -= this.HandleContainerContentAdded;
                currentContainer.OnContentRemoved -= this.HandleContainerContentRemoved;
                currentContainer.OnContentUpdated -= this.HandleContainerContentUpdated;

                currentContainer.OnThingChanged -= this.HandleContainerChanged;
            }

            if (containerItem.IsTracking(player.Id, out byte containerId))
            {
                this.RequestNofitication(
                    new GenericNotification(
                        this.Logger,
                        () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(
                            new ContainerOpenPacket(
                                containerId,
                                containerItem.ThingId,
                                containerItem.Type.Name,
                                containerItem.Capacity,
                                (containerItem.ParentCylinder is IContainerItem parentContainer) && parentContainer.Type.TypeId != 0,
                                containerItem.Content))));
            }
        }

        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="player">The player for which the container is being opened.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="asContainerId">The id of the container being closed, as seen by the player.</param>
        private void PerformPlayerContainerClose(IPlayer player, IContainerItem containerItem, byte asContainerId)
        {
            player.ThrowIfNull(nameof(player));
            containerItem.ThrowIfNull(nameof(containerItem));

            player.CloseContainerWithId(asContainerId);

            // clean up events if no one else cares about this container.
            if (containerItem.OpenedBy.Count == 0)
            {
                containerItem.OnContentAdded -= this.HandleContainerContentAdded;
                containerItem.OnContentRemoved -= this.HandleContainerContentRemoved;
                containerItem.OnContentUpdated -= this.HandleContainerContentUpdated;

                containerItem.OnThingChanged -= this.HandleContainerChanged;
            }

            this.RequestNofitication(
                new GenericNotification(
                    this.Logger,
                    () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                    new GenericNotificationArguments(new ContainerClosePacket(asContainerId))));
        }

        /// <summary>
        /// Attempts to place a creature on the map.
        /// </summary>
        /// <param name="location">The location to place the creature at.</param>
        /// <param name="creature">The creature to place.</param>
        /// <returns>True if the creature is successfully added to the map, false otherwise.</returns>
        private bool PlaceCreatureOnMap(Location location, ICreature creature)
        {
            if (location.Type != LocationType.Map)
            {
                return false;
            }

            if (this.map.GetTileAt(location, out ITile targetTile))
            {
                var (addResult, _) = targetTile.AddContent(this.ItemFactory, creature);

                if (addResult)
                {
                    this.CreatureManager.RegisterCreature(creature);

                    this.Logger.Debug($"Placed {creature.Name} at {location}.");

                    IEnumerable<IConnection> TargetConnectionsFunc()
                    {
                        if (creature is IPlayer player)
                        {
                            return this.GetConnectionsOfPlayersThatCanSee(location).Except(this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem());
                        }

                        return this.GetConnectionsOfPlayersThatCanSee(location);
                    }

                    var placedAtStackPos = targetTile.GetStackPositionOfThing(creature);

                    this.RequestNofitication(
                        new CreatureMovedNotification(
                            this.Logger,
                            this,
                            this.CreatureManager,
                            TargetConnectionsFunc,
                            new CreatureMovedNotificationArguments(creature.Id, default, byte.MaxValue, location, placedAtStackPos, wasTeleport: true)));
                }

                return addResult;
            }

            return false;
        }

        /// <summary>
        /// Handles a change event from a container.
        /// </summary>
        /// <param name="containerThatChangedAsThing">The container that changed.</param>
        /// <param name="eventArgs">The event arguments of the change.</param>
        private void HandleContainerChanged(IThing containerThatChangedAsThing, ThingStateChangedEventArgs eventArgs)
        {
            if (!(containerThatChangedAsThing is IContainerItem containerItem) || !eventArgs.PropertyChanged.Equals(nameof(containerItem.Location)))
            {
                return;
            }

            if (containerItem.Location.Type == LocationType.Map)
            {
                // Container was dropped or placed in a container that ultimately sits on the map, figure out which creatures are still in range.
                foreach (var (creatureId, containerId) in containerItem.OpenedBy.ToList())
                {
                    if (!(this.CreatureManager.FindCreatureById(creatureId) is IPlayer player))
                    {
                        continue;
                    }

                    var locationDiff = containerItem.Location - player.Location;

                    if (locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0)
                    {
                        this.PerformPlayerContainerClose(player, containerItem, containerId);
                    }
                }
            }
            else
            {
                // Container is held by a creature, which is the only one that has access now.
                // TODO: implement 'holders' ?
            }
        }

        /// <summary>
        /// Handles an event from a container content added.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="addedItem">The item that was added.</param>
        private void HandleContainerContentAdded(IContainerItem container, IItem addedItem)
        {
            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.CreatureManager.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.RequestNofitication(
                    new GenericNotification(
                        this.Logger,
                        () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerAddItemPacket(containerId, addedItem))));
            }
        }

        /// <summary>
        /// Handles an event from a container content removed.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexRemoved">The index that was removed.</param>
        private void HandleContainerContentRemoved(IContainerItem container, byte indexRemoved)
        {
            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.CreatureManager.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.RequestNofitication(
                    new GenericNotification(
                        this.Logger,
                        () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerRemoveItemPacket(indexRemoved, containerId))));
            }
        }

        /// <summary>
        /// Handles an event from a container content updated.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexOfUpdated">The index that was updated.</param>
        /// <param name="updatedItem">The updated item.</param>
        private void HandleContainerContentUpdated(IContainerItem container, byte indexOfUpdated, IItem updatedItem)
        {
            if (updatedItem == null)
            {
                return;
            }

            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.CreatureManager.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.RequestNofitication(
                    new GenericNotification(
                        this.Logger,
                        () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerUpdateItemPacket((byte)indexOfUpdated, containerId, updatedItem))));
            }
        }

        private void HandlePlayerInventoryChanged(IInventory inventory, Slot slot, IItem item)
        {
            if (!(inventory.Owner is IPlayer player))
            {
                return;
            }

            this.Logger.Information($"{player.Name}'s inventory slot {slot} changed to {item?.Type.Name ?? "empty"}.");

            var notificationArgs = item == null ?
                new GenericNotificationArguments(new PlayerInventoryClearSlotPacket(slot))
                :
                new GenericNotificationArguments(new PlayerInventorySetSlotPacket(slot, item));

            var notification = new GenericNotification(this.Logger, () => this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(), notificationArgs);

            this.RequestNofitication(notification);
        }

        /// <summary>
        /// Calculates the step duration of a creature moving from a given tile in the given direction.
        /// </summary>
        /// <param name="creature">The creature that's moving.</param>
        /// <param name="stepDirection">The direction of the step.</param>
        /// <param name="fromTile">The tile which the creature is moving from.</param>
        /// <returns>The duration time of the step.</returns>
        private TimeSpan CalculateStepDuration(ICreature creature, Direction stepDirection, ITile fromTile)
        {
            if (creature == null)
            {
                return TimeSpan.Zero;
            }

            var tilePenalty = fromTile?.Ground?.MovementPenalty ?? DefaultGroundMovementPenalty;

            var totalPenalty = tilePenalty * (stepDirection.IsDiagonal() ? 2 : 1);

            var durationInMs = Math.Ceiling(1000 * totalPenalty / (double)Math.Max(1u, creature.Speed) / 50) * 50;

            return TimeSpan.FromMilliseconds(durationInMs);
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

        /// <summary>
        /// Handles a window loaded event from the map loader.
        /// </summary>
        /// <param name="fromX">The start X coordinate for the loaded window.</param>
        /// <param name="toX">The end X coordinate for the loaded window.</param>
        /// <param name="fromY">The start Y coordinate for the loaded window.</param>
        /// <param name="toY">The end Y coordinate for the loaded window.</param>
        /// <param name="fromZ">The start Z coordinate for the loaded window.</param>
        /// <param name="toZ">The end Z coordinate for the loaded window.</param>
        private void HandleMapWindowLoaded(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ)
        {
            // For spawns, check which fall within this window:
            var spawnsInWindow = this.monsterSpawns
                .Where(s => s.Location.X >= fromX && s.Location.X <= toX &&
                            s.Location.Y >= fromY && s.Location.Y <= toY &&
                            s.Location.Z >= fromZ && s.Location.Z <= toZ);

            if (spawnsInWindow != null)
            {
                foreach (var spawn in spawnsInWindow)
                {
                    this.ScriptRequest_PlaceMonsterAt(spawn.Location, spawn.Id);
                }
            }
        }
    }
}
