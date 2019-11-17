// <copyright file="PlayerLoginHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class PlayerLoginHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public PlayerLoginHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.LogInPlayerLoginRequest;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var playerLoginInfo = message.ReadPlayerLoginInfo();

            connection.XTeaKey = playerLoginInfo.XteaKey;

            var gameConfig = ServiceConfiguration.GetConfiguration();

            if (playerLoginInfo.Version != gameConfig.ClientVersionInt)
            {
                this.ResponsePackets.Add(new GameServerDisconnectPacket($"You need client version {gameConfig.ClientVersionString} to connect to this server."));

                return;
            }

            if (this.Game.Status == WorldState.Creating)
            {
                this.ResponsePackets.Add(new GameServerDisconnectPacket("The game is just starting.\nPlease try again in a few minutes."));

                return;
            }

            using (var otContext = new OpenTibiaDbContext())
            {
                var failureReason = LoginFailureReason.None;

                var userRecord = otContext.Users.FirstOrDefault(u => u.Login == playerLoginInfo.AccountNumber && u.Passwd.Equals(playerLoginInfo.Password));
                var playerRecord = otContext.Players.FirstOrDefault(p => p.Account_Nr == playerLoginInfo.AccountNumber && p.Charname.Equals(playerLoginInfo.CharacterName));

                if (userRecord == null || playerRecord == null)
                {
                    failureReason = LoginFailureReason.AccountOrPasswordIncorrect;
                }
                else
                {
                    // Check bannishment.
                    if (userRecord.Banished > 0 || userRecord.Bandelete > 0)
                    {
                        // Lift if time is up
                        if (userRecord.Bandelete > 0 || DateTimeOffset.FromUnixTimeSeconds(userRecord.Banished_Until) > DateTimeOffset.UtcNow)
                        {
                            failureReason = LoginFailureReason.Bannished;
                        }
                        else
                        {
                            userRecord.Banished = 0;
                        }
                    }

                    // Check that no other characters from this account are online.
                    var anotherCharacterIsLoggedIn = otContext.Players.Any(p => p.Account_Nr == playerLoginInfo.AccountNumber && p.Online > 0 && !p.Charname.Equals(playerLoginInfo.CharacterName));

                    if (anotherCharacterIsLoggedIn)
                    {
                        failureReason = LoginFailureReason.AnotherCharacterIsLoggedIn;
                    }

                    // Check if game is open to public
                    if (this.Game.Status != WorldState.Open)
                    {
                        this.ResponsePackets.Add(new GameServerDisconnectPacket("The game is not open to the public yet.\nCheck for news on our webpage."));

                        return;
                    }
                }

                if (failureReason == LoginFailureReason.None)
                {
                    try
                    {
                        // Set player status to online.
                        // playerRecord.online = 1;

                        // otContext.SaveChanges();
                        var player = this.Game.Login(playerRecord, connection);

                        // set this to allow future packets from this connection.
                        connection.IsAuthenticated = true;
                        connection.PlayerId = player.CreatureId;

                        this.ResponsePackets.Add(new SelfAppearPacket(player.CreatureId, true, player));

                        // Add MapDescription
                        this.ResponsePackets.Add(new MapDescriptionPacket(player.Location, this.Game.GetMapDescriptionAt(player, player.Location)));

                        this.ResponsePackets.Add(new MagicEffectPacket(player.Location, AnimatedEffect.BubbleBlue));

                        this.ResponsePackets.Add(new PlayerInventoryPacket(player));
                        this.ResponsePackets.Add(new PlayerStatusPacket(player));
                        this.ResponsePackets.Add(new PlayerSkillsPacket(player));

                        this.ResponsePackets.Add(new WorldLightPacket(this.Game.LightLevel, this.Game.LightColor));

                        this.ResponsePackets.Add(new CreatureLightPacket(player));

                        // Adds a text message
                        this.ResponsePackets.Add(new TextMessagePacket(MessageType.StatusDefault, "This is a test message"));

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
                        this.ResponsePackets.Add(new PlayerConditionsPacket(player));

                        return;
                    }
                    catch (Exception ex)
                    {
                        // TODO: propper logging
                        Console.WriteLine(ex);

                        failureReason = LoginFailureReason.InternalServerError;
                    }
                }

                if (failureReason != LoginFailureReason.None)
                {
                    // TODO: implement correctly.
                    this.ResponsePackets.Add(new GameServerDisconnectPacket(failureReason.ToString()));
                }
            }
        }
    }
}