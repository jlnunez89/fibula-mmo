// -----------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities
{
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Abstract class that represents the base of all entities.
    /// </summary>
    public abstract class BaseEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets or sets the id of this entity.
        /// </summary>
        public string Id { get; set; }
    }
}
