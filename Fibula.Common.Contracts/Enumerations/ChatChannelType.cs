// -----------------------------------------------------------------
// <copyright file="ChatChannelType.cs" company="2Dudes">
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
    /// Enumerates the chat channel types.
    /// </summary>
    public enum ChatChannelType : ushort
    {
        /// <summary>
        /// The rule violations channel.
        /// </summary>
        RuleViolations = 0x03,

        /// <summary>
        /// The default game channel.
        /// </summary>
        Game = 0x04,

        /// <summary>
        /// The trade channel.
        /// </summary>
        Trade = 0x05,

        /// <summary>
        /// The real-life channel.
        /// </summary>
        RealLife = 0x06,

        /// <summary>
        /// The help channel.
        /// </summary>
        Help = 0x08,

        /// <summary>
        /// A private chat channel.
        /// </summary>
        Private = 0xFFFF,

        /// <summary>
        /// No channel type.
        /// </summary>
        None = 0xAAAA,
    }
}
