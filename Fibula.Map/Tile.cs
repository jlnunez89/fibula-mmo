// -----------------------------------------------------------------
// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Parsing.Contracts.Abstractions;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Map.Contracts.Abstractions;
    using Fibula.Server.Map.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a tile in the map.
    /// </summary>
    public class Tile : ITile
    {
        /// <summary>
        /// Stores the ids of the creatures in the tile.
        /// </summary>
        private readonly Stack<uint> creatureIdsOnTile;

        /// <summary>
        /// Stores the 'top' items on the tile.
        /// </summary>
        private readonly Stack<IItem> stayOnTopItems;

        /// <summary>
        /// Stores the 'top 2' items on the tile.
        /// </summary>
        private readonly Stack<IItem> stayOnBottomItems;

        /// <summary>
        /// Stores the down items on the tile.
        /// </summary>
        private readonly Stack<IItem> itemsOnTile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="location">The location of this tile.</param>
        /// <param name="ground">The ground item to initialize the tile with.</param>
        public Tile(Location location, IItem ground)
        {
            if (location.Type != LocationType.Map)
            {
                throw new ArgumentException($"Invalid location {location} for tile. A tile must have a {LocationType.Map} location.");
            }

            this.Location = location;

            this.Ground = ground;
            this.Flags = (byte)TileFlag.None;
            this.LastModified = DateTimeOffset.UtcNow;

            this.creatureIdsOnTile = new Stack<uint>();
            this.stayOnTopItems = new Stack<IItem>();
            this.stayOnBottomItems = new Stack<IItem>();
            this.itemsOnTile = new Stack<IItem>();
        }

        /// <summary>
        /// Gets this tile's location.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the location where this entity is being carried at, which is null for tiles.
        /// </summary>
        public Location? CarryLocation
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the single ground item that a tile may have.
        /// </summary>
        public IItem Ground { get; private set; }

        /// <summary>
        /// Gets the tile's creature ids.
        /// </summary>
        public IEnumerable<uint> CreatureIds => this.creatureIdsOnTile;

        /// <summary>
        /// Gets the tile's 'stay-on-top' items.
        /// </summary>
        public IEnumerable<IItem> StayOnTopItems => this.stayOnTopItems;

        /// <summary>
        /// Gets the tile's 'stay-on-bottom' items.
        /// </summary>
        public IEnumerable<IItem> StayOnBottomItems => this.stayOnBottomItems;

        /// <summary>
        /// Gets the tile's normal items.
        /// </summary>
        public IEnumerable<IItem> Items => this.itemsOnTile;

        /// <summary>
        /// Gets the flags from this tile.
        /// </summary>
        public byte Flags { get; private set; }

        /// <summary>
        /// Gets the last date and time that this tile was modified.
        /// </summary>
        public DateTimeOffset LastModified { get; private set; }

        /// <summary>
        /// Gets the count of creatures in this tile.
        /// </summary>
        public int CreatureCount => this.creatureIdsOnTile.Count;

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via collision evaluation.
        /// </summary>
        public bool HasCollisionEvents
        {
            get
            {
                return (this.Ground != null && this.Ground.HasCollision) || this.StayOnTopItems.Any(i => i.HasCollision) || this.stayOnBottomItems.Any(i => i.HasCollision) || this.Items.Any(i => i.HasCollision);
            }
        }

        /// <summary>
        /// Gets any items in the tile that have a collision event flag.
        /// </summary>
        public IEnumerable<IItem> ItemsWithCollision
        {
            get
            {
                var items = new List<IItem>();

                if (this.Ground.HasCollision)
                {
                    items.Add(this.Ground);
                }

                lock (this.stayOnTopItems)
                {
                    items.AddRange(this.stayOnTopItems.Where(i => i.HasCollision));
                }

                lock (this.stayOnBottomItems)
                {
                    items.AddRange(this.stayOnBottomItems.Where(i => i.HasCollision));
                }

                lock (this.itemsOnTile)
                {
                    items.AddRange(this.itemsOnTile.Where(i => i.HasCollision));
                }

                return items;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via separation events.
        /// </summary>
        public bool HasSeparationEvents
        {
            get
            {
                return (this.Ground != null && this.Ground.HasSeparation) || this.StayOnTopItems.Any(i => i.HasSeparation) || this.stayOnBottomItems.Any(i => i.HasSeparation) || this.Items.Any(i => i.HasSeparation);
            }
        }

        /// <summary>
        /// Gets any items in the tile that have a separation event flag.
        /// </summary>
        public IEnumerable<IItem> ItemsWithSeparation
        {
            get
            {
                var items = new List<IItem>();

                if (this.Ground.HasSeparation)
                {
                    items.Add(this.Ground);
                }

                lock (this.stayOnTopItems)
                {
                    items.AddRange(this.stayOnTopItems.Where(i => i.HasSeparation));
                }

                lock (this.stayOnBottomItems)
                {
                    items.AddRange(this.stayOnBottomItems.Where(i => i.HasSeparation));
                }

                lock (this.itemsOnTile)
                {
                    items.AddRange(this.itemsOnTile.Where(i => i.HasSeparation));
                }

                return items;
            }
        }

        /// <summary>
        /// Gets a value indicating whether items in this tile block throwing.
        /// </summary>
        public bool BlocksThrow
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksThrow) || this.StayOnTopItems.Any(i => i.BlocksThrow) || this.StayOnBottomItems.Any(i => i.BlocksThrow) || this.Items.Any(i => i.BlocksThrow);
            }
        }

        /// <summary>
        /// Gets a value indicating whether items in this tile block passing.
        /// </summary>
        public bool BlocksPass
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksPass) || this.CreatureIds.Any() || this.StayOnTopItems.Any(i => i.BlocksPass) || this.StayOnBottomItems.Any(i => i.BlocksPass) || this.Items.Any(i => i.BlocksPass);
            }
        }

        /// <summary>
        /// Gets a value indicating whether items in this tile block laying.
        /// </summary>
        public bool BlocksLay
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksLay) || this.StayOnTopItems.Any(i => i.BlocksLay) || this.StayOnBottomItems.Any(i => i.BlocksLay) || this.Items.Any(i => i.BlocksLay);
            }
        }

        /// <summary>
        /// Gets or sets the parent cylinder of this tile.
        /// </summary>
        public IThingContainer Parent
        {
            get
            {
                return null;
            }

            set
            {
                throw new InvalidOperationException($"{nameof(Tile)}s cannot have a parent cylinder.");
            }
        }

        /// <summary>
        /// Sets a flag on this tile.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        public void SetFlag(TileFlag flag)
        {
            this.Flags |= (byte)flag;
        }

        /// <summary>
        /// Checks if the tile has an item with the given type.
        /// </summary>
        /// <param name="typeId">The type to check for.</param>
        /// <returns>True if the tile contains at least one item with such id, false otherwise.</returns>
        public bool HasItemWithId(ushort typeId)
        {
            if (this.Ground != null && this.Ground.ThingId == typeId)
            {
                return true;
            }

            lock (this.stayOnTopItems)
            {
                if (this.stayOnTopItems.Any())
                {
                    foreach (var item in this.stayOnTopItems)
                    {
                        if (item.ThingId == typeId)
                        {
                            return true;
                        }
                    }
                }
            }

            lock (this.stayOnBottomItems)
            {
                if (this.stayOnBottomItems.Any())
                {
                    foreach (var item in this.stayOnBottomItems)
                    {
                        if (item.ThingId == typeId)
                        {
                            return true;
                        }
                    }
                }
            }

            lock (this.itemsOnTile)
            {
                if (this.itemsOnTile.Any())
                {
                    foreach (var item in this.itemsOnTile)
                    {
                        if (item.ThingId == typeId)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to find an item in the tile with the given type.
        /// </summary>
        /// <param name="typeId">The type to look for.</param>
        /// <returns>The item with such id, null otherwise.</returns>
        public IItem FindItemWithId(ushort typeId)
        {
            if (this.Ground != null && this.Ground.ThingId == typeId)
            {
                return this.Ground;
            }

            lock (this.stayOnTopItems)
            {
                if (this.stayOnTopItems.Any())
                {
                    foreach (var item in this.stayOnTopItems)
                    {
                        if (item.ThingId == typeId)
                        {
                            return item;
                        }
                    }
                }
            }

            lock (this.stayOnBottomItems)
            {
                if (this.stayOnBottomItems.Any())
                {
                    foreach (var item in this.stayOnBottomItems)
                    {
                        if (item.ThingId == typeId)
                        {
                            return item;
                        }
                    }
                }
            }

            lock (this.itemsOnTile)
            {
                if (this.itemsOnTile.Any())
                {
                    foreach (var item in this.itemsOnTile)
                    {
                        if (item.ThingId == typeId)
                        {
                            return item;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to remove an item with a given type in this tile.
        /// </summary>
        /// <param name="typeId">The type to look for an remove.</param>
        /// <returns>True if such an item was found and removed, false otherwise.</returns>
        public bool RemoveItemWithId(ushort typeId)
        {
            bool itemRemoved = false;

            if (this.Ground != null && this.Ground.ThingId == typeId)
            {
                this.Ground = null;

                itemRemoved = true;
            }
            else if (this.InternalRemoveStayOnTopItemById(typeId) || this.InternalRemoveStayOnBottomItemById(typeId) || this.InternalRemoveItemById(typeId))
            {
                itemRemoved = true;
            }

            if (itemRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return itemRemoved;
        }

        /// <summary>
        /// Attempts to get the position in the stack for the given type id.
        /// </summary>
        /// <param name="typeId">The type id of the item to find.</param>
        /// <returns>The position in the stack for the item found, or <see cref="byte.MaxValue"/> if it's not found.</returns>
        public byte GetPositionOfItemWithId(ushort typeId)
        {
            byte n = 0;

            if (this.Ground != null && typeId == this.Ground.Type.TypeId)
            {
                return n;
            }

            foreach (var item in this.StayOnTopItems)
            {
                ++n;
                if (typeId == item.Type.TypeId)
                {
                    return n;
                }
            }

            foreach (var item in this.StayOnBottomItems)
            {
                ++n;
                if (typeId == item.Type.TypeId)
                {
                    return n;
                }
            }

            n += (byte)this.CreatureIds.Count();

            foreach (var item in this.Items)
            {
                ++n;
                if (typeId == item.Type.TypeId)
                {
                    return n;
                }
            }

            return byte.MaxValue;
        }

        /// <summary>
        /// Attempts to get the position in the stack for the given <see cref="IThing"/>.
        /// </summary>
        /// <param name="thing">The thing to find.</param>
        /// <returns>The position in the stack for the <see cref="IThing"/>, or <see cref="byte.MaxValue"/> if its not found.</returns>
        public byte GetStackPositionOfThing(IThing thing)
        {
            thing.ThrowIfNull(nameof(thing));

            byte n = 0;

            if (this.Ground != null && thing == this.Ground)
            {
                return n;
            }

            foreach (var item in this.StayOnTopItems)
            {
                ++n;
                if (thing == item)
                {
                    return n;
                }
            }

            foreach (var item in this.StayOnBottomItems)
            {
                ++n;
                if (thing == item)
                {
                    return n;
                }
            }

            foreach (var creatureId in this.CreatureIds)
            {
                ++n;
                if (thing is ICreature creature && creature.Id == creatureId)
                {
                    return n;
                }
            }

            foreach (var item in this.Items)
            {
                ++n;
                if (thing == item)
                {
                    return n;
                }
            }

            return byte.MaxValue;
        }

        /// <summary>
        /// Attempts to get the tile's top <see cref="IThing"/> depending on the given position.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="stackPos">The zero-based position in the full stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        public IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte stackPos)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            var i = this.Ground == null ? 0 : 1;

            if (this.stayOnTopItems.Any() && stackPos < i + this.stayOnTopItems.Count)
            {
                return this.stayOnTopItems.ElementAt(Math.Max(0, stackPos - i));
            }

            i += this.stayOnTopItems.Count;

            if (this.stayOnBottomItems.Any() && stackPos < i + this.stayOnBottomItems.Count)
            {
                return this.stayOnBottomItems.ElementAt(Math.Max(0, stackPos - i));
            }

            i += this.stayOnBottomItems.Count;

            if (this.creatureIdsOnTile.Any() && stackPos < i + this.creatureIdsOnTile.Count)
            {
                return creatureFinder.FindCreatureById(this.creatureIdsOnTile.ElementAt(Math.Max(0, stackPos - i)));
            }

            i += this.creatureIdsOnTile.Count;

            if (this.itemsOnTile.Any() && stackPos < i + this.itemsOnTile.Count)
            {
                return this.itemsOnTile.ElementAt(Math.Max(0, stackPos - i));
            }

            // when nothing else works, return the ground (if any).
            return this.Ground;
        }

        /// <summary>
        /// Forcefully adds parsed content elements to this container.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="contentElements">The content elements to add.</param>
        // TODO: move to somewhere else to decouple.
        public void AddContent(ILogger logger, IThingFactory thingFactory, IEnumerable<IParsedElement> contentElements)
        {
            logger.ThrowIfNull(nameof(logger));
            thingFactory.ThrowIfNull(nameof(thingFactory));
            contentElements.ThrowIfNull(nameof(contentElements));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            // load and add tile flags and contents.
            foreach (var e in contentElements)
            {
                foreach (var attribute in e.Attributes)
                {
                    if (attribute.Name.Equals("Content"))
                    {
                        if (attribute.Value is IEnumerable<IParsedElement> elements)
                        {
                            var thingStack = new Stack<IThing>();

                            foreach (var element in elements)
                            {
                                if (element.IsFlag)
                                {
                                    // A flag is unexpected in this context.
                                    logger.Warning($"Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");

                                    continue;
                                }

                                IItem item = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = (ushort)element.Id });

                                if (item == null)
                                {
                                    logger.Warning($"Item with id {element.Id} not found in the catalog, skipping.");

                                    continue;
                                }

                                item.SetAttributes(logger.ForContext<IItem>(), itemFactory, element.Attributes);

                                thingStack.Push(item);
                            }

                            // Add them in reversed order.
                            while (thingStack.Count > 0)
                            {
                                var thing = thingStack.Pop();

                                this.AddContent(itemFactory, thing);

                                if (thing is IContainedThing thingWithParentCylinder)
                                {
                                    thingWithParentCylinder.ParentContainer = this;
                                }
                            }
                        }
                    }
                    else
                    {
                        // it's a flag
                        if (Enum.TryParse(attribute.Name, out TileFlag flagMatch))
                        {
                            this.SetFlag(flagMatch);
                        }
                        else
                        {
                            logger.Warning($"Unknown flag [{attribute.Name}] found on tile at location {this.Location}.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to add a <see cref="IThing"/> to this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index at which to add the <see cref="IThing"/>. Defaults to 0xFF, which instructs to add the <see cref="IThing"/> at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) AddContent(IThingFactory thingFactory, IThing thing, byte index = 0xFF)
        {
            thingFactory.ThrowIfNull(nameof(thingFactory));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            if (thing is ICreature creature)
            {
                lock (this.creatureIdsOnTile)
                {
                    this.creatureIdsOnTile.Push(creature.Id);
                }
            }
            else if (thing is IItem item)
            {
                if (item.IsGround)
                {
                    this.Ground = item;
                }
                else if (item.StaysOnTop)
                {
                    lock (this.stayOnTopItems)
                    {
                        this.stayOnTopItems.Push(item);
                    }
                }
                else if (item.StaysOnBottom)
                {
                    lock (this.stayOnBottomItems)
                    {
                        this.stayOnBottomItems.Push(item);
                    }
                }
                else
                {
                    lock (this.itemsOnTile)
                    {
                        var remainingAmountToAdd = item.Amount;

                        while (remainingAmountToAdd > 0)
                        {
                            if (!item.IsCumulative)
                            {
                                this.itemsOnTile.Push(item);
                                break;
                            }

                            var existingItem = this.itemsOnTile.Count > 0 ? this.itemsOnTile.Peek() as IItem : null;

                            // Check if there is an existing top item and if it is of the same type.
                            if (existingItem == null || existingItem.Type != item.Type || existingItem.Amount >= IItem.MaximumAmountOfCummulativeItems)
                            {
                                this.itemsOnTile.Push(item);
                                break;
                            }

                            remainingAmountToAdd += existingItem.Amount;

                            // Modify the existing item with the new amount, or the maximum permitted.
                            var newExistingAmount = Math.Min(remainingAmountToAdd, IItem.MaximumAmountOfCummulativeItems);

                            existingItem.SetAmount(newExistingAmount);

                            remainingAmountToAdd -= newExistingAmount;

                            if (remainingAmountToAdd == 0)
                            {
                                break;
                            }

                            item = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = item.Type.TypeId });

                            item.SetAmount(remainingAmountToAdd);

                            item.ParentContainer = this;
                        }
                    }
                }

                // Update the tile's version so that it invalidates the cache.
                // TOOD: if we start caching creatures, move to outer scope.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            if (thing != null && thing is IContainedThing containedThing)
            {
                containedThing.ParentContainer = this;
            }

            return (true, null);
        }

        /// <summary>
        /// Attempts to remove a thing from this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="index">Optional. The index from which to remove the <see cref="IThing"/>. Defaults to 0xFF, which instructs to remove the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) RemoveContent(IThingFactory thingFactory, ref IThing thing, byte index = 0xFF, byte amount = 1)
        {
            thingFactory.ThrowIfNull(nameof(thingFactory));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            if (amount == 0)
            {
                throw new ArgumentException($"Invalid {nameof(amount)} zero.");
            }

            IItem remainder = null;

            if (thing is ICreature creature)
            {
                return (this.RemoveCreature(creature.Id), null);
            }
            else if (thing is IItem item)
            {
                if (item.IsGround)
                {
                    this.Ground = null;
                }
                else if (item.StaysOnTop)
                {
                    if (amount > 1)
                    {
                        throw new ArgumentException($"Invalid {nameof(amount)} while removing a stay-on-top item: {amount}.");
                    }

                    return (this.InternalRemoveStayOnTopItem(thing), null);
                }
                else if (item.StaysOnBottom)
                {
                    if (amount > 1)
                    {
                        throw new ArgumentException($"Invalid {nameof(amount)} while removing a stay-on-bottom item: {amount}.");
                    }

                    return (this.InternalRemoveStayOnBottomItem(thing), null);
                }
                else
                {
                    lock (this.itemsOnTile)
                    {
                        if ((!item.IsCumulative && amount > 1) || (item.IsCumulative && item.Amount < amount))
                        {
                            return (false, null);
                        }

                        if (!item.IsCumulative || item.Amount == amount)
                        {
                            // Since we have the exact amount, we can remove the item instance from the tile.
                            this.itemsOnTile.Pop();
                        }
                        else
                        {
                            // We're removing less than the entire amount, so we need to calculate the remainder to add back.
                            var newExistingAmount = (byte)(item.Amount - amount);

                            item.SetAmount(newExistingAmount);

                            // item amount is left wrong.

                            // Create a new item as the remainder.
                            remainder = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = item.Type.TypeId });

                            remainder.SetAmount(amount);

                            thing = remainder;
                            remainder = item;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"Thing did not cast to either a {nameof(ICreature)} or {nameof(IItem)}.");
            }

            // Update the tile's version so that it invalidates the cache.
            this.LastModified = DateTimeOffset.UtcNow;

            return (true, remainder);
        }

        /// <summary>
        /// Attempts to replace a <see cref="IThing"/> from this container with another.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="fromThing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="toThing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index from which to replace the <see cref="IThing"/>. Defaults to 0xFF, which instructs to replace the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="fromThing"/> to replace.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainderToChange) ReplaceContent(IThingFactory thingFactory, IThing fromThing, IThing toThing, byte index = 0xFF, byte amount = 1)
        {
            (bool removeSuccessful, IThing removeRemainder) = this.RemoveContent(thingFactory, ref fromThing, index, amount);

            if (!removeSuccessful)
            {
                return (false, removeRemainder);
            }

            if (removeRemainder != null)
            {
                (bool addedRemainder, IThing remainderOfRemainder) = this.AddContent(thingFactory, removeRemainder, 0xFF);

                if (!addedRemainder)
                {
                    return (false, remainderOfRemainder);
                }
            }

            return this.AddContent(thingFactory, toThing, index);
        }

        ///// <summary>
        ///// Gets this tile's cylinder hierarchy.
        ///// </summary>
        ///// <param name="includeTile">The parameter is not used.</param>
        ///// <returns>The ordered collection of <see cref="IThingContainer"/>s in this tile's cylinder hierarchy.</returns>
        //public IEnumerable<IThingContainer> GetCylinderHierarchy(bool includeTile = true)
        //{
        //    return this.YieldSingleItem();
        //}

        /// <summary>
        /// Determines if this tile is considered to be blocking the path.
        /// </summary>
        /// <param name="avoidTypes">The damage types to avoid when checking for path blocking. By default, all types are considered path blocking.</param>
        /// <returns>True if the tile is considered path blocking, false otherwise.</returns>
        public bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All)
        {
            var blocking = this.BlocksPass;

            if (blocking)
            {
                return true;
            }

            blocking |= (this.Ground != null && this.Ground.IsPathBlocking(avoidTypes)) ||
                        this.CreatureIds.Any() ||
                        this.StayOnTopItems.Any(i => i.IsPathBlocking(avoidTypes)) ||
                        this.StayOnBottomItems.Any(i => i.IsPathBlocking(avoidTypes)) ||
                        this.Items.Any(i => i.IsPathBlocking(avoidTypes));

            return blocking;
        }

        /// <summary>
        /// Attempts to find an <see cref="IThing"/> whitin this container.
        /// </summary>
        /// <param name="index">The index at which to look for the <see cref="IThing"/>.</param>
        /// <returns>The <see cref="IThing"/> found at the index, if any was found.</returns>
        public IThing FindThingAtIndex(byte index)
        {
            var i = this.Ground == null ? 0 : 1;

            if (this.stayOnTopItems.Any() && index < i + this.stayOnTopItems.Count)
            {
                return this.stayOnTopItems.ElementAt(Math.Max(0, index - i));
            }

            i += this.stayOnTopItems.Count;

            if (this.stayOnBottomItems.Any() && index < i + this.stayOnBottomItems.Count)
            {
                return this.stayOnBottomItems.ElementAt(Math.Max(0, index - i));
            }

            i += this.stayOnBottomItems.Count;

            if (this.creatureIdsOnTile.Any() && index < i + this.creatureIdsOnTile.Count)
            {
                // Not an item.
                return null;
            }

            i += this.creatureIdsOnTile.Count;

            if (this.itemsOnTile.Any() && index < i + this.itemsOnTile.Count)
            {
                return this.itemsOnTile.ElementAt(Math.Max(0, index - i));
            }

            // when nothing else works, return the ground (if any).
            return this.Ground;
        }

        /// <summary>
        /// Attempts to remove the given creature id from the stack of this tile.
        /// </summary>
        /// <param name="creatureId">The id of the creature to remove.</param>
        /// <returns>True if the id is found and removed, false otherwise.</returns>
        private bool RemoveCreature(uint creatureId)
        {
            var tempStack = new Stack<uint>();

            uint removedCreatureId = default;

            lock (this.creatureIdsOnTile)
            {
                while (removedCreatureId == default && this.creatureIdsOnTile.Count > 0)
                {
                    var temp = this.creatureIdsOnTile.Pop();

                    if (creatureId == temp)
                    {
                        removedCreatureId = creatureId;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.creatureIdsOnTile.Push(tempStack.Pop());
                }
            }

            return removedCreatureId != default;
        }

        /// <summary>
        /// Attempts to remove a specific item of the stay-on-top category in this tile.
        /// </summary>
        /// <param name="stayOnTopItem">The item to remove.</param>
        /// <returns>True if the item was found and removed, false otherwise.</returns>
        private bool InternalRemoveStayOnTopItem(IThing stayOnTopItem)
        {
            if (stayOnTopItem == null)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.stayOnTopItems)
            {
                while (!wasRemoved && this.stayOnTopItems.Count > 0)
                {
                    var temp = this.stayOnTopItems.Pop();

                    if (stayOnTopItem == temp)
                    {
                        wasRemoved = true;
                        break;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.stayOnTopItems.Push(tempStack.Pop());
                }
            }

            if (wasRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return wasRemoved;
        }

        /// <summary>
        /// Attempts to remove a specific item of the stay-on-bottom category in this tile.
        /// </summary>
        /// <param name="stayOnBottomItem">The item to remove.</param>
        /// <returns>True if the item was found and removed, false otherwise.</returns>
        private bool InternalRemoveStayOnBottomItem(IThing stayOnBottomItem)
        {
            if (stayOnBottomItem == null)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.stayOnBottomItems)
            {
                while (!wasRemoved && this.stayOnBottomItems.Count > 0)
                {
                    var temp = this.stayOnBottomItems.Pop();

                    if (stayOnBottomItem == temp)
                    {
                        wasRemoved = true;
                        break;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.stayOnBottomItems.Push(tempStack.Pop());
                }
            }

            if (wasRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return wasRemoved;
        }

        /// <summary>
        /// Attempts to remove an item of the stay-on-top category in this tile, by it's type.
        /// </summary>
        /// <param name="stayOnTopItemTypeId">The type of the item to remove.</param>
        /// <returns>True if such an item was found and removed, false otherwise.</returns>
        private bool InternalRemoveStayOnTopItemById(ushort stayOnTopItemTypeId)
        {
            if (stayOnTopItemTypeId == default)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.stayOnTopItems)
            {
                while (!wasRemoved && this.stayOnTopItems.Count > 0)
                {
                    var temp = this.stayOnTopItems.Pop();

                    if (stayOnTopItemTypeId == temp.ThingId)
                    {
                        wasRemoved = true;
                        break;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.stayOnTopItems.Push(tempStack.Pop());
                }
            }

            if (wasRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return wasRemoved;
        }

        /// <summary>
        /// Attempts to remove an item of the stay-on-bottom category in this tile, by it's type.
        /// </summary>
        /// <param name="stayOnBottomItemTypeId">The type of the item to remove.</param>
        /// <returns>True if such an item was found and removed, false otherwise.</returns>
        private bool InternalRemoveStayOnBottomItemById(ushort stayOnBottomItemTypeId)
        {
            if (stayOnBottomItemTypeId == default)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.stayOnBottomItems)
            {
                while (!wasRemoved && this.stayOnBottomItems.Count > 0)
                {
                    var temp = this.stayOnBottomItems.Pop();

                    if (stayOnBottomItemTypeId == temp.ThingId)
                    {
                        wasRemoved = true;
                        break;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.stayOnBottomItems.Push(tempStack.Pop());
                }
            }

            if (wasRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return wasRemoved;
        }

        /// <summary>
        /// Attempts to remove an item in this tile, by it's type.
        /// </summary>
        /// <param name="itemTypeId">The type of the item to remove.</param>
        /// <returns>True if such an item was found and removed, false otherwise.</returns>
        private bool InternalRemoveItemById(ushort itemTypeId)
        {
            if (itemTypeId == default)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.itemsOnTile)
            {
                while (!wasRemoved && this.itemsOnTile.Count > 0)
                {
                    var temp = this.itemsOnTile.Pop();

                    if (itemTypeId == temp.ThingId)
                    {
                        wasRemoved = true;
                        break;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.itemsOnTile.Push(tempStack.Pop());
                }
            }

            if (wasRemoved)
            {
                // Update the tile's version so that it invalidates the cache.
                this.LastModified = DateTimeOffset.UtcNow;
            }

            return wasRemoved;
        }
    }
}
