// -----------------------------------------------------------------
// <copyright file="IItemTypeEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types of items.
    /// </summary>
    public interface IItemTypeEntity : IIdentifiableEntity, ICloneable
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
        ulong Flags { get; }

        /// <summary>
        /// Gets the attributes of this type of item.
        /// </summary>
        // TODO: get rid of this and add all attributes as properties?
        IDictionary<byte, IConvertible> DefaultAttributes { get; }

        /// <summary>
        /// Gets the client id of the type of this item.
        /// </summary>
        ushort ClientId { get; }
    }
}
