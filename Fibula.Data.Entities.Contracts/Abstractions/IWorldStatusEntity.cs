// -----------------------------------------------------------------
// <copyright file="IWorldStatusEntity.cs" company="2Dudes">
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

    /// <summary>
    /// Interface for a world status entry entity.
    /// </summary>
    public interface IWorldStatusEntity : IEntity
    {
        /// <summary>
        /// Gets the name of the world this entry is for.
        /// </summary>
        string World { get; }

        /// <summary>
        /// Gets the last time that this entry was updated.
        /// </summary>
        DateTimeOffset LastUpdated { get; }

        /// <summary>
        /// Gets the number of players online in this world.
        /// </summary>
        ushort PlayersOnline { get; }

        /// <summary>
        /// Gets the peak number of players ever observed in this world.
        /// </summary>
        ushort RecordOnline { get; }
    }
}
