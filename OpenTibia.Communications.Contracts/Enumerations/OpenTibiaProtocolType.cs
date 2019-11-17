// -----------------------------------------------------------------
// <copyright file="OpenTibiaProtocolType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different open tibia protocol types.
    /// </summary>
    public enum OpenTibiaProtocolType
    {
        /// <summary>
        /// Represents the login protocol.
        /// </summary>
        LoginProtocol,

        /// <summary>
        /// Represents the game protocol.
        /// </summary>
        GameProtocol,

        /// <summary>
        /// Represents the management protocol.
        /// </summary>
        ManagementProtocol,
    }
}
