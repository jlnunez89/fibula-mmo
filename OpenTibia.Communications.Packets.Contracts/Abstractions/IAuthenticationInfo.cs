// -----------------------------------------------------------------
// <copyright file="IAuthenticationInfo.cs" company="2Dudes">
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
    /// Interface that represents authentication information.
    /// </summary>
    public interface IAuthenticationInfo
    {
        /// <summary>
        /// Gets the password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the world name.
        /// </summary>
        string WorldName { get; }
    }
}