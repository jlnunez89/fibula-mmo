// -----------------------------------------------------------------
// <copyright file="IItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for an item factory.
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// Creates a new <see cref="IItem"/> given the id of its type.
        /// </summary>
        /// <param name="typeId">The id of the type.</param>
        /// <returns>The new <see cref="IItem"/> instance.</returns>
        IItem Create(ushort typeId);
    }
}