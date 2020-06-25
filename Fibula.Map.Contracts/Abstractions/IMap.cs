// -----------------------------------------------------------------
// <copyright file="IMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Interface for a map.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Attempts to get a <see cref="ITile"/> at a given <see cref="Location"/>, if any.
        /// </summary>
        /// <param name="location">The location to get the file from.</param>
        /// <param name="tile">A reference to the <see cref="ITile"/> found, if any.</param>
        /// <param name="loadAsNeeded">Optional. A value indicating whether to attempt to load tiles if the loader hasn't loaded them yet.</param>
        /// <returns>A value indicating whether a <see cref="ITile"/> was found, false otherwise.</returns>
        bool GetTileAt(Location location, out ITile tile, bool loadAsNeeded = true);

        /// <summary>
        /// Attempts to get a <see cref="ITile"/> at a given <see cref="Location"/>, if any.
        /// </summary>
        /// <param name="location">The location to get the file from.</param>
        /// <returns>A reference to the <see cref="ITile"/> found, if any.</returns>
        ITile GetTileAt(Location location);
    }
}