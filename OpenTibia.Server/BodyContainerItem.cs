// -----------------------------------------------------------------
// <copyright file="BodyContainerItem.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Items;

    /// <summary>
    /// Class that represents a container item for body slots.
    /// </summary>
    public class BodyContainerItem : ContainerItem
    {
        /// <summary>
        /// Stores this body container's location.
        /// </summary>
        private Location location;

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyContainerItem"/> class.
        /// </summary>
        /// <param name="slot">The slot at which this body container is.</param>
        public BodyContainerItem(Slot slot)
            : base(new ItemType())
        {
            this.Slot = slot;

            this.location = new Location()
            {
                X = Location.NonMapLocationX,
                Y = (byte)this.Slot,
            };
        }

        /// <summary>
        /// Gets the capacity of this container.
        /// </summary>
        public override byte Capacity => 0x01;

        /// <summary>
        /// Gets this body container's slot.
        /// </summary>
        public Slot Slot { get; }

        /// <summary>
        /// Gets this body container's location.
        /// </summary>
        public override Location Location
        {
            get
            {
                return this.ParentCylinder?.Location ?? this.location;
            }
        }

        /// <summary>
        /// Gets the location where this thing is being carried at, which is none for creatures.
        /// </summary>
        public override Location? CarryLocation
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Attempts to add an item to this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="thing">The thing to add to the cylinder, which must be an <see cref="IItem"/>.</param>
        /// <param name="index">Optional. The index at which to add the thing. Defaults to 0xFF, which instructs to add the thing at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public override (bool result, IThing remainder) AddContent(IItemFactory itemFactory, IThing thing, byte index = 0xFF)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            thing.ThrowIfNull(nameof(thing));

            if (!(thing is IItem item))
            {
                // Containers like this can only add items.
                return (false, null);
            }

            // Validate that the item being added is not a parent of this item.
            if (this.IsChildOf(item) || !this.CanDressItemHere(item))
            {
                // TODO: error message 'This is impossible'.
                return (false, thing);
            }

            // Find an index which falls in within the actual content boundaries.
            var targetIndex = index < this.Content.Count ? index : -1;

            // Then get an item if there is one, at that index.
            var existingItemAtIndex = targetIndex == -1 ? this.Content.FirstOrDefault() : this.Content[targetIndex];

            (bool success, IThing remainderItem) = (false, item);

            if (existingItemAtIndex != null)
            {
                // We matched with an item, let's attempt to add or join with it first.
                if (existingItemAtIndex.IsContainer && existingItemAtIndex is IContainerItem existingContainer)
                {
                    return existingContainer.AddContent(itemFactory, remainderItem);
                }
                else
                {
                    (success, remainderItem) = existingItemAtIndex.Merge(remainderItem as IItem);

                    if (success)
                    {
                        // Regardless if we're done, we've changed an item at this index, so we notify observers.
                        this.InvokeContentUpdated((byte)targetIndex, remainderItem as IItem);
                    }
                }
            }

            if (success)
            {
                // If we have partially succeeded, we need to return now.
                return (true, remainderItem);
            }

            if (existingItemAtIndex == null)
            {
                remainderItem.ParentCylinder = this;

                this.Content.Insert(0, remainderItem as IItem);

                this.InvokeContentAdded(remainderItem as IItem);

                remainderItem = null;
            }
            else
            {
                // Swap the items.
                this.Content.Clear();

                remainderItem.ParentCylinder = this;

                this.Content.Insert(0, remainderItem as IItem);
                this.InvokeContentUpdated(0, remainderItem as IItem);

                remainderItem = existingItemAtIndex;
            }

            return (true, remainderItem);
        }

        /// <summary>
        /// Checks if the given thing can be dressed in this body container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the thing can be dressed in this container, false otherwise.</returns>
        private bool CanDressItemHere(IItem item)
        {
            if (item == null || !(this.ParentCylinder is IPlayer player))
            {
                return false;
            }

            switch (this.Slot)
            {
                // Not valid targets
                default:
                case Slot.UnsetInvalid:
                case Slot.Anywhere:
                    return false;

                // Valid target, wildcard slot
                case Slot.Ammo:
                    return true;

                // Valid target, straightforward slots
                case Slot.Head:
                case Slot.Neck:
                case Slot.Back:
                case Slot.Body:
                case Slot.Legs:
                case Slot.Ring:
                case Slot.Feet:
                    return item.CanBeDressed && item.DressPosition == this.Slot;

                // Valid target, special slots
                case Slot.LeftHand:
                    if (!(player.Inventory[(byte)Slot.RightHand] is IContainerItem rightHandContainer))
                    {
                        return false;
                    }

                    var rightHandItem = rightHandContainer.Content.FirstOrDefault();

                    return rightHandItem == null || (item.DressPosition != Slot.TwoHanded && rightHandItem.DressPosition != Slot.TwoHanded);

                case Slot.RightHand:
                    if (!(player.Inventory[(byte)Slot.LeftHand] is IContainerItem leftHandContainer))
                    {
                        return false;
                    }

                    var leftHandItem = leftHandContainer.Content.FirstOrDefault();

                    return leftHandItem == null || (item.DressPosition != Slot.TwoHanded && leftHandItem.DressPosition != Slot.TwoHanded);
            }
        }
    }
}
