// -----------------------------------------------------------------
// <copyright file="ItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an <see cref="IItem"/> factory.
    /// </summary>
    public class ItemFactory : IItemFactory
    {
        /// <summary>
        /// Gets a reference to the item types catalog to use in this factory.
        /// </summary>
        private readonly IDictionary<ushort, IItemType> itemTypesCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFactory"/> class.
        /// </summary>
        /// <param name="itemLoader">A reference to the item type loader to use.</param>
        public ItemFactory(IItemTypeLoader itemLoader)
        {
            itemLoader.ThrowIfNull(nameof(itemLoader));

            this.ItemTypeLoader = itemLoader;

            this.itemTypesCatalog = this.ItemTypeLoader.LoadTypes();
        }

        /// <summary>
        /// Gets the reference to the item type loader in use.
        /// </summary>
        public IItemTypeLoader ItemTypeLoader { get; }

        /// <summary>
        /// Creates a new <see cref="IThing"/>.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IThing"/> creation.</param>
        /// <returns>A new instance of the <see cref="IThing"/>.</returns>
        public IThing Create(IThingCreationArguments creationArguments)
        {
            return this.CreateItem(creationArguments);
        }

        /// <summary>
        /// Creates a new <see cref="IItem"/> given the id of its type.
        /// </summary>
        /// <param name="creationArguments">The id of the type.</param>
        /// <returns>The new <see cref="IItem"/> instance.</returns>
        public IItem CreateItem(IThingCreationArguments creationArguments)
        {
            if (!(creationArguments is ItemCreationArguments itemCreationArguments))
            {
                throw new ArgumentException($"Invalid type of arguments '{creationArguments.GetType().Name}' supplied, expected {nameof(ItemCreationArguments)}", nameof(creationArguments));
            }

            if (itemCreationArguments.TypeId < ItemConstants.MaximumAmountOfCummulativeItems || !this.itemTypesCatalog.ContainsKey(itemCreationArguments.TypeId))
            {
                return null;
            }

            // TODO: chest actually means a quest chest...
            if (this.itemTypesCatalog[itemCreationArguments.TypeId].Flags.Contains(ItemFlag.Container) || this.itemTypesCatalog[itemCreationArguments.TypeId].Flags.Contains(ItemFlag.Chest))
            {
                return new ContainerItem(this.itemTypesCatalog[itemCreationArguments.TypeId]);
            }

            return new Item(this.itemTypesCatalog[itemCreationArguments.TypeId]);
        }
    }
}
