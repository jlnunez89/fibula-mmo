// -----------------------------------------------------------------
// <copyright file="IItemFactory.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Delegates;

    /// <summary>
    /// Interface for an item factory.
    /// </summary>
    public interface IItemFactory : IThingFactory
    {
        /// <summary>
        /// Event called when an item is created.
        /// </summary>
        event OnItemCreated ItemCreated;

        /// <summary>
        /// Creates a new <see cref="IItem"/>.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IItem"/> creation.</param>
        /// <returns>A new instance of the <see cref="IItem"/>.</returns>
        IItem CreateItem(IThingCreationArguments creationArguments);

        /// <summary>
        /// Looks up an <see cref="IItemTypeEntity"/> given a type id.
        /// </summary>
        /// <param name="typeId">The id of the type to look for.</param>
        /// <returns>A reference to the <see cref="IItemTypeEntity"/> found, and null if not found.</returns>
        IItemTypeEntity FindTypeById(ushort typeId);
    }
}
