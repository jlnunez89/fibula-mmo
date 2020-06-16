// -----------------------------------------------------------------
// <copyright file="GatewayLogInHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Data;
    using Serilog;

    /// <summary>
    /// Class that represents a new connection request handler for the login server.
    /// </summary>
    public sealed class GatewayLogInHandler : BaseRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayLogInHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="applicationContext">A reference to the application context.</param>
        public GatewayLogInHandler(ILogger logger, IApplicationContext applicationContext)
            : base(logger)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));

            this.ApplicationContext = applicationContext;
        }

        /// <summary>
        /// Gets a reference to the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client)
        {
            incomingPacket.ThrowIfNull(nameof(incomingPacket));
            client.ThrowIfNull(nameof(client));

            if (!(incomingPacket is IGatewayLoginInfo accountLoginInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IGatewayLoginInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            if (!(client.Connection is ISocketConnection socketConnection))
            {
                this.Logger.Error($"Expected a {nameof(ISocketConnection)} got a {client.Connection.GetType().Name}.");

                return null;
            }

            // Associate the xTea key to allow future validate packets from this connection.
            socketConnection.SetupAuthenticationKey(accountLoginInfo.XteaKey);

            if (accountLoginInfo.ClientVersion != this.ApplicationContext.Options.SupportedClientVersion.Numeric)
            {
                this.Logger.Information($"Client attempted to connect with version: {accountLoginInfo.ClientVersion}, OS: {accountLoginInfo.ClientOs}. Expected version: {this.ApplicationContext.Options.SupportedClientVersion.Numeric}.");

                // TODO: hardcoded messages.
                return new GatewayServerDisconnectPacket($"You need client version {this.ApplicationContext.Options.SupportedClientVersion.Description} to connect to this server.").YieldSingleItem();
            }

            using var unitOfWork = new UnitOfWork(this.ApplicationContext.DefaultDatabaseContext);

            // validate credentials.
            var accounts = unitOfWork.Accounts.FindMany(u => u.Number == accountLoginInfo.AccountNumber && u.Password.Equals(accountLoginInfo.Password));
            var account = accounts.FirstOrDefault();

            if (account == null)
            {
                // TODO: hardcoded messages.
                return new GatewayServerDisconnectPacket("Please enter a valid account number and password.").YieldSingleItem();
            }

            var charactersFound = unitOfWork.Characters.FindMany(p => p.AccountId == account.Id);

            if (!charactersFound.Any())
            {
                // TODO: hardcoded messages.
                return new GatewayServerDisconnectPacket($"You don't have any characters in your account.\nPlease create a new character in our web site: {this.ApplicationContext.Options.WebsiteUrl}").YieldSingleItem();
            }

            var charList = new List<CharacterInfo>();

            foreach (var character in charactersFound)
            {
                charList.Add(new CharacterInfo()
                {
                    Name = character.Name,
                    Ip = IPAddress.Parse(this.ApplicationContext.Options.World.IpAddress).GetAddressBytes(),
                    Port = this.ApplicationContext.Options.World.Port.Value,
                    World = this.ApplicationContext.Options.World.Name,
                });
            }

            return new IOutboundPacket[]
            {
                new MessageOfTheDayPacket(this.ApplicationContext.Options.World.MessageOfTheDay),
                new CharacterListPacket(charList, (ushort)(account.PremiumDays + account.TrialOrBonusPremiumDays)),
            };
        }
    }
}