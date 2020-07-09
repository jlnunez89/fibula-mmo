// -----------------------------------------------------------------
// <copyright file="ILocatable.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Interface for all entities with a location.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Gets this entity's location.
        /// </summary>
        Location Location { get; }

        /// <summary>
        /// Gets the location where this entity is being carried at, if any.
        /// </summary>
        Location? CarryLocation { get; }
    }
}
