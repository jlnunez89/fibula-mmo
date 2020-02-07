// -----------------------------------------------------------------
// <copyright file="LogInHandler.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a character log in request handler for the game server.
    /// </summary>
    public class LogInHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInHandler"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        /// <param name="protocolConfigurationOptions">A reference to the protocol configuration options.</param>
        /// <param name="logger">A reference to the logger to use in this handler.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext"></param>
        public LogInHandler(
            IApplicationContext applicationContext,
            IOptions<ProtocolConfigurationOptions> protocolConfigurationOptions,
            ILogger logger,
            IOperationFactory operationFactory,
            IGameContext gameContext)
            : base(logger, operationFactory, gameContext)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));
            protocolConfigurationOptions.ThrowIfNull(nameof(protocolConfigurationOptions));

            this.ApplicationContext = applicationContext;
            this.ProtocolConfiguration = protocolConfigurationOptions.Value;
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
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var loginInfo = message.ReadCharacterLoginInfo(this.ProtocolConfiguration.ClientVersion.Numeric);

            if (loginInfo.Version != default && loginInfo.Version != this.ProtocolConfiguration.ClientVersion.Numeric)
            {
                return new GameServerDisconnectPacket($"You need client version {this.ProtocolConfiguration.ClientVersion.Description} to connect to this server.").YieldSingleItem();
            }

            if (this.Context.Game.Status == WorldState.Loading)
            {
                return new GameServerDisconnectPacket("The game is just starting.\nPlease try again in a few minutes.").YieldSingleItem();
            }

            // Associate the xTea key to allow future validate packets from this connection.
            connection.SetupAuthenticationKey(loginInfo.XteaKey);
            connection.Authenticate(loginInfo.XteaKey);

            using var unitOfWork = new OpenTibiaUnitOfWork(this.ApplicationContext.DefaultDatabaseContext);

            AccountEntity account = unitOfWork.Accounts.FindOne(a => a.Number == loginInfo.AccountNumber && a.Password.Equals(loginInfo.Password));
            CharacterEntity character = null;

            if (account == null)
            {
                return new GameServerDisconnectPacket("The account number and password combination is invalid.").YieldSingleItem();
            }
            else
            {
                character = unitOfWork.Characters.FindOne(c => c.AccountId.Equals(account.Id) && c.Name.Equals(loginInfo.CharacterName));

                CharacterEntity otherCharacterOnline = unitOfWork.Characters.FindOne(c => c.IsOnline && c.AccountId == account.Id && !c.Name.Equals(loginInfo.CharacterName));

                if (character == null)
                {
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
                            return new GameServerDisconnectPacket("Your account is bannished.").YieldSingleItem();
                        }
                        else
                        {
                            account.Banished = false;
                        }
                    }
                    else if (account.Deleted)
                    {
                        return new GameServerDisconnectPacket("Your account is disabled.\nPlease contact us for more information.").YieldSingleItem();
                    }
                    else if (otherCharacterOnline != null)
                    {
                        return new GameServerDisconnectPacket("Another character in your account is online.").YieldSingleItem();
                    }
                    else if (this.Context.Game.Status == WorldState.Closed)
                    {
                        // Check if game is open to the public.
                        return new GameServerDisconnectPacket("This game world is not open to the public yet.\nCheck your access or the news on our webpage.").YieldSingleItem();
                    }
                }
            }

            // Set player status to online.
            character.IsOnline = true;

            this.ScheduleNewOperation(
                    OperationType.LogIn,
                    new LogInOperationCreationArguments(
                        new PlayerCreationMetadata(character.Id, character.Name, 100, 100, 100, 100, 4240),
                        connection,
                        this.Context.Game.WorldLightLevel,
                        this.Context.Game.WorldLightColor));

            // save any changes to the entities.
            unitOfWork.Complete();

            return null;
        }
    }
}