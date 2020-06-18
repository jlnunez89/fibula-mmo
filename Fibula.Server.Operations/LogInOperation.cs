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

namespace Fibula.Server.Operations
{
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Mechanics.Contracts.Enumerations;
    using Fibula.Server.Notifications;
    using Fibula.Server.Notifications.Arguments;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a login operation.
    /// </summary>
    public class LogInOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the action.</param>
        /// <param name="playerMetadata">The creation metadata of the player that is logging in.</param>
        /// <param name="worldLightLevel">The level of the world light to send to the player.</param>
        /// <param name="worldLightColor">The color of the world light to send to the player.</param>
        public LogInOperation(uint requestorId, IPlayerCreationMetadata playerMetadata, byte worldLightLevel, byte worldLightColor)
            : base(requestorId)
        {
            this.CurrentWorldLightLevel = worldLightLevel;
            this.CurrentWorldLightColor = worldLightColor;

            this.PlayerMetadata = playerMetadata;
        }

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
            // TODO: should be something like character.location
            var rookMark = new Location { X = 32097, Y = 32219, Z = 7 };
            var thaisMark = new Location { X = 32369, Y = 32241, Z = 7 };
            var targetLocation = thaisMark;

            IPlayer player = context.CreatureFactory.CreateCreature(new CreatureCreationArguments() { Type = CreatureType.Player, Metadata = this.PlayerMetadata }) as IPlayer;

            if (!context.TileAccessor.GetTileAt(targetLocation, out ITile targetTile) || !this.PlaceCreature(context, targetTile, player))
            {
                return;
            }

            if (player == null)
            {
                // Unable to place the player in the map.
                context.Scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => player.YieldSingleItem(),
                        new GenericNotificationArguments(
                            new GameServerDisconnectPacket("Your character could not be placed on the map.\nPlease try again, or contact an administrator if the issue persists."))));

                return;
            }

            context.Scheduler.ScheduleEvent(
                new GenericNotification(
                    () => player.YieldSingleItem(),
                    new GenericNotificationArguments(
                        new SelfAppearPacket(player.Id, true, player),
                        new MapDescriptionPacket(player.Location, context.MapDescriptor.DescribeAt(player, player.Location)),
                        new MagicEffectPacket(player.Location, AnimatedEffect.BubbleBlue),
                        new PlayerInventoryPacket(player),
                        new PlayerStatsPacket(player),
                        new PlayerSkillsPacket(player),
                        new WorldLightPacket(this.CurrentWorldLightLevel, this.CurrentWorldLightColor),
                        new CreatureLightPacket(player),
                        new TextMessagePacket(MessageType.StatusDefault, "This is a test message"),
                        // TODO: Send first time login message + outfit window here if needed.
                        // TODO: Send any Buddies here.
                        new PlayerConditionsPacket(player))));
        }
    }
}
