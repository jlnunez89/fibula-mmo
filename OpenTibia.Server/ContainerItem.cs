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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents all container items in the game.
    /// </summary>
    public class ContainerItem : Item, IContainerItem
    {
        private readonly object openedByLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItem"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public ContainerItem(IItemType type)
            : base(type)
        {
            this.openedByLock = new object();

            this.Content = new List<IItem>();
            this.OpenedBy = new Dictionary<uint, byte>();
        }

        /// <summary>
        /// A delegate to invoke when new content is added to this container.
        /// </summary>
        public event OnContentAdded OnContentAdded;

        /// <summary>
        /// A delegate to invoke when content is updated in this container.
        /// </summary>
        public event OnContentUpdated OnContentUpdated;

        /// <summary>
        /// A delegate to invoke when content is removed from this container.
        /// </summary>
        public event OnContentRemoved OnContentRemoved;

        /// <summary>
        /// Gets the collection of items contained in this container.
        /// </summary>
        public IList<IItem> Content { get; }

        /// <summary>
        /// Gets the mapping of player ids to container ids for which this container is known to be opened.
        /// </summary>
        public IDictionary<uint, byte> OpenedBy { get; }

        /// <summary>
        /// Gets the capacity of this container.
        /// </summary>
        public byte Capacity => Convert.ToByte(this.Attributes.ContainsKey(ItemAttribute.Capacity) ? this.Attributes[ItemAttribute.Capacity] : IContainerItem.DefaultContainerCapacity);

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
        /// <param name="originalItem">The item to add.</param>
        /// <param name="index">Optional. The zero-based index at which to add the item. Defaults to 0xFF, which won't match any item.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public (bool result, IItem remainderItem) AddContent(IItemFactory itemFactory, IItem originalItem, byte index = 0xFF)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            originalItem.ThrowIfNull(nameof(originalItem));

            // Validate that the item being added is not a parent of this item.
            if (this.IsChildOf(originalItem))
            {
                // TODO: error message 'This is impossible'.
                return (false, null);
            }

            // Find an index which falls in within the actual content boundaries.
            var targetIndex = index < this.Content.Count ? index : -1;

            // Then get an item if there is one, at that index.
            var existingItemAtIndex = targetIndex == -1 ? null : this.Content[targetIndex];

            (bool success, IItem remainderItem) = (false, null);
            IItem itemToAdd = existingItemAtIndex == null ? originalItem : null;

            if (existingItemAtIndex != null)
            {
                // We matched with an item, let's attempt to add or join with it first.
                if (existingItemAtIndex.IsContainer && existingItemAtIndex is IContainerItem existingContainer)
                {
                    (success, remainderItem) = existingContainer.AddContent(itemFactory, itemToAdd);
                }
                else
                {
                    (success, remainderItem) = existingItemAtIndex.JoinWith(itemFactory, itemToAdd);

                    if (success)
                    {
                        // Regardless if we're done, we've changed an item at this index, so we notify observers.
                        this.OnContentUpdated?.Invoke(this, targetIndex);
                    }
                }

                if (success)
                {
                    // update the item to add to be the remainder only, since we've added partially at least.
                    itemToAdd = remainderItem;
                }
            }

            if (itemToAdd == null)
            {
                // If there's nothing still waiting to be added, we're done.
                return (true, null);
            }

            // Now we need to add whatever is remaining to this container.
            // Is there capacity left?
            if (this.Capacity <= this.Content.Count)
            {
                // This is full.
                return (success, itemToAdd);
            }

            this.Content.Insert(0, itemToAdd);

            this.OnContentAdded?.Invoke(this, itemToAdd);

            return (true, null);
        }

        /// <summary>
        /// Attempts to remove an item from this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="itemId">The id of the item to look for to remove.</param>
        /// <param name="index">The index at which to look for the item to remove.</param>
        /// <param name="amount">The amount of the item to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        public (bool result, IItem remainderItem) RemoveContent(IItemFactory itemFactory, ushort itemId, byte index, byte amount)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

            if (index >= this.Content.Count)
            {
                return (false, null);
            }

            // Get the item at that index.
            var existingItem = this.Content[index];

            if (existingItem.Type.TypeId != itemId || existingItem.Amount < amount)
            {
                return (false, null);
            }

            if (!existingItem.IsCumulative || existingItem.Amount == amount)
            {
                // Item has the exact amount we're looking for, just remove it.
                this.Content.RemoveAt(index);
                this.OnContentRemoved?.Invoke(this, index);

                return (true, null);
            }

            (bool success, IItem remainderItem) = existingItem.SeparateFrom(itemFactory,  amount);

            if (success)
            {
                // We've changed an item at this index, so we notify observers.
                this.OnContentUpdated?.Invoke(this, index);
            }

            return (success, remainderItem);
        }

        /// <summary>
        /// Marks this container as oppened by a creature.
        /// </summary>
        /// <param name="creatureOpeningId">The id of the creature that is opening this container.</param>
        /// <param name="containerId">The id which the creature is proposing to label this container with.</param>
        /// <returns>The id of the container which this container is or will be known to this creature.</returns>
        /// <remarks>The id returned may not match the one supplied if the container was already opened by this creature before.</remarks>
        public byte Open(uint creatureOpeningId, byte containerId)
        {
            lock (this.openedByLock)
            {
                if (!this.OpenedBy.ContainsKey(creatureOpeningId))
                {
                    this.OpenedBy.Add(creatureOpeningId, containerId);
                }

                return this.OpenedBy[creatureOpeningId];
            }
        }

        /// <summary>
        /// Marks this container as closed by a creature.
        /// </summary>
        /// <param name="creatureClosingId">The id of the creature that is closing this container.</param>
        public void Close(uint creatureClosingId)
        {
            lock (this.openedByLock)
            {
                if (this.OpenedBy.ContainsKey(creatureClosingId))
                {
                    this.OpenedBy.Remove(creatureClosingId);
                }
            }
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
        /// Retrieves the id of this container as known to a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <returns>The id of this container if it was found for the creature, -1 otherwise.</returns>
        public sbyte AsKnownTo(uint creatureId)
        {
            lock (this.openedByLock)
            {
                if (this.OpenedBy.ContainsKey(creatureId))
                {
                    return (sbyte)this.OpenedBy[creatureId];
                }
            }

            return -1;
        }

        /// <summary>
        /// Adds parsed content elements to this container.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="contentElements">The content elements to add.</param>
        protected override void AddContent(ILogger logger, IItemFactory itemFactory, IEnumerable<IParsedElement> contentElements)
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
        /// <returns>True if this item is child of any item in the parent hierarchy, false otherwise.</returns>
        private bool IsChildOf(IItem item)
        {
            if (item != null && item is IContainerItem containerItem)
            {
                while (containerItem != null)
                {
                    if (this == containerItem)
                    {
                        return true;
                    }

                    containerItem = containerItem.ParentContainer;
                }
            }

            return false;
        }
    }
}
