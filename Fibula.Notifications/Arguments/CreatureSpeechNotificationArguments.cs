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

namespace Fibula.Server.Notifications.Arguments
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the creature speech notification.
    /// </summary>
    public class CreatureSpeechNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature that spoke.</param>
        /// <param name="speechType"></param>
        /// <param name="channelId"></param>
        /// <param name="text"></param>
        /// <param name="receiver"></param>
        public CreatureSpeechNotificationArguments(ICreature creature, SpeechType speechType, ChatChannelType channelId, string text, string receiver = "")
        {
            this.Creature = creature;
            this.SpeechType = speechType;
            this.ChannelId = channelId;
            this.Text = text;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the id of the creature that turned.
        /// </summary>
        public ICreature Creature { get; }

        public SpeechType SpeechType { get; internal set; }

        public string Text { get; internal set; }

        public string Receiver { get; }

        public ChatChannelType ChannelId { get; internal set; }
    }
}