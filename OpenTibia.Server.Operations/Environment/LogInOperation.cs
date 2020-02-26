// -----------------------------------------------------------------
// <copyright file="LogInOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Environment
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents a login operation.
    /// </summary>
    public class LogInOperation : BaseEnvironmentOperation
    {
        private static readonly Location NewbieStart = new Location { X = 32097, Y = 32219, Z = 7 };

        private static readonly Location VeteranStart = new Location { X = 32369, Y = 32241, Z = 7 };

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the action.</param>
        /// <param name="playerMetadata">The creation metadata of the player that is logging in.</param>
        /// <param name="connection">The connection that the player uses.</param>
        /// <param name="worldLightLevel">The level of the world light to send to the player.</param>
        /// <param name="worldLightColor">The color of the world light to send to the player.</param>
        public LogInOperation(ILogger logger, IElevatedOperationContext context, uint requestorId, ICreatureCreationMetadata playerMetadata, IConnection connection, byte worldLightLevel, byte worldLightColor)
            : base(logger, context, requestorId)
        {
            this.CurrentWorldLightLevel = worldLightLevel;
            this.CurrentWorldLightColor = worldLightColor;

            this.PlayerMetadata = playerMetadata;
            this.Connection = connection;
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
        /// Gets the connection instance of the player.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            // TODO: should be something like character.location
            var targetLocation = VeteranStart;

            IPlayer player = this.Context.CreatureFactory.Create(CreatureType.Player, this.PlayerMetadata) as IPlayer;

            if (!this.PlaceCreature(targetLocation, player))
            {
                return;
            }

            if (player == null)
            {
                // Unable to place the player in the map.
                this.Context.Scheduler.ScheduleEvent(
                    new GenericNotification(
                        this.Logger,
                        () => this.Connection.YieldSingleItem(),
                        new GenericNotificationArguments(
                            new GameServerDisconnectPacket("Your character could not be placed on the map.\nPlease try again, or contact an administrator if the issue persists."))));

                return;
            }

            this.Context.ConnectionManager.Register(this.Connection, player.Id);

            this.Context.Scheduler.ScheduleEvent(
                new GenericNotification(
                    this.Logger,
                    () => this.Connection.YieldSingleItem(),
                    new GenericNotificationArguments(
                        new SelfAppearPacket(player.Id, true, player),
                        new MapDescriptionPacket(player.Location, this.Context.MapDescriptor.DescribeAt(player, player.Location)),
                        new MagicEffectPacket(player.Location, AnimatedEffect.BubbleBlue),
                        new PlayerInventoryPacket(player),
                        new PlayerStatsPacket(player),
                        new PlayerSkillsPacket(player),
                        new WorldLightPacket(this.CurrentWorldLightLevel, this.CurrentWorldLightColor),
                        new CreatureLightPacket(player),
                        new TextMessagePacket(MessageType.StatusDefault, "This is a test message"),

                        // if (player->getLastLoginSaved() != 0)
                        // {
                        //    tempstring = "Your last visit was on ";
                        //    time_t lastLogin = player->getLastLoginSaved();
                        //    tempstring += ctime(&lastLogin);
                        //    tempstring.erase(tempstring.length() - 1);
                        //    tempstring += ".";

                        // AddTextMessage(msg, MSG_STATUS_DEFAULT, tempstring.c_str());
                        // }
                        // else
                        // {
                        //    tempstring = "Welcome to ";
                        //    tempstring += g_config.getString(ConfigManager::SERVER_NAME);
                        //    tempstring += ". Please choose an outfit.";
                        //    sendOutfitWindow(player);
                        // }

                        // Add any Vips here.

                        // for (VIPListSet::iterator it = player->VIPList.begin(); it != player->VIPList.end(); it++)
                        // {
                        //    bool online;
                        //    std::string vip_name;
                        //    if (IOPlayer::instance()->getNameByGuid((*it), vip_name))
                        //    {
                        //        online = (g_game.getPlayerByName(vip_name) != NULL);
                        //
                        // msg->AddByte(0xD2);
                        // msg->AddU32(guid);
                        // msg->AddString(name);
                        // msg->AddByte(isOnline ? 1 : 0);
                        //    }
                        // }
                        new PlayerConditionsPacket(player))));
        }
    }
}
