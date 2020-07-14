// -----------------------------------------------------------------
// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Fibula.Map.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a tile in the map.
    /// </summary>
    public class Tile : ITile
    {
        /// <summary>
        /// The object to use as the tile lock.
        /// </summary>
        private readonly object tileLock;

        /// <summary>
        /// Stores the creatures on the tile.
        /// </summary>
        private readonly Stack<ICreature> creaturesOnTile;

        /// <summary>
        /// Stores the 'top' items on the tile.
        /// </summary>
        private readonly Stack<IItem> stayOnTopItems;

        /// <summary>
        /// Stores the 'top 2' items on the tile.
        /// </summary>
        private readonly Stack<IItem> stayOnBottomItems;

        /// <summary>
        /// Stores the items on the tile.
        /// </summary>
        private readonly Stack<IItem> itemsOnTile;

        /// <summary>
        /// Stores the items on the tile that are ground borders.
        /// </summary>
        private readonly Stack<IItem> groundBorders;

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

            this.tileLock = new object();
            this.groundBorders = new Stack<IItem>();
            this.creaturesOnTile = new Stack<ICreature>();
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
        public Location? CarryLocation => null;

        /// <summary>
        /// Gets the single ground item that a tile may have.
        /// </summary>
        public IItem Ground { get; private set; }

        /// <summary>
        /// Gets the single liquid pool item that a tile may have.
        /// </summary>
        public IItem LiquidPool { get; private set; }

        /// <summary>
        /// Gets the tile's creature ids.
        /// </summary>
        public IEnumerable<ICreature> Creatures => this.creaturesOnTile;

        /// <summary>
        /// Gets the flags from this tile.
        /// </summary>
        public byte Flags { get; private set; }

        /// <summary>
        /// Gets the last date and time that this tile was modified.
        /// </summary>
        public DateTimeOffset LastModified { get; private set; }

        /// <summary>
        /// Gets a value indicating whether items in this tile block throwing.
        /// </summary>
        public bool BlocksThrow
        {
            get
            {
                // TODO: handle setting this as the items get added/removed to avoid constant calculation.
                return (this.Ground != null && this.Ground.BlocksThrow) ||
                        this.groundBorders.Any(i => i.BlocksThrow) ||
                        this.stayOnTopItems.Any(i => i.BlocksThrow) ||
                        this.stayOnBottomItems.Any(i => i.BlocksThrow) ||
                        this.itemsOnTile.Any(i => i.BlocksThrow);
            }
        }

        /// <summary>
        /// Gets a value indicating whether items in this tile block passing.
        /// </summary>
        public bool BlocksPass
        {
            get
            {
                // TODO: handle setting this as the items get added/removed to avoid constant calculation.
                return (this.Ground != null && this.Ground.BlocksPass) ||
                        this.Creatures.Any() ||
                        this.groundBorders.Any(i => i.BlocksPass) ||
                        this.stayOnTopItems.Any(i => i.BlocksPass) ||
                        this.stayOnBottomItems.Any(i => i.BlocksPass) ||
                        this.itemsOnTile.Any(i => i.BlocksPass);
            }
        }

        /// <summary>
        /// Gets a value indicating whether items in this tile block laying.
        /// </summary>
        public bool BlocksLay
        {
            get
            {
                // TODO: handle setting this as the items get added/removed to avoid constant calculation.
                return (this.Ground != null && this.Ground.BlocksLay) ||
                        this.groundBorders.Any(i => i.BlocksLay) ||
                        this.stayOnTopItems.Any(i => i.BlocksLay) ||
                        this.stayOnBottomItems.Any(i => i.BlocksLay) ||
                        this.itemsOnTile.Any(i => i.BlocksLay);
            }
        }

        /// <summary>
        /// Gets the thing that is on top based on the tile's stack order. Prioritizes creatures, then items.
        /// </summary>
        public IThing TopThing
        {
            get
            {
                return (IThing)this.TopCreature ?? this.TopItem;
            }
        }

        /// <summary>
        /// Gets the creature that is on top based on the tile's stack order.
        /// </summary>
        public ICreature TopCreature
        {
            get
            {
                return this.creaturesOnTile.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the item that is on top based on the tile's stack order.
        /// </summary>
        public IItem TopItem
        {
            get
            {
                return this.itemsOnTile.FirstOrDefault() ??
                       this.stayOnBottomItems.FirstOrDefault() ??
                       this.stayOnTopItems.FirstOrDefault() ??
                       this.LiquidPool ??
                       this.groundBorders.FirstOrDefault() ??
                       this.Ground;
            }
        }

        /// <summary>
        /// Attempts to get the tile's items to describe prioritized and ordered by their stack order.
        /// </summary>
        /// <param name="maxItemsToGet">The maximum number of items to include in the result.</param>
        /// <returns>The items in the tile, split by those which are fixed and those considered normal.</returns>
        /// <remarks>
        /// The algorithm prioritizes the returned items in the following order:
        /// 1) Ground item.
        /// 2) Clipped items.
        /// 3) Stay-on-bottom items.
        /// 4) Stay-on-top items.
        /// 5) Normal items.
        /// </remarks>
        public (IEnumerable<IItem> fixedItems, IEnumerable<IItem> normalItems) GetItemsToDescribeByPriority(int maxItemsToGet = MapConstants.MaximumNumberOfThingsToDescribePerTile)
        {
            var fixedItemList = new List<IItem>();
            var itemList = new List<IItem>();
            var addedCount = 0;

            lock (this.tileLock)
            {
                if (this.Ground != null)
                {
                    fixedItemList.Add(this.Ground);
                    addedCount++;
                }

                if (addedCount < maxItemsToGet && this.groundBorders.Any())
                {
                    var itemsToAdd = this.groundBorders.Take(maxItemsToGet - addedCount);

                    fixedItemList.AddRange(itemsToAdd);

                    addedCount += itemsToAdd.Count();
                }

                if (this.LiquidPool != null)
                {
                    fixedItemList.Add(this.LiquidPool);
                    addedCount++;
                }

                if (addedCount < maxItemsToGet && this.stayOnTopItems.Any())
                {
                    var itemsToAdd = this.stayOnTopItems.Take(maxItemsToGet - addedCount);

                    fixedItemList.AddRange(itemsToAdd);

                    addedCount += itemsToAdd.Count();
                }

                if (addedCount < maxItemsToGet && this.stayOnBottomItems.Any())
                {
                    var itemsToAdd = this.stayOnBottomItems.Take(maxItemsToGet - addedCount);

                    fixedItemList.AddRange(itemsToAdd);

                    addedCount += itemsToAdd.Count();
                }

                if (addedCount < maxItemsToGet && this.itemsOnTile.Any())
                {
                    var itemsToAdd = this.itemsOnTile.Take(maxItemsToGet - addedCount);

                    itemList.AddRange(itemsToAdd);

                    addedCount += itemsToAdd.Count();
                }
            }

            return (fixedItemList, itemList);
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

            if (this.LiquidPool != null && this.LiquidPool.ThingId == typeId)
            {
                return this.LiquidPool;
            }

            lock (this.tileLock)
            {
                return this.groundBorders.FirstOrDefault(i => i.ThingId == typeId) ?? this.stayOnTopItems.FirstOrDefault(i => i.ThingId == typeId) ?? this.stayOnBottomItems.FirstOrDefault(i => i.ThingId == typeId) ?? this.itemsOnTile.FirstOrDefault(i => i.ThingId == typeId);
            }
        }

        /// <summary>
        /// Attempts to get the position in the stack for the given <see cref="IThing"/>.
        /// </summary>
        /// <param name="thing">The thing to find.</param>
        /// <returns>The position in the stack for the <see cref="IThing"/>, or <see cref="byte.MaxValue"/> if its not found.</returns>
        public byte GetStackOrderOfThing(IThing thing)
        {
            thing.ThrowIfNull(nameof(thing));

            byte i = (byte)(this.Ground != null ? 1 : 0);

            if (this.Ground != null && thing == this.Ground)
            {
                return i;
            }

            foreach (var item in this.groundBorders)
            {
                if (thing == item)
                {
                    return i;
                }

                i++;
            }

            if (this.LiquidPool != null)
            {
                if (thing == this.LiquidPool)
                {
                    return i;
                }

                i++;
            }

            foreach (var item in this.stayOnTopItems)
            {
                if (thing == item)
                {
                    return i;
                }

                i++;
            }

            foreach (var item in this.stayOnBottomItems)
            {
                if (thing == item)
                {
                    return i;
                }

                i++;
            }

            foreach (var creatureOnTile in this.creaturesOnTile)
            {
                if (thing is ICreature creature && creature == creatureOnTile)
                {
                    return i;
                }

                i++;
            }

            foreach (var item in this.itemsOnTile)
            {
                if (thing == item)
                {
                    return i;
                }

                i++;
            }

            return byte.MaxValue;
        }

        /// <summary>
        /// Attempts to find an <see cref="IThing"/> whitin this container.
        /// </summary>
        /// <param name="index">The index at which to look for the <see cref="IThing"/>.</param>
        /// <returns>The <see cref="IThing"/> found at the index, if any was found.</returns>
        public IThing FindThingAtIndex(byte index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to add a <see cref="IThing"/> to this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index at which to add the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to add the <see cref="IThing"/> at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) AddContent(IThingFactory thingFactory, IThing thing, byte index = byte.MaxValue)
        {
            thingFactory.ThrowIfNull(nameof(thingFactory));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            lock (this.tileLock)
            {
                if (thing is ICreature creature)
                {
                    this.creaturesOnTile.Push(creature);
                }
                else if (thing is IItem item)
                {
                    if (item.IsGround)
                    {
                        this.Ground = item;
                    }
                    else if (item.IsGroundFix)
                    {
                        this.groundBorders.Push(item);
                    }
                    else if (item.IsLiquidPool)
                    {
                        this.LiquidPool = item;
                    }
                    else if (item.StaysOnTop)
                    {
                        this.stayOnTopItems.Push(item);
                    }
                    else if (item.StaysOnBottom)
                    {
                        this.stayOnBottomItems.Push(item);
                    }
                    else
                    {
                        var remainingAmountToAdd = item.Amount;

                        while (remainingAmountToAdd > 0)
                        {
                            if (!item.IsCumulative)
                            {
                                this.itemsOnTile.Push(item);
                                break;
                            }

                            var existingItem = this.itemsOnTile.Count > 0 ? this.itemsOnTile.Peek() : null;

                            // Check if there is an existing top item and if it is of the same type.
                            if (existingItem == null || existingItem.Type != item.Type || existingItem.Amount >= ItemConstants.MaximumAmountOfCummulativeItems)
                            {
                                this.itemsOnTile.Push(item);
                                break;
                            }

                            remainingAmountToAdd += existingItem.Amount;

                            // Modify the existing item with the new amount, or the maximum permitted.
                            var newExistingAmount = Math.Min(remainingAmountToAdd, ItemConstants.MaximumAmountOfCummulativeItems);

                            existingItem.Amount = newExistingAmount;

                            remainingAmountToAdd -= newExistingAmount;

                            if (remainingAmountToAdd == 0)
                            {
                                break;
                            }

                            item = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = item.Type.TypeId });

                            item.Amount = remainingAmountToAdd;

                            item.ParentContainer = this;
                        }
                    }

                    // Update the tile's version so that it invalidates the cache.
                    // TOOD: if we start caching creatures, move to outer scope.
                    this.LastModified = DateTimeOffset.UtcNow;
                }
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
        /// <param name="index">Optional. The index from which to remove the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to remove the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) RemoveContent(IThingFactory thingFactory, ref IThing thing, byte index = byte.MaxValue, byte amount = 1)
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
                return (this.InternalRemoveCreature(creature), null);
            }
            else if (thing is IItem item)
            {
                if (item.IsGround)
                {
                    this.Ground = null;
                }
                else if (item.IsGroundFix)
                {
                    if (amount > 1)
                    {
                        throw new ArgumentException($"Invalid {nameof(amount)} while removing a ground border item: {amount}.");
                    }

                    return (this.InternalRemoveGroundBorderItem(thing), null);
                }
                else if (item.IsLiquidPool)
                {
                    this.LiquidPool = null;
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
                    lock (this.tileLock)
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

                            item.Amount = newExistingAmount;

                            // item amount is left wrong.

                            // Create a new item as the remainder.
                            remainder = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = item.Type.TypeId });

                            remainder.Amount = amount;

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
        /// <param name="index">Optional. The index from which to replace the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to replace the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="fromThing"/> to replace.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainderToChange) ReplaceContent(IThingFactory thingFactory, IThing fromThing, IThing toThing, byte index = byte.MaxValue, byte amount = 1)
        {
            (bool removeSuccessful, IThing removeRemainder) = this.RemoveContent(thingFactory, ref fromThing, index, amount);

            if (!removeSuccessful)
            {
                return (false, removeRemainder);
            }

            if (removeRemainder != null)
            {
                (bool addedRemainder, IThing remainderOfRemainder) = this.AddContent(thingFactory, removeRemainder, byte.MaxValue);

                if (!addedRemainder)
                {
                    return (false, remainderOfRemainder);
                }
            }

            return this.AddContent(thingFactory, toThing, index);
        }

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
                        this.Creatures.Any() ||
                        this.stayOnTopItems.Any(i => i.IsPathBlocking(avoidTypes)) ||
                        this.stayOnBottomItems.Any(i => i.IsPathBlocking(avoidTypes)) ||
                        this.itemsOnTile.Any(i => i.IsPathBlocking(avoidTypes));

            return blocking;
        }

        /// <summary>
        /// Attempts to remove the given creature id from the stack of this tile.
        /// </summary>
        /// <param name="creature">The creature to attempt to remove.</param>
        /// <returns>True if the id is found and removed, false otherwise.</returns>
        private bool InternalRemoveCreature(ICreature creature)
        {
            var tempStack = new Stack<ICreature>();

            ICreature removedCreature = null;

            lock (this.tileLock)
            {
                while (removedCreature == default && this.creaturesOnTile.Count > 0)
                {
                    var tempCreature = this.creaturesOnTile.Pop();

                    if (creature == tempCreature)
                    {
                        removedCreature = creature;
                    }
                    else
                    {
                        tempStack.Push(tempCreature);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.creaturesOnTile.Push(tempStack.Pop());
                }
            }

            return removedCreature != null;
        }

        /// <summary>
        /// Attempts to remove a specific item of the ground borders category in this tile.
        /// </summary>
        /// <param name="groundBorderItem">The item to remove.</param>
        /// <returns>True if the item was found and removed, false otherwise.</returns>
        private bool InternalRemoveGroundBorderItem(IThing groundBorderItem)
        {
            if (groundBorderItem == null)
            {
                return false;
            }

            var tempStack = new Stack<IItem>();

            bool wasRemoved = false;

            lock (this.tileLock)
            {
                while (!wasRemoved && this.groundBorders.Count > 0)
                {
                    var temp = this.groundBorders.Pop();

                    if (groundBorderItem == temp)
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
                    this.groundBorders.Push(tempStack.Pop());
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

            lock (this.tileLock)
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

            lock (this.tileLock)
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

            lock (this.tileLock)
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

            lock (this.tileLock)
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

            lock (this.tileLock)
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
