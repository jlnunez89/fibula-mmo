// -----------------------------------------------------------------
// <copyright file="PlayerInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;

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

            this.inventory = new Dictionary<Slot, IContainerItem>();

            this.SetupBodyContainers();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PlayerInventory"/> class.
        /// </summary>
        ~PlayerInventory()
        {
            foreach (Slot slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
            {
                if (slot >= Slot.Anywhere)
                {
                    continue;
                }

                this.inventory[slot].ParentContainer = null;

                this.inventory[slot].ContentAdded -= this.HandleContentAdded;
                this.inventory[slot].ContentRemoved -= this.HandleContentRemoved;
                this.inventory[slot].ContentUpdated -= this.HandleContentUpdated;
            }
        }

        ///// <summary>
        ///// A delegate to invoke when a slot in the inventory changes.
        ///// </summary>
        // public event OnInventorySlotChanged SlotChanged;

        /// <summary>
        /// Gets a reference to the owner of this inventory.
        /// </summary>
        public ICreature Owner { get; }

        /// <summary>
        /// Gets the <see cref="IContainerItem"/> at a given position of this inventory.
        /// </summary>
        /// <param name="position">The slot where to get the item from.</param>
        /// <returns>The <see cref="IContainerItem"/>, if any was found.</returns>
        public IItem this[byte position]
        {
            get
            {
                if (this.inventory.TryGetValue((Slot)position, out IContainerItem container))
                {
                    return container;
                }

                return null;
            }
        }

        /// <summary>
        /// Sets up all body containers in the invetory slots.
        /// </summary>
        private void SetupBodyContainers()
        {
            foreach (Slot slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
            {
                if (slot >= Slot.Anywhere)
                {
                    continue;
                }

                this.inventory.Add(slot, new BodyContainerItem(slot));

                this.inventory[slot].ParentContainer = this.Owner;

                this.inventory[slot].ContentAdded += this.HandleContentAdded;
                this.inventory[slot].ContentRemoved += this.HandleContentRemoved;
                this.inventory[slot].ContentUpdated += this.HandleContentUpdated;
            }
        }

        /// <summary>
        /// Handles an item being added to the given container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="addedItem">The item added.</param>
        private void HandleContentAdded(IContainerItem container, IItem addedItem)
        {
            this.InvokeSlotChanged(container as BodyContainerItem, addedItem);
        }

        /// <summary>
        /// Handles an item being removed from the given container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexRemoved">The index of the removed item.</param>
        private void HandleContentRemoved(IContainerItem container, byte indexRemoved)
        {
            this.InvokeSlotChanged(container as BodyContainerItem, null);
        }

        /// <summary>
        /// Handles an item being updated in the given container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexOfUpdated">The index of the updated item.</param>
        /// <param name="updatedItem">The item that was updated.</param>
        private void HandleContentUpdated(IContainerItem container, byte indexOfUpdated, IItem updatedItem)
        {
            this.InvokeSlotChanged(container as BodyContainerItem, container.Content.FirstOrDefault());
        }

        /// <summary>
        /// Invokes the SlotChanged event on this inventory.
        /// </summary>
        /// <param name="container">The container that changed.</param>
        /// <param name="item">The item that changed. </param>
        private void InvokeSlotChanged(BodyContainerItem container, IItem item)
        {
            if (container == null)
            {
                return;
            }

            // TODO: are all container.Slot values valid here?
            // SlotChanged?.Invoke(this, container.Slot, item);
        }
    }
}
