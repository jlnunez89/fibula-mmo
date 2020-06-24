// -----------------------------------------------------------------
// <copyright file="WorldType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible world types.
    /// </summary>
    public enum WorldType : byte
    {
        /// <summary>
        /// No PvP allowed.
        /// </summary>
        Safe,

        /// <summary>
        /// PvP is allowed but punished.
        /// </summary>
        Normal,

        /// <summary>
        /// PvP is unpunished.
        /// </summary>
        FreeForAll,

        /// <summary>
        /// PvP is encouraged.
        /// </summary>
        Hardcore,
    }
}
