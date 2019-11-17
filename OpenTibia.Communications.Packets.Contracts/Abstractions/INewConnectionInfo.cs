// -----------------------------------------------------------------
// <copyright file="INewConnectionInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Interface for new connection information.
    /// </summary>
    public interface INewConnectionInfo
    {
        /// <summary>
        /// Gets the operating system of the client.
        /// </summary>
        ushort Os { get; }

        /// <summary>
        /// Gets the version of the client.
        /// </summary>
        ushort Version { get; }
    }
}
