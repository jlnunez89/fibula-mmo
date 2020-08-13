// -----------------------------------------------------------------
// <copyright file="ISpeechInfo.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Interface for speech information.
    /// </summary>
    public interface ISpeechInfo
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
