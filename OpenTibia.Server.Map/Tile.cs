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
        private readonly Stack<IItem> topItems1OnTile;

        /// <summary>
        /// Stores the 'top 2' items on the tile.
        /// </summary>
        private readonly Stack<IItem> topItems2OnTile;

        /// <summary>
        /// Stores the down items on the tile.
        /// </summary>
        private readonly Stack<IItem> downItemsOnTile;

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
            this.topItems1OnTile = new Stack<IItem>();
            this.topItems2OnTile = new Stack<IItem>();
            this.downItemsOnTile = new Stack<IItem>();
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
        /// Gets the tile's 'top' items.
        /// </summary>
        public IEnumerable<IItem> TopItems1 => this.topItems1OnTile;

        /// <summary>
        /// Gets the tile's 'top 2' items.
        /// </summary>
        public IEnumerable<IItem> TopItems2 => this.topItems2OnTile;

        /// <summary>
        /// Gets the tile's down items.
        /// </summary>
        public IEnumerable<IItem> DownItems => this.downItemsOnTile;

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
                this.creatureIdsOnTile.Push(creature.Id);
            }
            else if (thing is IItem item)
            {
                if (item.IsGround)
                {
                    this.Ground = item;
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Push(item);
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Push(item);
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        var currentItem = this.downItemsOnTile.Count > 0 ? this.downItemsOnTile.Peek() as IItem : null;

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

                            this.downItemsOnTile.Push(item);
                        }
                    }
                    else
                    {
                        this.downItemsOnTile.Push(item);
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
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Pop();
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Pop();
                }
                else
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
                        this.downItemsOnTile.Pop();
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"Thing did not cast to either a {nameof(ICreature)} or {nameof(IItem)}.");
            }

            this.LastModified = DateTimeOffset.UtcNow;

            return true;
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

            foreach (var item in this.TopItems1)
            {
                ++n;
                if (thing == item)
                {
                    return n;
                }
            }

            foreach (var item in this.TopItems2)
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

            foreach (var item in this.DownItems)
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
        /// Attempts to get the <see cref="IThing"/> at the position of the stack given.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="stackPosition">The position in the stack.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        public IThing GetThingAtStackPosition(ICreatureFinder creatureFinder, byte stackPosition)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            if (stackPosition == 0 && this.Ground != null)
            {
                return this.Ground;
            }

            var currentPos = this.Ground == null ? -1 : 0;

            if (stackPosition > currentPos + this.topItems1OnTile.Count)
            {
                currentPos += this.topItems1OnTile.Count;
            }
            else
            {
                foreach (var item in this.topItems1OnTile)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.topItems2OnTile.Count)
            {
                currentPos += this.topItems2OnTile.Count;
            }
            else
            {
                foreach (var item in this.topItems2OnTile)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.creatureIdsOnTile.Count)
            {
                currentPos += this.creatureIdsOnTile.Count;
            }
            else
            {
                foreach (var creatureId in this.creatureIdsOnTile)
                {
                    if (++currentPos == stackPosition)
                    {
                        return creatureFinder.FindCreatureById(creatureId);
                    }
                }
            }

            return stackPosition <= currentPos + this.downItemsOnTile.Count ? this.downItemsOnTile.Skip(1 - stackPosition - currentPos).First() : null;
        }

        /// <summary>
        /// Attempts to get the tile's top <see cref="IThing"/> depending on the given order.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="order">The order in the stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        public IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte order)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            var i = 0;

            if (order > i + this.topItems1OnTile.Count)
            {
                i += this.topItems1OnTile.Count;
            }
            else if (this.topItems1OnTile.Count > 0)
            {
                return this.topItems1OnTile.Skip(1 - order - i).First();
            }

            if (order > i + this.topItems2OnTile.Count)
            {
                i += this.topItems2OnTile.Count;
            }
            else if (this.topItems2OnTile.Count > 0)
            {
                return this.topItems2OnTile.Skip(1 - order - i).First();
            }

            if (order > i + this.creatureIdsOnTile.Count)
            {
                i += this.creatureIdsOnTile.Count;
            }
            else if (this.creatureIdsOnTile.Count > 0)
            {
                return creatureFinder.FindCreatureById(this.creatureIdsOnTile.Skip(1 - order - i).First());
            }

            if (order <= i + this.downItemsOnTile.Count && this.downItemsOnTile.Count > 0)
            {
                return this.downItemsOnTile.Skip(1 - order - i).First();
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
    }
}
