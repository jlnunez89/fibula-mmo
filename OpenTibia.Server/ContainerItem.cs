// -----------------------------------------------------------------
// <copyright file="ContainerItem.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents all container items in the game.
    /// </summary>
    public class ContainerItem : Item, IContainerItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItem"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public ContainerItem(IItemType type)
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
        /// Attempts to add an item to this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="thing">The thing to add to the cylinder, which must be an <see cref="IItem"/>.</param>
        /// <param name="index">Optional. The index at which to add the thing. Defaults to 0xFF, which instructs to add the thing at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public virtual (bool result, IThing remainder) AddContent(IItemFactory itemFactory, IThing thing, byte index = 0xFF)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            thing.ThrowIfNull(nameof(thing));

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

            remainderToAdd.ParentCylinder = this;

            this.Content.Insert(0, remainderToAdd as IItem);

            this.InvokeContentAdded(remainderToAdd as IItem);

            return (true, null);
        }

        /// <summary>
        /// Attempts to remove an item from this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="thing">The thing to remove from the cylinder, which must be an <see cref="IItem"/>.</param>
        /// <param name="index">Optional. The index from which to remove the thing. Defaults to 0xFF, which instructs to remove the thing if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public virtual (bool result, IThing remainder) RemoveContent(IItemFactory itemFactory, ref IThing thing, byte index = 0xFF, byte amount = 1)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            thing.ThrowIfNull(nameof(thing));

            IItem existingItem = null;
            ushort thingId = thing.ThingId;

            if (index == 0xFF)
            {
                existingItem = this.Content.FirstOrDefault(i => i.ThingId == thingId);
            }
            else
            {
                // Attempt to get the item at that index.
                existingItem = index >= this.Content.Count ? null : this.Content[index];
            }

            if (existingItem == null || thing.ThingId != existingItem.ThingId || existingItem.Amount < amount)
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
        /// Attempts to replace a thing from this cylinder with another.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="fromThing">The thing to remove from the cylinder.</param>
        /// <param name="toThing">The thing to add to the cylinder.</param>
        /// <param name="index">Optional. The index from which to replace the thing. Defaults to 0xFF, which instructs to replace the thing if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="fromThing"/> to replace.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public (bool result, IThing remainderToChange) ReplaceContent(IItemFactory itemFactory, IThing fromThing, IThing toThing, byte index = 255, byte amount = 1)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            fromThing.ThrowIfNull(nameof(fromThing));
            toThing.ThrowIfNull(nameof(toThing));

            IItem existingItem = null;

            if (index == 0xFF)
            {
                existingItem = this.Content.FirstOrDefault(i => i.ThingId == fromThing.ThingId);
            }
            else
            {
                // Attempt to get the item at that index.
                existingItem = index >= this.Content.Count ? null : this.Content[index];
            }

            if (existingItem == null || fromThing.ThingId != existingItem.ThingId || existingItem.Amount < amount)
            {
                return (false, null);
            }

            this.Content.RemoveAt(index);
            this.Content.Insert(index, toThing as IItem);

            toThing.ParentCylinder = this;

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

            return (sbyte)Math.Min(existingItem.Amount, (byte)100);
        }

        /// <summary>
        /// Adds parsed content elements to this container.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="contentElements">The content elements to add.</param>
        public void AddContent(ILogger logger, IItemFactory itemFactory, IEnumerable<IParsedElement> contentElements)
        {
            logger.ThrowIfNull(nameof(logger));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            contentElements.ThrowIfNull(nameof(contentElements));

            foreach (var element in contentElements)
            {
                if (element.IsFlag)
                {
                    // A flag is unexpected in this context.
                    logger.Warning($"Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");

                    continue;
                }

                IItem item = itemFactory.Create((ushort)element.Id);

                if (item == null)
                {
                    logger.Warning($"Item with id {element.Id} not found in the catalog, skipping.");

                    continue;
                }

                item.SetAttributes(logger.ForContext<IItem>(), itemFactory, element.Attributes);

                // TODO: we should be able to go over capacity here.
                this.AddContent(itemFactory, item, 0xFF);
            }
        }

        /// <summary>
        /// Checks that this item's parents are not this same item.
        /// </summary>
        /// <param name="item">The parent item to check.</param>
        /// <returns>True if the given item is a parent of this item, at any level of the parent hierarchy, false otherwise.</returns>
        public bool IsChildOf(IItem item)
        {
            var current = this.ParentCylinder;

            while (current != null)
            {
                if (item == current)
                {
                    return true;
                }

                current = current.ParentCylinder;
            }

            return false;
        }

        /// <summary>
        /// Gets an item within this cylinder.
        /// </summary>
        /// <param name="index">The index at which to get the item.</param>
        /// <returns>The item found at the index, if there is any.</returns>
        public IItem FindItemAt(byte index)
        {
            return this[index];
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
