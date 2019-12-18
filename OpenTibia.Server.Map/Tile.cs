// -----------------------------------------------------------------
// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

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
        /// <param name="ground">The ground item to initialize the tile with.</param>
        public Tile(IItem ground)
        {
            this.Ground = ground;
            this.Flags = (byte)TileFlag.None;
            this.LastModified = DateTimeOffset.UtcNow;

            this.creatureIdsOnTile = new Stack<uint>();
            this.stayOnTopItems = new Stack<IItem>();
            this.stayOnBottomItems = new Stack<IItem>();
            this.itemsOnTile = new Stack<IItem>();
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
        /// Sets a flag on this tile.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        public void SetFlag(TileFlag flag)
        {
            this.Flags |= (byte)flag;
        }

        /// <summary>
        /// Attempts to add a <see cref="IThing"/> to this tile.
        /// </summary>
        /// <param name="itemFactory">The item factory in use.</param>
        /// <param name="thing">The thing to add.</param>
        /// <param name="count">The amount of the thing to add.</param>
        public void AddThing(IItemFactory itemFactory, ref IThing thing, byte count = 1)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
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
                        if (item.IsCumulative)
                        {
                            var currentItem = this.itemsOnTile.Count > 0 ? this.itemsOnTile.Peek() as IItem : null;

                            if (currentItem != null && currentItem.Type == item.Type && currentItem.Amount < IItem.MaximumAmountOfCummulativeItems)
                            {
                                // add these up.
                                var remaining = currentItem.Amount + count;

                                var newCount = (byte)Math.Min(remaining, IItem.MaximumAmountOfCummulativeItems);

                                currentItem.SetAmount(newCount);

                                remaining -= newCount;

                                if (remaining > 0)
                                {
                                    IThing newThing = itemFactory.Create(item.Type.TypeId);

                                    this.AddThing(itemFactory, ref newThing, (byte)remaining);

                                    thing = newThing;
                                }
                            }
                            else
                            {
                                item.SetAmount(count);

                                this.itemsOnTile.Push(item);
                            }
                        }
                        else
                        {
                            this.itemsOnTile.Push(item);
                        }
                    }
                }

                // Update the tile's version so that it invalidates the cache.
                // TOOD: if we start caching creatures, move to outer scope.
                this.LastModified = DateTimeOffset.UtcNow;
            }
        }

        /// <summary>
        /// Attempts to remove a <see cref="IThing"/> from this tile.
        /// </summary>
        /// <param name="itemFactory">The item factory in use.</param>
        /// <param name="thing">The <see cref="IThing"/> to remove.</param>
        /// <param name="count">The amount of the <see cref="IThing"/> to remove.</param>
        /// <returns>True if the thing was found and at least partially removed, false otherwise.</returns>
        public bool RemoveThing(IItemFactory itemFactory, ref IThing thing, byte count = 1)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (thing is ICreature creature)
            {
                return this.RemoveCreature(creature.Id);
            }
            else if (thing is IItem item)
            {
                if (item.IsGround)
                {
                    this.Ground = null;
                }
                else if (item.StaysOnTop)
                {
                    if (count > 1)
                    {
                        throw new ArgumentException($"Invalid count while removing a stay-on-top item: {count}.");
                    }

                    return this.InternalRemoveStayOnTopItem(thing);
                }
                else if (item.StaysOnBottom)
                {
                    if (count > 1)
                    {
                        throw new ArgumentException($"Invalid count while removing a stay-on-bottom item: {count}.");
                    }

                    return this.InternalRemoveStayOnBottomItem(thing);
                }
                else
                {
                    // TODO: revise this.
                    lock (this.itemsOnTile)
                    {
                        // Down items.
                        var removeItem = true;

                        if (item.IsCumulative)
                        {
                            if (item.Amount < count)
                            {
                                return false;
                            }

                            if (item.Amount > count)
                            {
                                // create a new item (it got split...)
                                var newItem = itemFactory.Create(item.Type.TypeId);

                                newItem.SetAmount(count);

                                item.SetAmount((byte)(item.Amount - count));

                                thing = newItem;

                                removeItem = false;
                            }
                        }

                        if (removeItem)
                        {
                            this.itemsOnTile.Pop();
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

            return true;
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
        /// Attempts to remove an item with a given type in this tile.
        /// </summary>
        /// <param name="typeId">The type to look for an remove.</param>
        /// <returns>True if such an item was found and removed, false otherwise.</returns>
        public bool RemoveItemWithId(ushort typeId)
        {
            if (this.Ground != null && this.Ground.ThingId == typeId)
            {
                this.Ground = null;
                return true;
            }

            if (this.InternalRemoveStayOnTopItemById(typeId))
            {
                return true;
            }

            if (this.InternalRemoveStayOnBottomItemById(typeId))
            {
                return true;
            }

            return this.InternalRemoveItemById(typeId);
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
        /// <param name="position">The zero-based position in the full stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        public IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte position)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            var i = this.Ground == null ? 0 : 1;

            if (this.stayOnTopItems.Any() && position < i + this.stayOnTopItems.Count)
            {
                return this.stayOnTopItems.ElementAt(position - i);
            }

            i += this.stayOnTopItems.Count;

            if (this.stayOnBottomItems.Any() && position < i + this.stayOnBottomItems.Count)
            {
                return this.stayOnBottomItems.ElementAt(position - i);
            }

            i += this.stayOnBottomItems.Count;

            if (this.creatureIdsOnTile.Any() && position < i + this.creatureIdsOnTile.Count)
            {
                return creatureFinder.FindCreatureById(this.creatureIdsOnTile.ElementAt(position - i));
            }

            i += this.creatureIdsOnTile.Count;

            if (this.itemsOnTile.Any() && position < i + this.itemsOnTile.Count)
            {
                return this.itemsOnTile.ElementAt(position - i);
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

            return wasRemoved;
        }
    }
}
