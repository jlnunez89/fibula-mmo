// -----------------------------------------------------------------
// <copyright file="CreatureSpeechNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Arguments
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the creature speech notification.
    /// </summary>
    public class CreatureSpeechNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature that spoke.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="channelType">The channel type.</param>
        /// <param name="text">The message content.</param>
        /// <param name="receiver">Optional. The receiver of the message.</param>
        public CreatureSpeechNotificationArguments(ICreature creature, SpeechType speechType, ChatChannelType channelType, string text, string receiver = "")
        {
            this.Creature = creature;
            this.SpeechType = speechType;
            this.ChannelType = channelType;
            this.Text = text;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the id of the creature that spoke.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the type of speech.
        /// </summary>
        public SpeechType SpeechType { get; }

        /// <summary>
        /// Gets the type of channel.
        /// </summary>
        public ChatChannelType ChannelType { get; }

        /// <summary>
        /// Gets the text of the speech.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the receiver of the speech, if this is a private channel message.
        /// </summary>
        public string Receiver { get; }
    }
}