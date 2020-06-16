// -----------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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