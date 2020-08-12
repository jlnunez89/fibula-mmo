// -----------------------------------------------------------------
// <copyright file="ChatChannelType.cs" company="2Dudes">
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
