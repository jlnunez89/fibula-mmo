// -----------------------------------------------------------------
// <copyright file="WorldType.cs" company="2Dudes">
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
