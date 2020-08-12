// -----------------------------------------------------------------
// <copyright file="ItemFactory.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Items.Contracts.Extensions;

    /// <summary>
    /// Class that represents an <see cref="IItem"/> factory.
    /// </summary>
    public class ItemFactory : IItemFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFactory"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        public ItemFactory(IApplicationContext applicationContext)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));

            this.ApplicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

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

            if (itemCreationArguments.TypeId < ItemConstants.MaximumAmountOfCummulativeItems)
            {
                return null;
            }

            using var unitOfWork = this.ApplicationContext.CreateNewUnitOfWork();

            if (!(unitOfWork.ItemTypes.GetById(itemCreationArguments.TypeId.ToString()) is IItemTypeEntity itemType))
            {
                throw new ArgumentException($"Unknown item type with Id {itemCreationArguments.TypeId} in creation arguments for an item.", nameof(creationArguments));
            }

            // TODO: chest actually means a quest chest...
            if (itemType.HasItemFlag(ItemFlag.IsContainer) || itemType.HasItemFlag(ItemFlag.IsQuestChest))
            {
                return new ContainerItem(itemType);
            }

            var newItem = new Item(itemType);

            if (itemCreationArguments.Attributes != null)
            {
                foreach (var (attribute, attributeValue) in itemCreationArguments.Attributes)
                {
                    newItem.Attributes[attribute] = attributeValue;
                }
            }

            return newItem;
        }

        /// <summary>
        /// Looks up an <see cref="IItemTypeEntity"/> given a type id.
        /// </summary>
        /// <param name="typeId">The id of the type to look for.</param>
        /// <returns>A reference to the <see cref="IItemTypeEntity"/> found, and null if not found.</returns>
        public IItemTypeEntity FindTypeById(ushort typeId)
        {
            using var unitOfWork = this.ApplicationContext.CreateNewUnitOfWork();

            return unitOfWork.ItemTypes.GetById(typeId.ToString());
        }
    }
}
