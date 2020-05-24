// -----------------------------------------------------------------
// <copyright file="CreatureSpokeNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the creature spoke notification.
    /// </summary>
    public class CreatureSpokeNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpokeNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature that turned.</param>
        /// <param name="speechType"></param>
        /// <param name="channelId"></param>
        /// <param name="text"></param>
        public CreatureSpokeNotificationArguments(ICreature creature, SpeechType speechType, ChatChannelType channelId, string text)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.SpeechType = speechType;
            this.ChannelId = channelId;
            this.Text = text;
        }

        /// <summary>
        /// Gets the creature that turned.
        /// </summary>
        public ICreature Creature { get; }

        public SpeechType SpeechType { get; internal set; }

        public string Text { get; internal set; }

        public ChatChannelType ChannelId { get; internal set; }
    }
}