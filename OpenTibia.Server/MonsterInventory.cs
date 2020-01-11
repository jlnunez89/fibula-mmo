// -----------------------------------------------------------------
// <copyright file="MonsterInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Monsters
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class MonsterInventory : IInventory
    {
        private readonly object[] recalcLocks;

        private Dictionary<byte, (IItem, ushort)> inventory;
        private byte lastPosByte;

        public event OnInventorySlotChanged OnSlotChanged;

        //private byte totalArmor;
        //private byte totalAttack;
        //private byte totalDefense;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterInventory"/> class.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="owner">The owner of this inventory.</param>
        /// <param name="inventoryComposition">A collection of monster inventory posibilities, composed of typeId, maximumAmount, and chance.</param>
        /// <param name="maxCapacity">The maximum capacity of the monster's inventory.</param>
        public MonsterInventory(IItemFactory itemFactory, ICreature owner, IEnumerable<(ushort typeId, byte maxAmount, ushort chance)> inventoryComposition, ushort maxCapacity = 20)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            owner.ThrowIfNull(nameof(owner));

            this.Owner = owner;
            this.inventory = new Dictionary<byte, (IItem, ushort)>();

            this.recalcLocks = new object[3];
            this.lastPosByte = 0;

            //this.totalAttack = 0xFF;
            //this.totalArmor = 0xFF;
            //this.totalDefense = 0xFF;

            this.DetermineLoot(itemFactory, inventoryComposition, maxCapacity);
        }

        public ICreature Owner { get; }

        public IItem this[Slot slot] => throw new NotImplementedException();

        public IItem this[byte idx] => this.inventory.ContainsKey(idx) ? this.inventory[idx].Item1 : null;

        //public byte TotalArmor
        //{
        //    get
        //    {
        //        if (this.totalArmor == 0xFF)
        //        {
        //            lock (this.recalcLocks[0])
        //            {
        //                if (this.totalArmor == 0xFF)
        //                {
        //                    var total = default(byte);

        //                    foreach (var tuple in this.inventory.Values)
        //                    {
        //                        total += tuple.Item1.Armor;
        //                    }

        //                    this.totalArmor = total;
        //                }
        //            }
        //        }

        //        return this.totalArmor;
        //    }
        //}

        //public byte TotalAttack
        //{
        //    get
        //    {
        //        if (this.totalAttack != 0xFF)
        //        {
        //            return this.totalAttack;
        //        }

        //        lock (this.recalcLocks[1])
        //        {
        //            if (this.totalAttack != 0xFF)
        //            {
        //                return this.totalAttack;
        //            }

        //            var total = this.inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Attack));

        //            this.totalAttack = total;
        //        }

        //        return this.totalAttack;
        //    }
        //}

        //public byte TotalDefense
        //{
        //    get
        //    {
        //        if (this.totalDefense != 0xFF)
        //        {
        //            return this.totalDefense;
        //        }

        //        lock (this.recalcLocks[2])
        //        {
        //            if (this.totalDefense != 0xFF)
        //            {
        //                return this.totalDefense;
        //            }

        //            var total = this.inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Defense));

        //            this.totalDefense = total;
        //        }

        //        return this.totalDefense;
        //    }
        //}

        //public byte AttackRange => 0x01;

        public bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300)
        {
            extraItem = null;

            this.inventory[this.lastPosByte++] = (item, lossProbability);

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            //if (this.inventory.ContainsKey(positionByte))
            //{
            //    var found = this.inventory[positionByte].Item1;

            //    if (found.Count < count)
            //    {
            //        return null;
            //    }

            //    // remove the whole item
            //    if (found.Count == count)
            //    {
            //        this.inventory.Remove(positionByte);
            //        found.SetHolder(null, default);

            //        return found;
            //    }

            //    IItem newItem = ItemFactory.Create(found.Type.TypeId);

            //    newItem.SetAmount(count);
            //    found.SetAmount((byte)(found.Amount - count));

            //    wasPartial = true;
            //    return newItem;
            //}

            //this.inventory = new Dictionary<byte, (IItem, ushort)>(this.inventory);

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;
            //bool removed = false;
            //var slot = this.inventory.Keys.FirstOrDefault(k => this.inventory[k].Item1.Type.TypeId == itemId);

            //try
            //{
            //    var found = this.inventory[slot].Item1;

            //    if (found.Count < count)
            //    {
            //        return null;
            //    }

            //    // remove the whole item
            //    if (found.Count == count)
            //    {
            //        this.inventory.Remove(slot);
            //        found.SetHolder(null, default);

            //        removed = true;
            //        return found;
            //    }

            //    IItem newItem = ItemFactory.Create(found.Type.TypeId);

            //    newItem.SetAmount(count);
            //    found.SetAmount((byte)(found.Amount - count));

            //    wasPartial = true;
            //    removed = true;
            //    return newItem;
            //}
            //catch
            //{
            //    return null;
            //}
            //finally
            //{
            //    if (removed)
            //    {
            //        this.inventory = new Dictionary<byte, Tuple<IItem, ushort>>(this.inventory);
            //    }
            //}
            return null;
        }

        private void DetermineLoot(IItemFactory itemFactory, IEnumerable<(ushort typeId, byte maxAmount, ushort chance)> inventoryComposition, ushort maxCapacity)
        {
            var rng = new Random();

            foreach (var (typeId, maxAmount, chance) in inventoryComposition)
            {
                if (rng.Next(1000) > chance)
                {
                    continue;
                }

                // Got lucky! This creature has this item.
                if (!(itemFactory.Create(typeId) is IItem newItem))
                {
                    Console.WriteLine($"Unknown item with id {typeId} as loot in monster type {(this.Owner as Monster)?.Type.RaceId}.");
                    continue;
                }

                if (newItem.IsCumulative)
                {
                    var amount = (byte)(rng.Next(maxAmount) + 1);

                    newItem.SetAmount(amount);
                }

                this.Add(newItem, out IItem unused);
            }
        }
    }
}