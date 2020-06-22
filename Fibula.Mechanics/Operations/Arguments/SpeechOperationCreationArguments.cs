// -----------------------------------------------------------------
// <copyright file="SpeechOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

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
        public SpeechOperationCreationArguments(uint requestorId, SpeechType speechType, ChatChannelType channelId, string content, string receiver)
        {
            this.RequestorId = requestorId;

            this.SpeechType = speechType;
            this.ChannelId = channelId;
            this.Content = content;
            this.Receiver = receiver;
        }

        public OperationType Type => OperationType.Speech;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public SpeechType SpeechType { get; }

        public ChatChannelType ChannelId { get; }

        public string Content { get; }

        public string Receiver { get; }
    }
}
