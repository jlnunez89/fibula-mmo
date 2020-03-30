// -----------------------------------------------------------------
// <copyright file="ISpeechInfo.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for speech information.
    /// </summary>
    public interface ISpeechInfo
    {
        /// <summary>
        /// Gets the speech type.
        /// </summary>
        SpeechType Type { get; }

        /// <summary>
        /// Gets the channel type.
        /// </summary>
        ChatChannelType ChannelId { get; }

        /// <summary>
        /// Gets the receiver of the message.
        /// </summary>
        string Receiver { get; }

        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        string Content { get; }
    }
}