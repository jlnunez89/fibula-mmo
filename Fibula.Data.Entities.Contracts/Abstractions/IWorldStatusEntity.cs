// -----------------------------------------------------------------
// <copyright file="IWorldStatusEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
