// -----------------------------------------------------------------
// <copyright file="PlayerLoginHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Contracts.Abstractions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Data.Entities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a character log in request handler for the game server.
    /// </summary>
    public class PlayerLoginHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginHandler"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        /// <param name="protocolConfigurationOptions">A reference to the protocol configuration options.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="logger">A reference to the logger to use in this handler.</param>
        public PlayerLoginHandler(
            IApplicationContext applicationContext,
            IOptions<ProtocolConfigurationOptions> protocolConfigurationOptions,
            IGame gameInstance,
            ILogger logger)
            : base(gameInstance)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));
            protocolConfigurationOptions.ThrowIfNull(nameof(protocolConfigurationOptions));

            this.ApplicationContext = applicationContext;
            this.ProtocolConfiguration = protocolConfigurationOptions.Value;
            this.Logger = logger.ForContext<PlayerLoginHandler>();
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.LogIn;

        /// <summary>
        /// Gets a reference to the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Gets the protocol configuration.
        /// </summary>
        public ProtocolConfigurationOptions ProtocolConfiguration { get; }

        /// <summary>
        /// Gets the logger to use in this handler.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            var loginInfo = message.ReadCharacterLoginInfo();

            var responsePackets = new List<IOutgoingPacket>();

            if (loginInfo.Version != this.ProtocolConfiguration.ClientVersion.Numeric)
            {
                responsePackets.Add(new GameServerDisconnectPacket($"You need client version {this.ProtocolConfiguration.ClientVersion.Description} to connect to this server."));

                return (true, responsePackets);
            }

            if (this.Game.Status == WorldState.Loading)
            {
                responsePackets.Add(new GameServerDisconnectPacket("The game is just starting.\nPlease try again in a few minutes."));

                return (true, responsePackets);
            }

            // Associate the xTea key to allow future validate packets from this connection.
            connection.SetupAuthenticationKey(loginInfo.XteaKey);

            if (!connection.Authenticate(loginInfo.XteaKey))
            {
                connection.SetupAuthenticationKey(loginInfo.XteaKey);

                responsePackets.Add(new GameServerDisconnectPacket("There was a problem with your session key.\nPlease logging in again."));

                return (true, responsePackets);
            }

            using var unitOfWork = new OpenTibiaUnitOfWork(this.ApplicationContext.DefaultDatabaseContext);

            AccountEntity account = unitOfWork.Accounts.FindOne(a => a.Number == loginInfo.AccountNumber && a.Password.Equals(loginInfo.Password));
            CharacterEntity character = null;

            var failureReason = string.Empty;

            if (account == null)
            {
                failureReason = "The account number and password combination is invalid.";
            }
            else
            {
                character = unitOfWork.Characters.FindOne(c => c.AccountId.Equals(account.Id) && c.Name.Equals(loginInfo.CharacterName));

                CharacterEntity otherCharacterOnline = unitOfWork.Characters.FindOne(c => c.IsOnline && c.AccountId == account.Id && !c.Name.Equals(loginInfo.CharacterName));

                if (character == null)
                {
                    failureReason = "The character selected was not found in this account.";
                }
                else
                {
                    // Check bannishment.
                    if (account.Banished)
                    {
                        // Lift if time is up
                        if (account.Banished && account.BanishedUntil > DateTimeOffset.UtcNow)
                        {
                            failureReason = "Your account is bannished.";
                        }
                        else
                        {
                            account.Banished = false;
                        }
                    }
                    else if (account.Deleted)
                    {
                        failureReason = "Your account is disabled.\nPlease contact us for more information.";
                    }
                    else if (otherCharacterOnline != null)
                    {
                        failureReason = "Another character in your account is online.";
                    }
                    else if (this.Game.Status != WorldState.Open)
                    {
                        // Check if game is open to public
                        failureReason = "The server is not open to the public yet.\nCheck for news on our webpage.";
                    }
                }
            }

            if (failureReason == string.Empty)
            {
                try
                {
                    // Set player status to online.
                    character.IsOnline = true;

                    var player = this.Game.PlayerLogin(character, connection);

                    // Also, associate it to the new player.
                    connection.AssociateToPlayer(player.Id);

                    responsePackets.Add(new SelfAppearPacket(player.Id, true, player));

                    // Add MapDescription
                    var mapDescription = this.Game.GetDescriptionOfMapForPlayer(player, player.Location);

                    responsePackets.Add(new MapDescriptionPacket(player.Location, mapDescription.ToArray()));

                    responsePackets.Add(new MagicEffectPacket(player.Location, AnimatedEffect.BubbleBlue));

                    responsePackets.Add(new PlayerInventoryPacket(player));

                    // Adds a text message
                    responsePackets.Add(new PlayerStatsPacket(player));

                    responsePackets.Add(new PlayerSkillsPacket(player));

                    responsePackets.Add(new WorldLightPacket(this.Game.WorldLightLevel, this.Game.WorldLightColor));

                    responsePackets.Add(new CreatureLightPacket(player));

                    // Adds a text message
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusDefault, "This is a test message"));

                    // std::string tempstring = g_config.getString(ConfigManager::LOGIN_MSG);
                    // if (tempstring.size() > 0)
                    // {
                    //    AddTextMessage(msg, MSG_STATUS_DEFAULT, tempstring.c_str());
                    // }

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

                    // Send condition icons
                    responsePackets.Add(new PlayerConditionsPacket(player));
                }
                catch (Exception ex)
                {
                    this.Logger.Error($"Unexpected error handling player login request: {ex.Message}");

                    failureReason = "Internal server error.";
                }
            }

            if (failureReason != string.Empty)
            {
                responsePackets.Clear();

                responsePackets.Add(new GameServerDisconnectPacket(failureReason.ToString()));
            }

            // save any changes to the entities.
            unitOfWork.Complete();

            return (true, responsePackets);
        }
    }
}