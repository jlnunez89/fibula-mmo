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

namespace OpenTibia.Server.Contracts.Structs
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Structure of a tile in the map.
    /// </summary>
    public struct Tile
    {
        /// <summary>
        /// A default <see cref="Tile"/> that is empty.
        /// </summary>
        public static readonly Tile Empty = default;

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
        /// Initializes a new instance of the <see cref="Tile"/> struct.
        /// </summary>
        /// <param name="ground">The ground item to initialize the tile with.</param>
        public Tile(IItem ground)
        {
            this.Ground = ground;
            this.Flags = (byte)TileFlag.None;

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

                        if (currentItem != null && currentItem.Type == item.Type && currentItem.Amount < 100)
                        {
                            // add these up.
                            var remaining = currentItem.Amount + count;

                            var newCount = (byte)Math.Min(remaining, 100);

                            // currentItem.Amount = newCount;

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
                            // item.Amount = count;

                            this.downItemsOnTile.Push(item);
                        }
                    }
                    else
                    {
                        this.downItemsOnTile.Push(item);
                    }
                }

                // invalidate the cache.
                // this.cachedDescription = null;
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
                var removeItem = true;

                if (item.IsGround)
                {
                    this.Ground = null;

                    removeItem = false;
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Pop();

                    removeItem = false;
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Pop();

                    removeItem = false;
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        if (item.Amount < count)
                        {
                            // throwing because this should have been checked before.
                            throw new ArgumentException("Remove count is greater than available.");
                        }

                        if (item.Amount > count)
                        {
                            // create a new item (it got split...)
                            var newItem = itemFactory.Create(item.Type.TypeId);

                            // newItem.SetAmount(count);

                            // item.Amount -= count;

                            thing = newItem;
                            removeItem = false;
                        }
                    }
                }

                if (removeItem)
                {
                    this.downItemsOnTile.Pop();

                    // item.Tile = null;
                }
            }
            else
            {
                throw new InvalidCastException($"Thing did not cast to either a {nameof(ICreature)} or {nameof(IItem)}.");
            }

            //this.contentLastEditionTime = DateTimeOffset.Now;

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
