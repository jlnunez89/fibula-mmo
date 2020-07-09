// -----------------------------------------------------------------
// <copyright file="SpeechOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="channelType">The type of channel that the speech is happens in.</param>
        /// <param name="content">The content of the speech.</param>
        /// <param name="receiver">The receiver of the speech, if applicable.</param>
        public SpeechOperationCreationArguments(uint requestorId, SpeechType speechType, ChatChannelType channelType, string content, string receiver)
        {
            this.RequestorId = requestorId;

            this.SpeechType = speechType;
            this.ChannelType = channelType;
            this.Content = content;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.Speech;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the type of speech.
        /// </summary>
        public SpeechType SpeechType { get; }

        /// <summary>
        /// Gets the type of channel of the speech.
        /// </summary>
        public ChatChannelType ChannelType { get; }

        /// <summary>
        /// Gets the content of the speech.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the receiver of the speech, if any.
        /// </summary>
        public string Receiver { get; }
    }
}
