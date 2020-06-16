// -----------------------------------------------------------------
// <copyright file="TileFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Map.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the flags of a <see cref="ITile"/>.
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
