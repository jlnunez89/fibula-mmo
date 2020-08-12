// -----------------------------------------------------------------
// <copyright file="IMonsterTypeLoader.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Interface for an <see cref="IMonsterTypeLoader"/> loader.
    /// </summary>
    public interface IMonsterTypeLoader
    {
        /// <summary>
        /// Attempts to load the monster catalog.
        /// </summary>
        /// <returns>The catalog, containing a mapping of loaded id to the monster types.</returns>
        IDictionary<ushort, IMonsterTypeEntity> LoadTypes();
    }
}
