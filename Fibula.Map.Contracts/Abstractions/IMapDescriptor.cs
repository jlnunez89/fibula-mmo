// -----------------------------------------------------------------
// <copyright file="IMapDescriptor.cs" company="2Dudes">
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
    using System.Buffers;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;

    /// <summary>
    /// Interface for map descriptors.
    /// </summary>
    public interface IMapDescriptor
    {
        /// <summary>
        /// The key name for the metadata element returned for those ids of creatures to learn.
        /// </summary>
        const string CreatureIdsToLearnMetadataKeyName = "CreatureIdsToLearn";

        /// <summary>
        /// The key name for the metadata element returned for those ids of creatures to forget.
        /// </summary>
        const string CreatureIdsToForgetMetadataKeyName = "CreatureIdsToForget";

        /// <summary>
        /// Gets the description bytes of the map on behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The center location from which the description is being retrieved.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the description.</returns>
        (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeAt(IPlayer player, Location location);

        /// <summary>
        /// Gets the description bytes of the map on behalf of a given player for the specified window.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="startX">The starting X coordinate of the window.</param>
        /// <param name="startY">The starting Y coordinate of the window.</param>
        /// <param name="startZ">The starting Z coordinate of the window.</param>
        /// <param name="endZ">The ending Z coordinate of the window.</param>
        /// <param name="windowSizeX">The size of the window in X.</param>
        /// <param name="windowSizeY">The size of the window in Y.</param>
        /// <param name="startingZOffset">Optional. A starting offset for Z.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the description.</returns>
        (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeWindow(IPlayer player, ushort startX, ushort startY, sbyte startZ, sbyte endZ, byte windowSizeX = MapConstants.DefaultWindowSizeX, byte windowSizeY = MapConstants.DefaultWindowSizeY, sbyte startingZOffset = 0);

        /// <summary>
        /// Gets the description bytes of a single tile of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The location from which the description of the tile is being retrieved.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the tile's description.</returns>
        (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeTile(IPlayer player, Location location);
    }
}
