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

    /// <summary>
    /// Interface for an item factory.
    /// </summary>
    public interface IItemFactory : IThingFactory
    {
        /// <summary>
        /// Creates a new <see cref="IItem"/>.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IItem"/> creation.</param>
        /// <returns>A new instance of the <see cref="IItem"/>.</returns>
        IItem CreateItem(IThingCreationArguments creationArguments);

        /// <summary>
        /// Looks up an <see cref="IItemType"/> given a type id.
        /// </summary>
        /// <param name="typeId">The id of the type to look for.</param>
        /// <returns>A reference to the <see cref="IItemType"/> found, and null if not found.</returns>
        IItemType FindTypeById(ushort typeId);
    }
}
