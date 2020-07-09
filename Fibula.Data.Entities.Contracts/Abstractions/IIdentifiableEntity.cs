// -----------------------------------------------------------------
// <copyright file="IIdentifiableEntity.cs" company="2Dudes">
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
    /// <summary>
    /// Interface for all entities which contain an id.
    /// </summary>
    public interface IIdentifiableEntity : IEntity
    {
        /// <summary>
        /// Gets the id of this entity.
        /// </summary>
        string Id { get; }
    }
}
