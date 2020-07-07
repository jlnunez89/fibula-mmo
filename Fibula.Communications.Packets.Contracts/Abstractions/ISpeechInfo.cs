// -----------------------------------------------------------------
// <copyright file="ISpeechInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for speech information.
    /// </summary>
    public interface ISpeechInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the speech type.
        /// </summary>
        SpeechType SpeechType { get; }

        /// <summary>
        /// Gets the channel type.
        /// </summary>
        ChatChannelType ChannelType { get; }

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
