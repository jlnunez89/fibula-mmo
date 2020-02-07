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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;

    /// <summary>
    /// Class that represents an inventory for monsters.
    /// </summary>
    public class MonsterInventory : IInventory
    {
        /// <summary>
        /// The factor to use to calculate drop chance.
        /// </summary>
        private const int DropChanceFactor = 1000;

        /// <summary>
        /// The default maximum capacity for this inventory.
        /// </summary>
        private const int DefaultMaximumCapacity = 20;

        /// <summary>
        /// The default drop probability for an item in the inventory.
        /// </summary>
        private const int DefaultLossProbability = 1000;

        /// <summary>
        /// Internal store for the inventory.
        /// </summary>
        private readonly Dictionary<byte, (IItem, ushort)> inventory;

        /// <summary>
        /// Stores the last position byte in use for this inventory.
        /// </summary>
        private byte lastPosByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterInventory"/> class.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="owner">The owner of this inventory.</param>
        /// <param name="inventoryComposition">A collection of monster inventory posibilities, composed of typeId, maximumAmount, and chance.</param>
        /// <param name="maxCapacity">The maximum capacity of the monster's inventory.</param>
        public MonsterInventory(IItemFactory itemFactory, ICreature owner, IEnumerable<(ushort typeId, byte maxAmount, ushort chance)> inventoryComposition, ushort maxCapacity = DefaultMaximumCapacity)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            owner.ThrowIfNull(nameof(owner));

            this.lastPosByte = 0;
            this.inventory = new Dictionary<byte, (IItem, ushort)>();

            this.Owner = owner;

            this.DetermineLoot(itemFactory, inventoryComposition, maxCapacity);
        }

        /// <summary>
        /// A delegate to invoke when a slot in the inventory changes.
        /// </summary>
        public event OnInventorySlotChanged SlotChanged;

        /// <summary>
        /// Gets a reference to the owner of this inventory.
        /// </summary>
        public ICreature Owner { get; }

        /// <summary>
        /// Gets the attack range suggested by equiped weapons in this inventory.
        /// </summary>
        public byte EquipmentAttackRange => 1;

        /// <summary>
        /// Gets the attack power suggested by equiped weapons in this inventory.
        /// </summary>
        public ushort EquipmentAttackPower => 0;

        /// <summary>
        /// Gets the defense power suggested by equiped weapons in this inventory.
        /// </summary>
        public ushort EquipmentDefensePower => 0;

        /// <summary>
        /// Gets the armor rating suggested by equiped weapons in this inventory.
        /// </summary>
        public ushort EquipmentArmorRating => 0;

        /// <summary>
        /// Gets the <see cref="IItem"/> at a given position of this inventory.
        /// </summary>
        /// <param name="position">The position where to get the item from.</param>
        /// <returns>The <see cref="IItem"/>, if any was found.</returns>
        public IItem this[byte position] => this.inventory.ContainsKey(position) ? this.inventory[position].Item1 : null;

        /// <summary>
        /// Determines and sets this inventory's content (which is the loot).
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory, to create the items.</param>
        /// <param name="inventoryPossibilities">The possible inventory contents.</param>
        /// <param name="maxCapacity">A limit to the maximum number of items to create.</param>
        private void DetermineLoot(IItemFactory itemFactory, IEnumerable<(ushort typeId, byte maxAmount, ushort chance)> inventoryPossibilities, ushort maxCapacity)
        {
            var rng = new Random();

            foreach (var (typeId, maxAmount, chance) in inventoryPossibilities.OrderBy(i => i.chance))
            {
                if (maxCapacity > ushort.MinValue && this.inventory.Count == maxCapacity)
                {
                    // Reached the maximum.
                    break;
                }

                if (rng.Next(DropChanceFactor) > chance)
                {
                    continue;
                }

                // Got lucky! This creature has this item.
                if (!(itemFactory.Create(typeId) is IItem newItem))
                {
                    // TODO: propper logging.
                    Console.WriteLine($"Unknown item with id {typeId} as loot in monster type {(this.Owner as Monster)?.Type.RaceId}.");
                    continue;
                }

                if (newItem.IsCumulative)
                {
                    var amount = (byte)(rng.Next(maxAmount) + 1);

                    newItem.SetAmount(amount);
                }

                // TODO: propper logging.
                Console.WriteLine($"Added {newItem} as loot in {(this.Owner as Monster)?.Name}.");

                this.inventory[this.lastPosByte++] = (newItem, DefaultLossProbability);
            }
        }
    }
}