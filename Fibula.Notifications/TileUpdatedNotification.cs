// -----------------------------------------------------------------
// <copyright file="TileUpdatedNotification.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Notifications.Arguments;
    using Fibula.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for a tile update.
    /// </summary>
    public class TileUpdatedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatedNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public TileUpdatedNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, TileUpdatedNotificationArguments arguments)
            : base(findTargetPlayers)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public TileUpdatedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            var (descriptionMetadata, descriptionBytes) = this.Arguments.TileDescriptionFunction(player, this.Arguments.Location);

            if (descriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object creatureIdsToLearnBoxed) &&
                descriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object creatureIdsToForgetBoxed) &&
                creatureIdsToLearnBoxed is IEnumerable<uint> creatureIdsToLearn && creatureIdsToForgetBoxed is IEnumerable<uint> creatureIdsToForget)
            {
                this.Sent += (client) =>
                {
                    foreach (var creatureId in creatureIdsToLearn)
                    {
                        client.AddKnownCreature(creatureId);
                    }

                    foreach (var creatureId in creatureIdsToForget)
                    {
                        client.RemoveKnownCreature(creatureId);
                    }
                };
            }

            return new TileUpdatePacket(this.Arguments.Location, descriptionBytes).YieldSingleItem();
        }
    }
}
