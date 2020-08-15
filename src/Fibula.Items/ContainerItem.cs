// -----------------------------------------------------------------
// <copyright file="ContainerItem.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Delegates;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all container items in the game.
    /// </summary>
    public class ContainerItem : Item, IContainerItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItem"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public ContainerItem(IItemTypeEntity type)
            : base(type)
        {
            this.Content = new List<IItem>();
        }

        /// <summary>
        /// A delegate to invoke when new content is added to this container.
        /// </summary>
        public event OnContentAdded ContentAdded;

        /// <summary>
        /// A delegate to invoke when content is updated in this container.
        /// </summary>
        public event OnContentUpdated ContentUpdated;

        /// <summary>
        /// A delegate to invoke when content is removed from this container.
        /// </summary>
        public event OnContentRemoved ContentRemoved;

        /// <summary>
        /// Gets the collection of items contained in this container.
        /// </summary>
        public IList<IItem> Content { get; }

        /// <summary>
        /// Gets the capacity of this container.
        /// </summary>
        public virtual byte Capacity => Convert.ToByte(this.Attributes.ContainsKey(ItemAttribute.Capacity) ? this.Attributes[ItemAttribute.Capacity] : IContainerItem.DefaultContainerCapacity);

        /// <summary>
        /// Attempts to retrieve an item from the contents of this container based on a given index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The item retrieved, if any, or null.</returns>
        public IItem this[int index]
        {
            get
            {
                if (index >= 0 && index < this.Content.Count)
                {
                    return this.Content[index];
                }

                return null;
            }
        }

        /// <summary>
        /// Attempts to add a <see cref="IThing"/> to this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index at which to add the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to add the <see cref="IThing"/> at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public virtual (bool result, IThing remainder) AddContent(IThingFactory thingFactory, IThing thing, byte index = byte.MaxValue)
        {
            thingFactory.ThrowIfNull(nameof(thingFactory));
            thing.ThrowIfNull(nameof(thing));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            if (!(thing is IItem item))
            {
                // Containers like this can only add items.
                return (false, null);
            }

            // Validate that the item being added is not itself, or a parent of this item.
            if (thing == this || this.IsChildOf(item))
            {
                // TODO: error message 'This is impossible'.
                return (false, thing);
            }

            // Find an index which falls in within the actual content boundaries.
            var targetIndex = index < this.Content.Count ? index : -1;
            var atCapacity = this.Capacity == this.Content.Count;

            // Then get an item if there is one, at that index.
            var existingItemAtIndex = targetIndex == -1 ? null : this.Content[targetIndex];

            (bool success, IThing remainderToAdd) = (false, item);

            if (existingItemAtIndex != null)
            {
                // We matched with an item, let's attempt to add or join with it first.
                if (existingItemAtIndex.IsContainer && existingItemAtIndex is IContainerItem existingContainer)
                {
                    (success, remainderToAdd) = existingContainer.AddContent(itemFactory, remainderToAdd);
                }
                else
                {
                    (success, remainderToAdd) = existingItemAtIndex.Merge(remainderToAdd as IItem);

                    if (success)
                    {
                        // Regardless if we're done, we've changed an item at this index, so we notify observers.
                        if (remainderToAdd != null && !atCapacity)
                        {
                            targetIndex++;
                        }

                        this.InvokeContentUpdated((byte)targetIndex, existingItemAtIndex);
                    }
                }
            }

            if (remainderToAdd == null)
            {
                // If there's nothing still waiting to be added, we're done.
                return (true, null);
            }

            // Now we need to add whatever is remaining to this container.
            if (atCapacity)
            {
                // This is full.
                return (success, remainderToAdd);
            }

            remainderToAdd.ParentContainer = this;

            this.Content.Insert(0, remainderToAdd as IItem);

            this.InvokeContentAdded(remainderToAdd as IItem);

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
        public virtual (bool result, IThing remainder) RemoveContent(IThingFactory thingFactory, ref IThing thing, byte index = byte.MaxValue, byte amount = 1)
        {
            thingFactory.ThrowIfNull(nameof(thingFactory));
            thing.ThrowIfNull(nameof(thing));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            IItem existingItem = null;
            ushort thingId = thing.TypeId;

            if (index == byte.MaxValue)
            {
                existingItem = this.Content.FirstOrDefault(i => i.TypeId == thingId);
            }
            else
            {
                // Attempt to get the item at that index.
                existingItem = index >= this.Content.Count ? null : this.Content[index];
            }

            if (existingItem == null || thing.TypeId != existingItem.TypeId || existingItem.Amount < amount)
            {
                return (false, null);
            }

            if (!existingItem.IsCumulative || existingItem.Amount == amount)
            {
                // Item has the exact amount we're looking for, just remove it.
                this.Content.RemoveAt(index);
                this.InvokeContentRemoved(index);

                return (true, null);
            }

            (bool success, IItem itemProduced) = existingItem.Split(itemFactory, amount);

            if (success)
            {
                if (itemProduced != null)
                {
                    thing = itemProduced;
                }

                // We've changed an item at this index, so we notify observers.
                this.InvokeContentUpdated(index, existingItem);
            }

            return (success, existingItem);
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
            thingFactory.ThrowIfNull(nameof(thingFactory));
            fromThing.ThrowIfNull(nameof(fromThing));
            toThing.ThrowIfNull(nameof(toThing));

            if (!(thingFactory is IItemFactory itemFactory))
            {
                throw new ArgumentException($"The {nameof(thingFactory)} must be derived of type {nameof(IItemFactory)}.");
            }

            IItem existingItem = null;

            if (index == byte.MaxValue)
            {
                existingItem = this.Content.FirstOrDefault(i => i.TypeId == fromThing.TypeId);
            }
            else
            {
                // Attempt to get the item at that index.
                existingItem = index >= this.Content.Count ? null : this.Content[index];
            }

            if (existingItem == null || fromThing.TypeId != existingItem.TypeId || existingItem.Amount < amount)
            {
                return (false, null);
            }

            this.Content.RemoveAt(index);
            this.Content.Insert(index, toThing as IItem);

            toThing.ParentContainer = this;

            // We've changed an item at this index, so we notify observers.
            this.InvokeContentUpdated(index, toThing as IItem);

            return (true, null);
        }

        /// <summary>
        /// Counts the amount of the specified content item at a given index within this container.
        /// </summary>
        /// <param name="index">The index at which to count.</param>
        /// <param name="typeIdExpected">Optional. The type id of the content item expected to be found.</param>
        /// <returns>The count of the item at the index. If <paramref name="typeIdExpected"/> is specified, the value returned will only count if the type matches, otherwise -1 will be returned.</returns>
        public sbyte CountAmountAt(byte index, ushort typeIdExpected = 0)
        {
            IItem existingItem = null;

            try
            {
                existingItem = this.Content[this.Content.Count - index - 1];
            }
            catch
            {
                // ignored
            }

            if (existingItem == null)
            {
                return -1;
            }

            if (existingItem.Type.TypeId != typeIdExpected)
            {
                return 0;
            }

            return (sbyte)Math.Min(existingItem.Amount, ItemConstants.MaximumAmountOfCummulativeItems);
        }

        /// <summary>
        /// Checks that this item's parents are not this same item.
        /// </summary>
        /// <param name="item">The parent item to check.</param>
        /// <returns>True if the given item is a parent of this item, at any level of the parent hierarchy, false otherwise.</returns>
        public bool IsChildOf(IItem item)
        {
            var currentThing = this.ParentContainer as IThing;

            while (currentThing != null)
            {
                if (item == currentThing)
                {
                    return true;
                }

                currentThing = currentThing.ParentContainer as IThing;
            }

            return false;
        }

        /// <summary>
        /// Attempts to find an <see cref="IThing"/> whitin this container.
        /// </summary>
        /// <param name="index">The index at which to look for the <see cref="IThing"/>.</param>
        /// <returns>The <see cref="IThing"/> found at the index, if any was found.</returns>
        public IThing FindThingAtIndex(byte index)
        {
            return this[index];
        }

        /// <summary>
        /// Creates a new <see cref="ContainerItem"/> that is a shallow copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ContainerItem"/> that is a shallow copy of this instance.</returns>
        public override IThing Clone()
        {
            var newItem = new ContainerItem(this.Type);

            // Override the default attributes with the actual attributes this guy has.
            foreach (var (attribute, attributeValue) in this.Attributes)
            {
                newItem.Attributes[attribute] = attributeValue;
            }

            return newItem;
        }

        /// <summary>
        /// Invokes the <see cref="ContentAdded"/> event on this container.
        /// </summary>
        /// <param name="itemAdded">The item added.</param>
        protected void InvokeContentAdded(IItem itemAdded)
        {
            this.ContentAdded?.Invoke(this, itemAdded);
        }

        /// <summary>
        /// Invokes the <see cref="ContentRemoved"/> event on this container.
        /// </summary>
        /// <param name="index">The index within the container from where the item was removed.</param>
        protected void InvokeContentRemoved(byte index)
        {
            this.ContentRemoved?.Invoke(this, index);
        }

        /// <summary>
        /// Invokes the <see cref="ContentUpdated"/> event on this container.
        /// </summary>
        /// <param name="index">The index within the container from where the item was updated.</param>
        /// <param name="updatedItem">The item that was updated.</param>
        protected void InvokeContentUpdated(byte index, IItem updatedItem)
        {
            this.ContentUpdated?.Invoke(this, index, updatedItem);
        }
    }
}
