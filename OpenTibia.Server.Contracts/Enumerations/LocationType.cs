// -----------------------------------------------------------------
// <copyright file="LocationType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible types of locations.
    /// </summary>
    public enum LocationType : byte
    {
        /// <summary>
        /// Inside a specific container.
        /// </summary>
        InsideContainer,

        /// <summary>
        /// The inventory slot of a player.
        /// </summary>
        InventorySlot,

        /// <summary>
        /// On the map.
        /// </summary>
        Map,
    }
}
