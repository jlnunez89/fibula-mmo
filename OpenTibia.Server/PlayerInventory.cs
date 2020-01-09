// -----------------------------------------------------------------
// <copyright file="PlayerInventory.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an inventory for players.
    /// </summary>
    internal class PlayerInventory : IInventory
    {
        /// <summary>
        /// Holds the inventory.
        /// </summary>
        private readonly Dictionary<Slot, IContainerItem> inventory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventory"/> class.
        /// </summary>
        /// <param name="owner">A reference to the owner of this inventory.</param>
        public PlayerInventory(ICreature owner)
        {
            owner.ThrowIfNull(nameof(owner));

            this.Owner = owner;

            this.inventory = new Dictionary<Slot, IContainerItem>(); ;

            this.SetupBodyContainers();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PlayerInventory"/> class.
        /// </summary>
        ~PlayerInventory()
        {
            foreach (Slot slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
            {
                this.inventory[slot].ParentCylinder = null;

                this.inventory[slot].OnContentAdded -= this.HandleContentAdded;
                this.inventory[slot].OnContentRemoved -= this.HandleContentRemoved;
                this.inventory[slot].OnContentUpdated -= this.HandleContentUpdated;
            }
        }

        /// <summary>
        /// A delegate to invoke when a slot in the inventory changes.
        /// </summary>
        public event OnInventorySlotChanged OnSlotChanged;

        /// <summary>
        /// Gets a reference to the owner of this inventory.
        /// </summary>
        public ICreature Owner { get; }

        /// <summary>
        /// Gets the <see cref="IContainerItem"/> at a given position of this inventory.
        /// </summary>
        /// <param name="slot">The slot where to get the item from.</param>
        /// <returns>The <see cref="IContainerItem"/>, if any was found.</returns>
        public IItem this[Slot slot]
        {
            get
            {
                if (this.inventory.TryGetValue(slot, out IContainerItem container))
                {
                    return container;
                }

                return null;
            }
        }

        // public byte TotalAttack => (byte)Math.Max(this.Inventory.ContainsKey(Slot.LeftHand) ? this.Inventory[Slot.LeftHand].Item1.Attack : 0, this.Inventory.ContainsKey(Slot.RightHand) ? this.Inventory[Slot.RightHand].Item1.Attack : 0);

        // public byte TotalDefense => (byte)Math.Max(this.Inventory.ContainsKey(Slot.LeftHand) ? this.Inventory[Slot.LeftHand].Item1.Defense : 0, this.Inventory.ContainsKey(Slot.RightHand) ? this.Inventory[Slot.RightHand].Item1.Defense : 0);

        // public byte TotalArmor
        // {
        //    get
        //    {
        //        byte totalArmor = 0;

        // totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Neck) ? this.Inventory[Slot.Neck].Item1.Armor : 0);
        //        totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Head) ? this.Inventory[Slot.Head].Item1.Armor : 0);
        //        totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Body) ? this.Inventory[Slot.Body].Item1.Armor : 0);
        //        totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Legs) ? this.Inventory[Slot.Legs].Item1.Armor : 0);
        //        totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Feet) ? this.Inventory[Slot.Feet].Item1.Armor : 0);
        //        totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Ring) ? this.Inventory[Slot.Ring].Item1.Armor : 0);

        // return totalArmor;
        //    }
        // }

        // public byte AttackRange => (byte)Math.Max(
        //    Math.Max(
        //    this.Inventory.ContainsKey(Slot.LeftHand) ? this.Inventory[Slot.LeftHand].Item1.Range : 0,
        //    this.Inventory.ContainsKey(Slot.RightHand) ? this.Inventory[Slot.RightHand].Item1.Range : 0),
        //    this.Inventory.ContainsKey(Slot.TwoHanded) ? this.Inventory[Slot.TwoHanded].Item1.Range : 0);

        public bool Add(IItem item, out IItem extraItem, byte positionByte, byte count = 1, ushort lossProbability = 300)
        {
            // if (count == 0 || count > 100)
            // {
            //    throw new ArgumentException($"Invalid count {count}.", nameof(count));
            // }

            extraItem = null;

            // if (item == null || positionByte > (byte)Slot.Anywhere)
            // {
            //    return false;
            // }

            // var targetSlot = (Slot)positionByte;

            //// TODO: check dress positions here.

            //// if (targetSlot != Slot.Right && targetSlot != Slot.Left && targetSlot != Slot.WhereEver)
            //// {

            //// }
            // try
            // {
            //    var current = this.inventory[targetSlot];

            // if (current != null)
            //    {
            //        var joinResult = current.Item1.Join(item);

            // // update the added item in the slot.
            //        Game.Instance.NotifySinglePlayer(this.Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = current.Item1 }));

            // if (joinResult || current.Item1.IsContainer)
            //        {
            //            return joinResult;
            //        }

            // // exchange items
            //        if (current.Item1.IsCumulative && item.Type.TypeId == current.Item1.Type.TypeId && current.Item1.Count == 100)
            //        {
            //            extraItem = item;
            //            item = current.Item1;
            //        }
            //        else
            //        {
            //            extraItem = current.Item1;
            //            extraItem.SetHolder(null, default);
            //        }
            //    }
            // }
            // catch
            // {
            //    // ignored
            // }

            //// set the item in place.
            // this.inventory[targetSlot] = new Tuple<IItem, ushort>(item, item.IsContainer ? (ushort)1000 : lossProbability);

            // item.SetHolder(this.Owner, new Location { X = 0xFFFF, Y = 0, Z = (sbyte)targetSlot });

            //// update the added item in the slot.
            // Game.Instance.NotifySinglePlayer(this.Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = item }));

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            // if (positionByte == (byte)Slot.TwoHanded || positionByte == (byte)Slot.Anywhere)
            // {
            //    return null;
            // }

            // if (this.inventory.ContainsKey((Slot)positionByte))
            // {
            //    var found = this.Inventory[(Slot)positionByte].Item1;

            // if (found.Count < count)
            //    {
            //        return null;
            //    }

            // // remove the whole item
            //    if (found.Count == count)
            //    {
            //        this.Inventory.Remove((Slot)positionByte);
            //        found.SetHolder(null, default);

            // // update the slot.
            //        Game.Instance.NotifySinglePlayer(
            //            this.Owner as IPlayer,
            //            conn => new GenericNotification(
            //                conn,
            //                new InventoryClearSlotPacket { Slot = (Slot)positionByte }));

            // return found;
            //    }

            // IItem newItem = ItemFactory.Create(found.Type.TypeId);

            // newItem.SetAmount(count);
            //    found.SetAmount((byte)(found.Amount - count));

            // // update the remaining item in the slot.
            //    Game.Instance.NotifySinglePlayer(
            //        this.Owner as IPlayer,
            //        conn => new GenericNotification(
            //            conn,
            //            new InventorySetSlotPacket { Slot = (Slot)positionByte, Item = found }));

            // wasPartial = true;

            // return newItem;
            // }

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;

            // var slot = this.inventory.Keys.FirstOrDefault(k => this.inventory[k].Item1.Type.TypeId == itemId);

            // if (slot != default)
            // {
            //    var found = this.inventory[slot].Item1;

            // if (found.Count < count)
            //    {
            //        return null;
            //    }

            // // remove the whole item
            //    if (found.Count == count)
            //    {
            //        this.Inventory.Remove(slot);
            //        found.SetHolder(null, default);

            // // update the slot.
            //        Game.Instance.NotifySinglePlayer(
            //            this.Owner as IPlayer,
            //            conn => new GenericNotification(
            //                conn,
            //                new InventoryClearSlotPacket { Slot = slot }));

            // return found;
            //    }

            // IItem newItem = ItemFactory.Create(found.Type.TypeId);

            // newItem.SetAmount(count);
            //    found.SetAmount((byte)(found.Amount - count));

            // // update the remaining item in the slot.
            //    Game.Instance.NotifySinglePlayer(
            //        this.Owner as IPlayer,
            //        conn => new GenericNotification(
            //            conn,
            //            new InventorySetSlotPacket { Slot = slot, Item = found }));

            // wasPartial = true;

            // return newItem;
            // }

            // TODO: exhaustive search of container items here.
            return null;
        }

        private void SetupBodyContainers()
        {
            foreach (Slot slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
            {
                if (slot >= Slot.Anywhere)
                {
                    continue;
                }

                this.inventory.Add(slot, new BodyContainerItem(slot));

                this.inventory[slot].ParentCylinder = this.Owner;

                this.inventory[slot].OnContentAdded += this.HandleContentAdded;
                this.inventory[slot].OnContentRemoved += this.HandleContentRemoved;
                this.inventory[slot].OnContentUpdated += this.HandleContentUpdated;
            }
        }

        private void HandleContentAdded(IContainerItem container, IItem addedItem)
        {
            this.InvokeSlotChanged(container, addedItem);
        }

        private void HandleContentRemoved(IContainerItem container, byte indexRemoved)
        {
            this.InvokeSlotChanged(container, null);
        }

        private void HandleContentUpdated(IContainerItem container, byte indexOfUpdated, IItem updatedItem)
        {
            this.InvokeSlotChanged(container, container.Content.FirstOrDefault());
        }

        private void InvokeSlotChanged(IContainerItem container, IItem item)
        {
            container.ThrowIfNull(nameof(container));

            foreach (Slot slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
            {
                if (slot >= Slot.Anywhere)
                {
                    continue;
                }

                if (container == this.inventory[slot])
                {
                    this.OnSlotChanged?.Invoke(this, slot, item);
                }
            }
        }
    }
}
