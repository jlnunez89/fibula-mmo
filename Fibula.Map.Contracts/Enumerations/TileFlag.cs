// -----------------------------------------------------------------
// <copyright file="TileFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the flags of a tile.
    /// </summary>
    public enum TileFlag : byte
    {
        /// <summary>
        /// No flag.
        /// </summary>
        None = 0,

        /// <summary>
        /// A tile that reloads after some time of being "unseen".
        /// </summary>
        Refresh = 1 << 0,

        /// <summary>
        /// A tile that is considered a protection zone.
        /// </summary>
        ProtectionZone = 1 << 1,

        /// <summary>
        /// A tile in which a character is not allowed to voluntarily log out on.
        /// </summary>
        NoLogout = 1 << 2,
    }
}
