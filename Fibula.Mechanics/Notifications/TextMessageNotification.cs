// -----------------------------------------------------------------
// <copyright file="TextMessageNotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for text messages.
    /// </summary>
    public class TextMessageNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="type">The type of text to send.</param>
        /// <param name="message">The text to send.</param>
        public TextMessageNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, MessageType type, string message)
            : base(findTargetPlayers)
        {
            this.Type = type;
            this.Text = message;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the text message type.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Gets the text value.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            return new TextMessagePacket(this.Type, this.Text).YieldSingleItem();
        }
    }
}
