// -----------------------------------------------------------------
// <copyright file="LogInOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents a login operation.
    /// </summary>
    public class LogInOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the action.</param>
        /// <param name="client">The client requesting the log in.</param>
        /// <param name="playerMetadata">The creation metadata of the player that is logging in.</param>
        /// <param name="worldLightLevel">The level of the world light to send to the player.</param>
        /// <param name="worldLightColor">The color of the world light to send to the player.</param>
        public LogInOperation(uint requestorId, IClient client, ICreatureCreationMetadata playerMetadata, byte worldLightLevel, byte worldLightColor)
            : base(requestorId)
        {
            this.Client = client;
            this.CurrentWorldLightLevel = worldLightLevel;
            this.CurrentWorldLightColor = worldLightColor;

            this.PlayerMetadata = playerMetadata;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets the client requesting the log in.
        /// </summary>
        public IClient Client { get; }

        /// <summary>
        /// Gets the current light level of the world, to send with the login information.
        /// </summary>
        public byte CurrentWorldLightLevel { get; }

        /// <summary>
        /// Gets the current light color of the world, to send with the login information.
        /// </summary>
        public byte CurrentWorldLightColor { get; }

        /// <summary>
        /// Gets the player metadata.
        /// </summary>
        public ICreatureCreationMetadata PlayerMetadata { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            // This will eventually come from the character, or fall back.
            var targetLoginLocation = MapConstants.ThaisTempleMark;

            var creationArguments = new PlayerCreationArguments()
            {
                Client = this.Client,
                Type = CreatureType.Player,
                Metadata = this.PlayerMetadata,
            };

            if (!(context.CreatureFactory.CreateCreature(creationArguments) is IPlayer player))
            {
                context.Logger.Warning($"Unable to create player instance for {this.PlayerMetadata.Name}, aborting log in.");

                return;
            }

            if (!context.Map.GetTileAt(targetLoginLocation, out ITile targetTile) || !this.PlaceCreature(context, targetTile, player))
            {
                // Unable to place the player in the map.
                context.Scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => player.YieldSingleItem(),
                        new GenericNotificationArguments(
                            new GameServerDisconnectPacket("Your character could not be placed on the map.\nPlease try again, or contact an administrator if the issue persists."))));

                return;
            }

            var (descriptionMetadata, descriptionBytes) = context.MapDescriptor.DescribeAt(player, player.Location);

            // TODO: In addition, we need to send the player's inventory, the first time login message + outfit window here if applicable.
            // And any VIP records here.
            var notification = new GenericNotification(
                () => player.YieldSingleItem(),
                new GenericNotificationArguments(
                    new PlayerLoginPacket(player.Id, player),
                    new MapDescriptionPacket(player.Location, descriptionBytes),
                    new MagicEffectPacket(player.Location, AnimatedEffect.BubbleBlue),
                    new PlayerStatsPacket(player),
                    new PlayerSkillsPacket(player),
                    new WorldLightPacket(this.CurrentWorldLightLevel, this.CurrentWorldLightColor),
                    new CreatureLightPacket(player),
                    new TextMessagePacket(MessageType.StatusDefault, "This is a test message"),
                    new PlayerConditionsPacket(player)));

            if (descriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object creatureIdsToLearnBoxed) &&
                descriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object creatureIdsToForgetBoxed) &&
                creatureIdsToLearnBoxed is IEnumerable<uint> creatureIdsToLearn && creatureIdsToForgetBoxed is IEnumerable<uint> creatureIdsToForget)
            {
                notification.Sent += (client) =>
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

            notification.Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
        }
    }
}
