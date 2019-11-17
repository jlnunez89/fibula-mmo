// -----------------------------------------------------------------
// <copyright file="NewConnectionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Contracts.Abstractions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Server.Messaging.Packets;

    /// <summary>
    /// Class that represents a new connection request handler for the login server.
    /// </summary>
    public sealed class NewConnectionHandler : BaseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewConnectionHandler"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        /// <param name="gameConfigOptions">The game configuration options.</param>
        /// <param name="protocolConfigOptions">The protocol configuration options.</param>
        public NewConnectionHandler(
            IApplicationContext applicationContext,
            IOptions<GameConfigurationOptions> gameConfigOptions,
            IOptions<ProtocolConfigurationOptions> protocolConfigOptions)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));
            gameConfigOptions.ThrowIfNull(nameof(gameConfigOptions));
            protocolConfigOptions.ThrowIfNull(nameof(protocolConfigOptions));

            this.ApplicationContext = applicationContext;
            this.GameConfiguration = gameConfigOptions.Value;
            this.ProtocolConfiguration = protocolConfigOptions.Value;
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.LoginServerRequest;

        /// <summary>
        /// Gets a reference to the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Gets the game configuration.
        /// </summary>
        public GameConfigurationOptions GameConfiguration { get; }

        /// <summary>
        /// Gets the protocol configuration.
        /// </summary>
        public ProtocolConfigurationOptions ProtocolConfiguration { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            var responsePackets = new List<IOutgoingPacket>();

            var newConnectionInfo = message.ReadNewConnectionInfo();

            if (newConnectionInfo.Version != this.ProtocolConfiguration.ClientVersion.Numeric)
            {
                // TODO: hardcoded messages.
                responsePackets.Add(new LoginServerDisconnectPacket($"You need client version {this.ProtocolConfiguration.ClientVersion.Description} to connect to this server."));

                return (true, responsePackets);
            }

            // Make a copy of the message in case we fail to decrypt using the first set of keys.
            var messageCopy = message.Copy();

            message.RsaDecrypt(useCipKeys: this.ProtocolConfiguration.UsingCipsoftRsaKeys);

            // If GetByte() here is not Zero, it means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
            if (message.GetByte() != 0)
            {
                message = messageCopy;

                message.RsaDecrypt(useCipKeys: !this.ProtocolConfiguration.UsingCipsoftRsaKeys);

                if (message.GetByte() != 0)
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
                    return (false, null);
                }
            }

            var accLoginInfo = message.ReadAccountLoginInfo();

            connection.XTeaKey = accLoginInfo.XteaKey;

            using (var unitOfWork = new OpenTibiaUnitOfWork(this.ApplicationContext.DefaultDatabaseContext))
            {
                // validate credentials.
                var accounts = unitOfWork.Accounts.Find(u => u.Number == accLoginInfo.AccountNumber && u.Password.Equals(accLoginInfo.Password));
                var account = accounts.FirstOrDefault();

                if (account == null)
                {
                    // TODO: hardcoded messages.
                    responsePackets.Add(new LoginServerDisconnectPacket("Please enter a valid account number and password."));

                    return (true, responsePackets);
                }

                var charactersFound = unitOfWork.Characters.Find(p => p.AccountId == account.Id);

                if (!charactersFound.Any())
                {
                    // TODO: hardcoded messages.
                    responsePackets.Add(new LoginServerDisconnectPacket($"You don't have any characters in your account.\nPlease create a new character in our web site: {this.GameConfiguration.World.WebsiteUrl}"));

                    return (true, responsePackets);
                }

                var charList = new List<ICharacterListItem>();

                foreach (var character in charactersFound)
                {
                    charList.Add(new CharacterListItem(
                        character.Name,
                        IPAddress.Parse(this.GameConfiguration.PublicAddressBinding.Ipv4Address),
                        this.GameConfiguration.PublicAddressBinding.Port,
                        this.GameConfiguration.World.Name));
                }

                responsePackets.Add(new MessageOfTheDayPacket(this.GameConfiguration.World.MessageOfTheDay));
                responsePackets.Add(new CharacterListPacket(charList, (ushort)(account.PremiumDays + account.TrialOrBonusPremiumDays)));

                return (true, responsePackets);
            }
        }
    }
}