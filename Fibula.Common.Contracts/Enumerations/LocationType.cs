// -----------------------------------------------------------------
// <copyright file="LocationType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible types of locations.
    /// </summary>
    public enum LocationType : byte
    {
        /// <summary>
        /// Unset and generally invalid location.
        /// </summary>
        NotSet,

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
