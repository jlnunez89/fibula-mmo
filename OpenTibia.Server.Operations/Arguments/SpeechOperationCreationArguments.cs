// -----------------------------------------------------------------
// <copyright file="SpeechOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="SpeechOperation"/>.
    /// </summary>
    public class SpeechOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="speechType"></param>
        /// <param name="channelId"></param>
        /// <param name="receiver"></param>
        /// <param name="content"></param>
        public SpeechOperationCreationArguments(uint requestorId, SpeechType speechType, ChatChannelType channelId, string receiver, string content)
        {
            this.RequestorId = requestorId;

            this.Type = speechType;
            this.ChannelId = channelId;
            this.Receiver = receiver;
            this.Content = content;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public SpeechType Type { get; }

        public ChatChannelType ChannelId { get; }

        public string Receiver { get; }

        public string Content { get; }
    }
}
