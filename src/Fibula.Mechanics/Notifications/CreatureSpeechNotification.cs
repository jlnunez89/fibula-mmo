// -----------------------------------------------------------------
// <copyright file="CreatureSpeechNotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Notifications
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for a creature speech.
    /// </summary>
    public class CreatureSpeechNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="creature">The creature that spoke.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="channelType">The channel type.</param>
        /// <param name="text">The message content.</param>
        /// <param name="receiver">Optional. The receiver of the message.</param>
        public CreatureSpeechNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, ICreature creature, SpeechType speechType, ChatChannelType channelType, string text, string receiver = "")
            : base(findTargetPlayers)
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

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            return new CreatureSpeechPacket(
                this.Creature.Id,
                this.Creature.Name,
                this.SpeechType,
                this.Text,
                this.Creature.Location,
                this.ChannelType,
                (uint)DateTimeOffset.Now.ToUnixTimeMilliseconds()).YieldSingleItem();
        }
    }
}
