// -----------------------------------------------------------------
// <copyright file="IContainedThing.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all entities that keep track of a parent container.
    /// </summary>
    public interface IContainedThing
    {
        /// <summary>
        /// Gets or sets the parent container of this entity.
        /// </summary>
        IThingContainer ParentContainer { get; set; }
    }
}
