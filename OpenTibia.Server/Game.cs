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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Arguments;
    using OpenTibia.Server.Parsing.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents the game instance.
    /// </summary>
    public class Game : IGame, IEventRulesEvaluator
    {
        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks for orphaned conections.
        /// </summary>
        private static readonly TimeSpan CheckConnectionsDelay = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The current <see cref="IMap"/> instance.
        /// </summary>
        private readonly IMap map;

        /// <summary>
        /// The connection manager instance.
        /// </summary>
        private readonly IConnectionManager connectionManager;

        /// <summary>
        /// The creature manager instance.
        /// </summary>
        private readonly ICreatureManager creatureManager;

        /// <summary>
        /// Gets the creature factory instance.
        /// </summary>
        private readonly ICreatureFactory creatureFactory;

        /// <summary>
        /// The reference to the opeartions factory.
        /// </summary>
        private readonly IOperationFactory operationFactory;

        /// <summary>
        /// Gets the scheduler used by the game instance.
        /// </summary>
        private readonly IScheduler scheduler;

        /// <summary>
        /// Stores a reference to the pathfinder helper algorithm.
        /// </summary>
        private readonly IPathFinder pathFinder;

        /// <summary>
        /// Gets the monster spawns in the game.
        /// </summary>
        private readonly IEnumerable<Spawn> monsterSpawns;

        /// <summary>
        /// A reference to the logger in use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> containing the <see cref="IEventRule"/>s of the game.
        /// </summary>
        private readonly IDictionary<EventRuleType, ISet<IEventRule>> eventRulesCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="map">A reference to the map to use.</param>
        /// <param name="pathFinder">A reference to the pathfinder algorithm helper instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="eventRulesLoader">A reference to the event rules loader.</param>
        /// <param name="monsterSpawnsLoader">A reference to the monster spawns loader.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="scheduler">A reference to the global scheduler instance.</param>
        public Game(
            ILogger logger,
            IMap map,
            IPathFinder pathFinder,
            IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            IEventRulesLoader eventRulesLoader,
            IMonsterSpawnLoader monsterSpawnsLoader,
            ICreatureFactory creatureFactory,
            IOperationFactory operationFactory,
            IScheduler scheduler)
        {
            logger.ThrowIfNull(nameof(logger));
            map.ThrowIfNull(nameof(map));
            pathFinder.ThrowIfNull(nameof(pathFinder));
            connectionManager.ThrowIfNull(nameof(connectionManager));
            creatureManager.ThrowIfNull(nameof(creatureManager));
            eventRulesLoader.ThrowIfNull(nameof(eventRulesLoader));
            monsterSpawnsLoader.ThrowIfNull(nameof(monsterSpawnsLoader));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            operationFactory.ThrowIfNull(nameof(operationFactory));

            this.logger = logger.ForContext<Game>();
            this.connectionManager = connectionManager;
            this.creatureManager = creatureManager;
            this.creatureFactory = creatureFactory;
            this.operationFactory = operationFactory;
            this.scheduler = scheduler;
            this.map = map;
            this.pathFinder = pathFinder;

            // Load some catalogs.
            this.eventRulesCatalog = eventRulesLoader.LoadEventRules();
            this.monsterSpawns = monsterSpawnsLoader.LoadSpawns();

            // Initialize game vars.
            this.Status = WorldState.Loading;
            this.WorldLightColor = (byte)LightColors.White;
            this.WorldLightLevel = (byte)LightLevels.World;

            // Hook some event handlers.
            this.scheduler.OnEventFired += this.ProcessEvent;
            this.map.WindowLoaded += this.OnMapWindowLoaded;
        }

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

                // var creatureThinkingTask = Task.Factory.StartNew(this.CreatureThinkingLoop, cancellationToken, TaskCreationOptions.LongRunning);

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
        /// Evaluates any rules of the given type using the supplied arguments.
        /// </summary>
        /// <param name="caller">The evaluation requestor.</param>
        /// <param name="type">The type of rules to evaluate.</param>
        /// <param name="eventRuleArguments">The arguments to evaluate with.</param>
        /// <returns>True if at least one rule was matched and executed, false otherwise.</returns>
        public bool EvaluateRules(object caller, EventRuleType type, IEventRuleArguments eventRuleArguments)
        {
            switch (type)
            {
                case EventRuleType.Use:
                    if (eventRuleArguments is UseEventRuleArguments useRuleArgs)
                    {
                        return this.EvaluateUseEventRules(useRuleArgs.ItemUsed, useRuleArgs.Requestor);
                    }

                    break;

                case EventRuleType.Collision:
                    if (eventRuleArguments is CollisionEventRuleArguments collisionRuleArgs)
                    {
                        return this.EvaluateCollisionEventRules(collisionRuleArgs.AtLocation, collisionRuleArgs.ThingMoving, collisionRuleArgs.Requestor);
                    }

                    break;

                case EventRuleType.Movement:
                    if (eventRuleArguments is MovementEventRuleArguments movementRuleArgs)
                    {
                        return this.EvaluateMovementEventRules(movementRuleArgs.ThingMoving, movementRuleArgs.Requestor);
                    }

                    break;

                // case EventRuleType.MultiUse:
                //    if (eventRuleArguments is MultiUseEventRuleArguments multiUseRuleArgs)
                //    {
                //        return this.EvaluateMultiUseEventRules(multiUseRuleArgs.ItemUsed, multiUseRuleArgs.Requestor);
                //    }

                //    break;

                case EventRuleType.Separation:
                    if (eventRuleArguments is SeparationEventRuleArguments separationRuleArgs)
                    {
                        return this.EvaluateSeparationEventRules(separationRuleArgs.AtLocation, separationRuleArgs.ThingMoving, separationRuleArgs.Requestor);
                    }

                    break;

                default:
                    this.logger.Warning($"Unsupported type '{type}' of rule evalation requested, ignored.");
                    break;
            }

            return false;
        }

        public bool CompareItemCountAt(Location location, FunctionComparisonType comparisonType, ushort value)
        {
            if (!this.map.GetTileAt(location, out ITile tile))
            {
                return false;
            }

            var count = tile.Ground != null ? 1 : 0;

            count += tile.Items.Count();

            return comparisonType switch
            {
                FunctionComparisonType.Equal => count == value,
                FunctionComparisonType.GreaterThanOrEqual => count >= value,
                FunctionComparisonType.LessThanOrEqual => count <= value,
                FunctionComparisonType.GreaterThan => count > value,
                FunctionComparisonType.LessThan => count < value,

                _ => false,
            };
        }

        public bool IsAllowedToLogOut(IPlayer player)
        {
            return player.IsAllowedToLogOut;
        }

        public bool IsAtLocation(IThing thing, Location location)
        {
            return thing != null && thing.Location == location;
        }

        public bool IsCreature(IThing thing)
        {
            return thing is ICreature;
        }

        public bool IsDressed(IThing thing)
        {
            if (!(thing is IItem item))
            {
                return false;
            }

            return (item.ParentCylinder is BodyContainerItem bodyContainer) && (item.DressPosition == Slot.Anywhere || item.DressPosition == bodyContainer.Slot);
        }

        public bool IsHouse(IThing thing)
        {
            // TODO: implement houses.
            return false; // thing?.Tile != null && thing.Tile.IsHouse;
        }

        public bool IsHouseOwner(IThing thing, IPlayer user)
        {
            // TODO: implement house ownership.
            return this.IsHouse(thing); // && thing.Tile.House.Owner == user.Name;
        }

        public bool IsSpecificItem(IThing thing, ushort typeId)
        {
            return thing is IItem item && item.Type.TypeId == typeId;
        }

        public bool IsPlayer(IThing thing)
        {
            return thing is IPlayer;
        }

        public bool IsObjectThere(Location location, ushort typeId)
        {
            return this.map.GetTileAt(location, out ITile targetTile) && targetTile.HasItemWithId(typeId);
        }

        public bool HasAccessFlag(IPlayer user, string rightStr)
        {
            return true; // TODO: implement.
        }

        public bool HasFlag(IThing itemThing, string flagStr)
        {
            if (!(itemThing is IItem))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(flagStr))
            {
                return true;
            }

            return Enum.TryParse(flagStr, out ItemFlag parsedFlag) && ((IItem)itemThing).Type.Flags.Contains(parsedFlag);
        }

        public bool HasProfession(IThing thing, byte profesionId)
        {
            // TODO: implement professions.
            return thing != null && thing is IPlayer && false;
        }

        public bool CompareItemAttribute(IThing thing, ItemAttribute attribute, FunctionComparisonType comparisonType, ushort value)
        {
            if (thing == null || !(thing is IItem thingAsItem))
            {
                return false;
            }

            if (!thingAsItem.Attributes.ContainsKey(attribute))
            {
                return false;
            }

            return comparisonType switch
            {
                FunctionComparisonType.Equal => Convert.ToUInt16(thingAsItem.Attributes[attribute]) == value,
                FunctionComparisonType.GreaterThanOrEqual => Convert.ToUInt16(thingAsItem.Attributes[attribute]) >= value,
                FunctionComparisonType.LessThanOrEqual => Convert.ToUInt16(thingAsItem.Attributes[attribute]) <= value,
                FunctionComparisonType.GreaterThan => Convert.ToUInt16(thingAsItem.Attributes[attribute]) > value,
                FunctionComparisonType.LessThan => Convert.ToUInt16(thingAsItem.Attributes[attribute]) < value,

                _ => false,
            };
        }

        public bool IsRandomNumberUnder(byte value, int maxValue = 100)
        {
            return new Random().Next(maxValue) <= value;
        }

        /// <summary>
        /// Attempts to create an item at a given location.
        /// </summary>
        /// <param name="location">The location at which to create the item.</param>
        /// <param name="itemTypeId">The type id of the item to create.</param>
        /// <param name="effect">An effect to use when the creation takes place.</param>
        public void CreateItemAtLocation(Location location, ushort itemTypeId, byte effect)
        {
            this.DispatchOperation(OperationType.CreateItem, new CreateItemOperationCreationArguments(requestorId: 0, itemTypeId, location));
        }

        /// <summary>
        /// Attempts to change a given item to the supplied id.
        /// </summary>
        /// <param name="thing">The thing to change.</param>
        /// <param name="toTypeId">The id of the item type to change to.</param>
        /// <param name="effect">An optional effect to send as part of the change.</param>
        public void ChangeItem(IThing thing, ushort toTypeId, byte effect)
        {
            if (thing == null)
            {
                return;
            }

            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect >= (byte)AnimatedEffect.First && effect <= (byte)AnimatedEffect.Last)
            {
                animatedEffect = (AnimatedEffect)effect;

                this.scheduler.ScheduleEvent(
                    new AnimatedEffectNotification(
                        this.logger,
                        () => this.connectionManager.PlayersThatCanSee(this.creatureManager, thing.Location),
                        new AnimatedEffectNotificationArguments(thing.Location, animatedEffect)));
            }

            this.DispatchOperation(
                    OperationType.ChangeItem,
                    new ChangeItemOperationCreationArguments(
                        requestorId: 0,
                        thing.ThingId,
                        thing.CarryLocation ?? thing.Location,
                        toTypeId,
                        carrierCreature: (thing is IItem item) ? item.Carrier : null));
        }

        /// <summary>
        /// Attempts to change a given item to the supplied id at a given location.
        /// </summary>
        /// <param name="location">The location at which the change will happen.</param>
        /// <param name="fromTypeId">The id of the item from which the change is happening.</param>
        /// <param name="toTypeId">The id of the item to which the change is happening.</param>
        /// <param name="effect">An optional effect to send as part of the change.</param>
        public void ChangeItemAtLocation(Location location, ushort fromTypeId, ushort toTypeId, byte effect)
        {
            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect >= (byte)AnimatedEffect.First && effect <= (byte)AnimatedEffect.Last)
            {
                animatedEffect = (AnimatedEffect)effect;

                this.scheduler.ScheduleEvent(
                    new AnimatedEffectNotification(
                        this.logger,
                        () => this.connectionManager.PlayersThatCanSee(this.creatureManager, location),
                        new AnimatedEffectNotificationArguments(location, animatedEffect)));
            }

            this.DispatchOperation(
                    OperationType.ChangeItem,
                    new ChangeItemOperationCreationArguments(
                        requestorId: 0,
                        fromTypeId,
                        location,
                        toTypeId));
        }

        public void ChangePlayerStartLocation(IPlayer player, Location newLocation)
        {
            // TODO: implement.
        }

        /// <summary>
        /// Attempts to display an animated efect on the given location.
        /// </summary>
        /// <param name="location">The location at which to display the effect.</param>
        /// <param name="effect">The effect to display.</param>
        public void DisplayAnimatedEffectAt(Location location, byte effect)
        {
            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect >= (byte)AnimatedEffect.First && effect <= (byte)AnimatedEffect.Last)
            {
                animatedEffect = (AnimatedEffect)effect;
            }

            if (animatedEffect != AnimatedEffect.None)
            {
                this.scheduler.ScheduleEvent(
                    new AnimatedEffectNotification(
                        this.logger,
                        () => this.connectionManager.PlayersThatCanSee(this.creatureManager, location),
                        new AnimatedEffectNotificationArguments(location, animatedEffect)));
            }
        }

        public void DisplayAnimatedText(Location location, string text, byte textType)
        {
            this.scheduler.ScheduleEvent(
                new TextMessageNotification(
                    this.logger,
                    () => this.connectionManager.PlayersThatCanSee(this.creatureManager, location),
                    new TextMessageNotificationArguments((MessageType)textType, text)));
        }

        /// <summary>
        /// Attempts to delete an item.
        /// </summary>
        /// <param name="thing">The item to delete.</param>
        public void Delete(IThing thing)
        {
            if (thing == null || !(thing is IItem item))
            {
                return;
            }

            this.DispatchOperation(OperationType.DeleteItem, new DeleteItemOperationCreationArguments(requestorId: 0, item.ThingId, item.Location));
        }

        /// <summary>
        /// Attempts to delete an item at a given location on the map.
        /// </summary>
        /// <param name="location">The location at which to delete the item.</param>
        /// <param name="itemType">The type of the item to delete.</param>
        public void DeleteOnMap(Location location, ushort itemType)
        {
            this.DispatchOperation(OperationType.DeleteItem, new DeleteItemOperationCreationArguments(requestorId: 0, itemType, location));
        }

        /// <summary>
        /// Attempts to get the description (attribute only) of the item.
        /// </summary>
        /// <param name="thingToDescribe">The item to get the description of.</param>
        /// <param name="creature">The creature who the description is for.</param>
        public void DescribeFor(IThing thingToDescribe, ICreature creature)
        {
            if (thingToDescribe == null ||
                creature == null ||
                !(thingToDescribe is IItem itemToDescribe) ||
                string.IsNullOrWhiteSpace(itemToDescribe.Type.Description) ||
                !(creature is IPlayer player))
            {
                return;
            }

            this.scheduler.ScheduleEvent(
                new TextMessageNotification(
                    this.logger,
                    () => this.connectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                    new TextMessageNotificationArguments(MessageType.DescriptionGreen, itemToDescribe.Type.Description)));
        }

        public void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
        {
            // TODO: implement correctly when combat is...
            if (!(damagedThing is ICreature damagedCreature))
            {
                return;
            }

            switch (damageSourceType)
            {
                default: // physical
                    break;
                case 2: // magic? or mana?
                    break;
                case 4: // fire instant
                    this.DisplayAnimatedEffectAt(damagedCreature.Location, (byte)AnimatedEffect.Flame);
                    break;
                case 8: // energy instant
                    this.DisplayAnimatedEffectAt(damagedCreature.Location, (byte)AnimatedEffect.DamageEnergy);
                    break;
                case 16: // poison instant?
                    this.DisplayAnimatedEffectAt(damagedCreature.Location, (byte)AnimatedEffect.RingsGreen);
                    break;
                case 32: // poison over time (poisoned condition)
                    break;
                case 64: // fire over time (burned condition)
                    break;
                case 128: // energy over time (electrified condition)
                    break;
            }
        }

        public void LogPlayerOut(IPlayer player)
        {
            this.DispatchOperation(OperationType.LogOut, new LogOutOperationCreationArguments(player));
        }

        /// <summary>
        /// Attempts to move some thing to a given location.
        /// </summary>
        /// <param name="thingToMove">The thing to move.</param>
        /// <param name="targetLocation">The location to move the creature to.</param>
        public void MoveTo(IThing thingToMove, Location targetLocation)
        {
            if (thingToMove == null)
            {
                return;
            }

            if (thingToMove is ICreature creature)
            {
                TimeSpan creatureMovementCooldown = creature.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.scheduler.CurrentTime);

                this.DispatchOperation(
                        OperationType.MapToMapMovement,
                        new MapToMapMovementOperationCreationArguments(
                            requestorId: 0,
                            creature,
                            creature.Location,
                            targetLocation),
                        creatureMovementCooldown);
            }
            else if (thingToMove is IItem item)
            {
                this.DispatchOperation(
                        OperationType.MapToMapMovement,
                        new MapToMapMovementOperationCreationArguments(
                            requestorId: 0,
                            item,
                            item.Location,
                            targetLocation,
                            amount: item.Amount));
            }
        }

        /// <summary>
        /// Attempts to move all items and creatures in a location to a given location.
        /// </summary>
        /// <param name="fromLocation">The location from which to move everything.</param>
        /// <param name="targetLocation">The location to move everything to.</param>
        /// <param name="exceptTypeIds">Optional. Any type ids to explicitly exclude.</param>
        public void MoveTo(Location fromLocation, Location targetLocation, params ushort[] exceptTypeIds)
        {
            if (!this.map.GetTileAt(fromLocation, out ITile fromTile))
            {
                return;
            }

            foreach (var creatureId in fromTile.CreatureIds)
            {
                var creature = this.creatureManager.FindCreatureById(creatureId);

                if (creature == null)
                {
                    continue;
                }

                this.DispatchOperation(
                        OperationType.MapToMapMovement,
                        new MapToMapMovementOperationCreationArguments(
                            requestorId: 0,
                            creature,
                            fromLocation,
                            targetLocation));
            }

            foreach (var item in fromTile.Items)
            {
                if (exceptTypeIds.Length > 0 && exceptTypeIds.Contains(item.ThingId))
                {
                    continue;
                }

                this.DispatchOperation(
                        OperationType.MapToMapMovement,
                        new MapToMapMovementOperationCreationArguments(
                            requestorId: 0,
                            item,
                            fromLocation,
                            targetLocation,
                            amount: item.Amount));
            }
        }

        /// <summary>
        /// Attempts to move an item of the given type from the given location to another location.
        /// </summary>
        /// <param name="itemType">The type of the item to move.</param>
        /// <param name="fromLocation">The location from which to move the item.</param>
        /// <param name="toLocation">The location to which to move the item.</param>
        public void MoveTo(ushort itemType, Location fromLocation, Location toLocation)
        {
            if (!this.map.GetTileAt(fromLocation, out ITile fromTile) || !(fromTile.FindItemWithId(itemType) is IItem item) || item == null)
            {
                return;
            }

            this.DispatchOperation(
                    OperationType.MapToMapMovement,
                    new MapToMapMovementOperationCreationArguments(
                        requestorId: 0,
                        item,
                        item.Location,
                        toLocation,
                        amount: item.Amount));
        }

        /// <summary>
        /// Attempts to place a new monster at the given location.
        /// </summary>
        /// <param name="location">The location at which to place the monster.</param>
        /// <param name="monsterType">The race of the monster to place.</param>
        public void PlaceMonsterAt(Location location, ushort monsterType)
        {
            var newMonster = this.creatureFactory.Create(CreatureType.Monster, new MonsterCreationMetadata(monsterType));

            if (!this.map.GetTileAt(location, out ITile atTile))
            {
                this.logger.Warning($"No tile found at {location} to place creature at, ignoring.");

                return;
            }

            this.DispatchOperation(OperationType.PlaceCreature, new PlaceCreatureOperationCreationArguments(requestorId: 0, atTile, newMonster));
        }

        public void TagThing(IPlayer player, string format, IThing targetThing)
        {
            // TODO: implement.
        }

        private void DispatchOperation(OperationType operationType, IOperationCreationArguments operationArguments, TimeSpan withDelay = default)
        {
            IOperation newOperation = this.operationFactory.Create(operationType, operationArguments);

            // Hook up the event rules event listener here.
            // This gets unhooked when the operation is processed.
            newOperation.EventRulesEvaluationTriggered += this.EvaluateRules;

            // Normalize delay to protect against negative time spans.
            var operationDelay = withDelay < TimeSpan.Zero ? TimeSpan.Zero : withDelay;

            // Add delay from current exhaustion of the requestor, if any.
            if (operationArguments.RequestorId > 0 && this.creatureManager.FindCreatureById(operationArguments.RequestorId) is ICreature creature)
            {
                TimeSpan cooldownRemaining = creature.CalculateRemainingCooldownTime(newOperation.ExhaustionType, this.scheduler.CurrentTime);

                operationDelay += cooldownRemaining;
            }

            this.scheduler.ScheduleEvent(newOperation, operationDelay);
        }

        /// <summary>
        /// Evaluates use event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="thingBeingUsed">The thing that is being used.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        /// <remarks>This operation is not thread-safe.</remarks>
        private bool EvaluateUseEventRules(IThing thingBeingUsed, ICreature requestor)
        {
            if (thingBeingUsed is IItem itemUsed)
            {
                var useItemEventRules = this.eventRulesCatalog[EventRuleType.Use].Cast<IUseItemEventRule>();
                var requestorPlayer = requestor as IPlayer;
                var itemUsedAsThing = itemUsed as IThing;
                IThing discardThing = null;

                var rulesThatCanBeExecuted = useItemEventRules.Where(e => e.ItemToUseId == itemUsed.Type.TypeId && e.CanBeExecuted(this, itemUsed, null, requestorPlayer));

                // Execute all actions.
                if (rulesThatCanBeExecuted.Any())
                {
                    foreach (var rule in rulesThatCanBeExecuted)
                    {
                        rule.Execute(this, ref itemUsedAsThing, ref discardThing, ref requestorPlayer);
                    }

                    return true;
                }
            }

            return false;
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
                    var requestorPlayer = requestor as IPlayer;
                    var itemAsThing = item as IThing;

                    var rulesThatCanBeExecuted = separationEvents.Where(e => e.SeparatingThingId == item.Type.TypeId && e.CanBeExecuted(this, item, thingMoving, requestorPlayer));

                    // Execute all actions.
                    if (rulesThatCanBeExecuted.Any())
                    {
                        foreach (var rule in rulesThatCanBeExecuted)
                        {
                            rule.Execute(this, ref itemAsThing, ref thingMoving, ref requestorPlayer);
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
                    var requestorPlayer = requestor as IPlayer;
                    var itemAsThing = item as IThing;

                    var rulesThatCanBeExecuted = collisionEvents.Where(e => e.CollidingThingId == item.Type.TypeId && e.CanBeExecuted(this, item, thingMoving, requestorPlayer));

                    // Execute all actions.
                    if (rulesThatCanBeExecuted.Any())
                    {
                        foreach (var rule in rulesThatCanBeExecuted)
                        {
                            rule.Execute(this, ref itemAsThing, ref thingMoving, ref requestorPlayer);
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
            var requestorPlayer = requestor as IPlayer;
            IThing discardThing = null;

            var rulesThatCanBeExecuted = movementEventRules.Where(e => e.CanBeExecuted(this, thingMoving, null, requestorPlayer));

            // Execute all actions.
            if (rulesThatCanBeExecuted.Any())
            {
                foreach (var rule in rulesThatCanBeExecuted)
                {
                    rule.Execute(this, ref thingMoving, ref discardThing, ref requestorPlayer);
                }

                return true;
            }

            return false;
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
                    this.scheduler.ScheduleEvent(
                        new WorldLightChangedNotification(
                            this.logger,
                            () => this.connectionManager.GetAllActive(),
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
                // Thread.Sleep is OK here because ConnectionSweeper runs on it's own thread.
                Thread.Sleep(CheckConnectionsDelay);

                // Now let's clean up and try to log out all orphaned ones.
                foreach (var orphanedConnection in this.connectionManager.GetAllOrphaned())
                {
                    if (!(this.creatureManager.FindCreatureById(orphanedConnection.PlayerId) is IPlayer player))
                    {
                        continue;
                    }

                    player.SetAttackTarget(null);

                    this.DispatchOperation(OperationType.LogOut, new LogOutOperationCreationArguments(player));
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
                Stopwatch sw = Stopwatch.StartNew();

                evt.Execute();

                sw.Stop();

                this.logger.Verbose($"Processed event {evt.GetType().Name} with id: {evt.EventId}, current game time: {this.scheduler.CurrentTime.ToUnixTimeMilliseconds()}.");
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error in event {evt.GetType().Name} with id: {evt.EventId}: {ex.Message}.");
                this.logger.Error(ex.StackTrace);
            }
            finally
            {
                // If this was an operation that just ran, we need to do some extra stuff.
                if (evt is IOperation operation)
                {
                    // Unhook any event rule listeners we are currently tied to.
                    operation.EventRulesEvaluationTriggered -= this.EvaluateRules;

                    // Add any exhaustion for the requestor of the operation, if any.
                    if (this.creatureManager.FindCreatureById(operation.RequestorId) is ICreature requestor)
                    {
                        requestor.AddExhaustion(operation.ExhaustionType, this.scheduler.CurrentTime, operation.ExhaustionCost);
                    }
                }
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
        private void OnMapWindowLoaded(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ)
        {
            var rng = new Random();

            // For spawns, check which fall within this window:
            var spawnsInWindow = this.monsterSpawns
                .Where(s => s.Location.X >= fromX && s.Location.X <= toX &&
                            s.Location.Y >= fromY && s.Location.Y <= toY &&
                            s.Location.Z >= fromZ && s.Location.Z <= toZ);

            if (spawnsInWindow == null)
            {
                return;
            }

            foreach (var spawn in spawnsInWindow)
            {
                for (int i = 0; i < spawn.Count; i++)
                {
                    var r = spawn.Radius / 4;
                    var newMonster = this.creatureFactory.Create(CreatureType.Monster, new MonsterCreationMetadata(spawn.Id));

                    var randomLoc = spawn.Location + new Location { X = (int)Math.Round(r * Math.Cos(rng.Next(360))), Y = (int)Math.Round(r * Math.Sin(rng.Next(360))), Z = 0 };

                    // Need to actually pathfind to avoid placing a monster in unreachable places.
                    this.pathFinder.FindBetween(spawn.Location, randomLoc, out Location foundLocation, (i + 1) * 10);

                    // TODO: some property of newMonster here to figure out what actually blocks path finding.
                    if (this.map.GetTileAt(foundLocation, out ITile targetTile) && !targetTile.IsPathBlocking())
                    {
                        this.DispatchOperation(OperationType.PlaceCreature, new PlaceCreatureOperationCreationArguments(requestorId: 0, targetTile, newMonster));
                    }
                }
            }
        }
    }
}
