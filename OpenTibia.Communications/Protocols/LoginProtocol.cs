// -----------------------------------------------------------------
// <copyright file="LoginProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    internal class LoginProtocol : BaseProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginProtocol"/> class.
        /// </summary>
        /// <param name="handlerSelector"></param>
        /// <param name="protocolConfigOptions"></param>
        /// <param name="gameConfigOptions"></param>
        public LoginProtocol(
            IHandlerSelector handlerSelector,
            ProtocolConfigurationOptions protocolConfigOptions,
            GameConfigurationOptions gameConfigOptions)
            : base(handlerSelector, keepConnectionOpen: false)
        {
            protocolConfigOptions.ThrowIfNull(nameof(protocolConfigOptions));
            gameConfigOptions.ThrowIfNull(nameof(gameConfigOptions));

            this.ProtocolConfiguration = protocolConfigOptions;
            this.GameConfiguration = gameConfigOptions;
        }

        public ProtocolConfigurationOptions ProtocolConfiguration { get; }

        public GameConfigurationOptions GameConfiguration { get; }

        public override void ProcessMessage(IConnection connection, INetworkMessage inboundMessage)
        {
            connection.ThrowIfNull(nameof(connection));
            inboundMessage.ThrowIfNull(nameof(inboundMessage));

            IncomingManagementPacketType packetType = (IncomingManagementPacketType)inboundMessage.GetByte();

            if (packetType != IncomingManagementPacketType.LoginServerRequest)
            {
                // TODO: proper logging.
                // This packet should NOT have been routed to this protocol.
                Trace.TraceWarning("Non LoginServerRequest packet routed to LoginProtocol. Packet was ignored.");
                return;
            }

            var newConnectionInfo = inboundMessage.ReadNewConnectionInfo();

            if (newConnectionInfo.Version != this.ProtocolConfiguration.ClientVersion.Numeric)
            {
                // TODO: hardcoded messages.
                this.SendDisconnect(connection, $"You need client version {this.ProtocolConfiguration.ClientVersion.Description} to connect to this server.");

                return;
            }

            // Make a copy of the message in case we fail to decrypt using the first set of keys.
            var messageCopy = inboundMessage.Copy();

            inboundMessage.RsaDecrypt(useCipKeys: this.ProtocolConfiguration.UsingCipsoftRsaKeys);

            // If GetByte() here is not Zero, it means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
            if (inboundMessage.GetByte() != 0)
            {
                inboundMessage = messageCopy;

                inboundMessage.RsaDecrypt(useCipKeys: !this.ProtocolConfiguration.UsingCipsoftRsaKeys);

                if (inboundMessage.GetByte() != 0)
                {
                    // These RSA keys are also unsuccessful... give up.
                    // loginPacket = new AccountLoginPacket(inboundMessage);

                    // connection.SetXtea(loginPacket?.XteaKey);

                    //// TODO: hardcoded messages.
                    // if (gameConfig.UsingCipsoftRSAKeys)
                    // {
                    //    this.SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that does not replace the RSA Keys.\nWe recommend using Tibia Loader's 7.7 client.\nYou may also download the client from our website.");
                    // }
                    // else
                    // {
                    //    this.SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that replaces the RSA Keys.\nWe recommend using OTLand's IP changer with a virgin 7.7 client.\nYou may also download the client from our website.");
                    // }
                    return;
                }
            }

            var accLoginInfo = inboundMessage.ReadAccountLoginInfo();

            connection.XTeaKey = accLoginInfo.XteaKey;

            using (var otContext = new OpenTibiaDbContext())
            {
                // validate credentials.
                var user = otContext.Users.FirstOrDefault(u => u.Login == accLoginInfo.AccountNumber && u.Passwd.Equals(accLoginInfo.Password));

                if (user == null)
                {
                    // TODO: hardcoded messages.
                    this.SendDisconnect(connection, "Please enter a valid account number and password.");
                }
                else
                {
                    var charactersFound = otContext.Players.Where(p => p.Account_Nr == user.Login);

                    if (!charactersFound.Any())
                    {
                        // TODO: hardcoded messages.
                        this.SendDisconnect(connection, $"You have no characters.\nPlease create a new character in our web site first: {this.GameConfiguration.World.WebsiteUrl}");
                    }
                    else
                    {
                        var charList = new List<ICharacterListItem>();

                        foreach (var character in charactersFound)
                        {
                            charList.Add(new CharacterListItem(
                                character.Charname,
                                IPAddress.Parse(this.GameConfiguration.PublicAddressBinding.Ipv4Address),
                                this.GameConfiguration.PublicAddressBinding.Port,
                                this.GameConfiguration.World.Name));
                        }

                        // TODO: motd
                        this.SendCharacterList(connection, this.GameConfiguration.World.MessageOfTheDay, (ushort)Math.Min(user.Premium_Days + user.Trial_Premium_Days, ushort.MaxValue), charList);
                    }
                }
            }
        }

        private void SendDisconnect(IConnection connection, string reason)
        {
            var message = new NetworkMessage();

            message.WriteLoginServerDisconnectPacket(new LoginServerDisconnectPacket(reason));

            connection.Send(message);
        }

        private void SendCharacterList(IConnection connection, string motd, ushort premiumDays, IEnumerable<ICharacterListItem> chars)
        {
            var message = new NetworkMessage();

            if (motd != string.Empty)
            {
                message.WriteMessageOfTheDayPacket(new MessageOfTheDayPacket(motd));
            }

            message.WriteCharacterListPacket(new CharacterListPacket(chars, premiumDays));

            connection.Send(message);
        }
    }
}
