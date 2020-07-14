// -----------------------------------------------------------------
// <copyright file="IMap.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Map.Contracts.Delegates;

    /// <summary>
    /// Interface for a map.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Event invoked when a window of coordinates in the map is loaded.
        /// </summary>
        event WindowLoaded WindowLoaded;

        /// <summary>
        /// Gets the reference to the selected map loader.
        /// </summary>
        IMapLoader Loader { get; }

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
