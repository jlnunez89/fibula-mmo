// -----------------------------------------------------------------
// <copyright file="IItemType.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for types of items.
    /// </summary>
    public interface IItemType
    {
        /// <summary>
        /// Gets the id of the type of this item.
        /// </summary>
        ushort TypeId { get; }

        /// <summary>
        /// Gets the name of this type of item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the decription of this type of item.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the flags for this type of item.
        /// </summary>
        ISet<ItemFlag> Flags { get; }

        /// <summary>
        /// Gets the attributes of this type of item.
        /// </summary>
        // TODO: get rid of this and add all attributes as properties.
        IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; }

        /// <summary>
        /// Gets the client id of the type of this item.
        /// </summary>
        ushort ClientId { get; }
    }
}
