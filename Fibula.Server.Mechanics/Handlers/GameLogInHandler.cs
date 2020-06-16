// -----------------------------------------------------------------
// <copyright file="GameLogInHandler.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Data;
    using Fibula.Data.Entities;
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a character log in request handler for the game server.
    /// </summary>
    public class GameLogInHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogInHandler"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        /// <param name="logger">A reference to the logger to use in this handler.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public GameLogInHandler(
            IApplicationContext applicationContext,
            ILogger logger,
            IGame gameInstance)
            : base(logger, gameInstance)
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

            if (!(incomingPacket is IGameLogInInfo loginInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IGameLogInInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            if (!(client.Connection is ISocketConnection socketConnection))
            {
                this.Logger.Error($"Expected a {nameof(ISocketConnection)} got a {client.Connection.GetType().Name}.");

                return null;
            }

            // Associate the xTea key to allow future validate packets from this connection.
            socketConnection.SetupAuthenticationKey(loginInfo.XteaKey);

            if (loginInfo.ClientVersion != this.ApplicationContext.Options.SupportedClientVersion.Numeric)
            {
                this.Logger.Information($"Client attempted to connect with version: {loginInfo.ClientVersion}, OS: {loginInfo.ClientOs}. Expected version: {this.ApplicationContext.Options.SupportedClientVersion.Numeric}.");

                // TODO: hardcoded messages.
                return new GameServerDisconnectPacket($"You need client version {this.ApplicationContext.Options.SupportedClientVersion.Description} to connect to this server.").YieldSingleItem();
            }

            if (this.Game.WorldInfo.Status == WorldState.Loading)
            {
                return new GameServerDisconnectPacket("The game is just starting.\nPlease try again in a few minutes.").YieldSingleItem();
            }

            using var unitOfWork = new UnitOfWork(this.ApplicationContext.DefaultDatabaseContext);

            AccountEntity account = unitOfWork.Accounts.FindOne(a => a.Number == loginInfo.AccountNumber && a.Password.Equals(loginInfo.Password));
            CharacterEntity character = null;

            if (account == null)
            {
                // TODO: hardcoded messages.
                return new GameServerDisconnectPacket("The account number and password combination is invalid.").YieldSingleItem();
            }
            else
            {
                character = unitOfWork.Characters.FindOne(c => c.AccountId.Equals(account.Id) && c.Name.Equals(loginInfo.CharacterName));

                CharacterEntity otherCharacterOnline = unitOfWork.Characters.FindOne(c => c.IsOnline && c.AccountId == account.Id && !c.Name.Equals(loginInfo.CharacterName));

                if (character == null)
                {
                    // TODO: hardcoded messages.
                    return new GameServerDisconnectPacket("The character selected was not found in this account.").YieldSingleItem();
                }
                else
                {
                    // Check bannishment.
                    if (account.Banished)
                    {
                        // Lift if time is up
                        if (account.Banished && account.BanishedUntil > DateTimeOffset.UtcNow)
                        {
                            // TODO: hardcoded messages.
                            return new GameServerDisconnectPacket("Your account is bannished.").YieldSingleItem();
                        }
                        else
                        {
                            account.Banished = false;
                        }
                    }
                    else if (account.Deleted)
                    {
                        // TODO: hardcoded messages.
                        return new GameServerDisconnectPacket("Your account is disabled.\nPlease contact us for more information.").YieldSingleItem();
                    }
                    else if (otherCharacterOnline != null)
                    {
                        // TODO: hardcoded messages.
                        return new GameServerDisconnectPacket("Another character in your account is online.").YieldSingleItem();
                    }
                    else if (this.Game.WorldInfo.Status == WorldState.Closed)
                    {
                        // TODO: hardcoded messages.
                        // Check if game is open to the public.
                        return new GameServerDisconnectPacket("This game world is not open to the public yet.\nCheck your access or the news on our webpage.").YieldSingleItem();
                    }
                }
            }

            // Set player status to online.
            character.IsOnline = true;

            // TODO: possibly a friendly name conversion here.
            client.ClientInformation.Agent = loginInfo.ClientOs.ToString();
            client.ClientInformation.Version = loginInfo.ClientVersion.ToString();

            this.Game.LogPlayerIn(new PlayerCreationMetadata(client, character.Id, character.Name, 100, 100, 100, 100, 4240));

            // save any changes to the entities.
            unitOfWork.Complete();

            return null;
        }
    }
}