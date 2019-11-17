// -----------------------------------------------------------------
// <copyright file="ManagementHandlerSelector.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a handler selector for incoming management packet types.
    /// </summary>
    public class ManagementHandlerSelector : IHandlerSelector
    {
        /// <summary>
        /// The known handlers to pick from.
        /// </summary>
        private readonly IList<IHandler> handlersKnown;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementHandlerSelector"/> class.
        /// </summary>
        /// <param name="handlersRegistered">The collection of handlers registered in the configuration root fo the service.</param>
        public ManagementHandlerSelector(IEnumerable<IHandler> handlersRegistered)
        {
            handlersRegistered.ThrowIfNull(nameof(handlersRegistered));

            this.handlersKnown = handlersRegistered.ToList();
        }

        /// <summary>
        /// Gets the protocol type for which this handler selector works.
        /// </summary>
        public OpenTibiaProtocolType ForProtocol => OpenTibiaProtocolType.ManagementProtocol;

        /// <summary>
        /// Returns the most appropriate handler for the specified packet type.
        /// </summary>
        /// <param name="forType">The packet type to select the handler for.</param>
        /// <returns>An instance of an <see cref="IHandler"/> implementaion.</returns>
        public IHandler SelectForType(byte forType)
        {
            var handler = this.handlersKnown.SingleOrDefault(h => h.ForPacketType == forType);

            if (handler == null)
            {
                return new DefaultHandler(forType);
            }

            /*
            switch ((LoginOrManagementIncomingPacketType)packeType)
            {
                case LoginOrManagementIncomingPacketType.AuthenticationRequest:
                    return new AuthenticationHandler();
                case LoginOrManagementIncomingPacketType.ServerStatusRequest:
                    return new ServerStatusHandler();
                case LoginOrManagementIncomingPacketType.PlayerLogIn:
                    return new Handlers.PlayerLoginHandler();
                case LoginOrManagementIncomingPacketType.PlayerLogOut:
                    return new PlayerLogoutHandler();
                case LoginOrManagementIncomingPacketType.NameLock:
                    // DebugPacket();
                    // TODO
                    // writeMsg.addByte(0x00); // ErrorCode
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.Banishment:
                    return new BanismentHandler();
                case LoginOrManagementIncomingPacketType.Notation:
                    return new NotationHandler();
                case LoginOrManagementIncomingPacketType.ReportStatement:
                    return new ReportStatementHandler();
                case LoginOrManagementIncomingPacketType.CharacterDeath:
                    return new CharacterDeathHandler();
                case LoginOrManagementIncomingPacketType.CreatePlayerUnsure:
                    // DebugPacket();
                    // unsigned int ACCID = msg.getU32();
                    // unsigned int unknown = msg.getByte();

                    // writeMsg.addByte(0x00); // ErrorCode
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.FinishAuctions:
                    return new FinishAuctionsHandler();
                case LoginOrManagementIncomingPacketType.TransferHouses:
                    // return new TransferHousesHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictFreeAccounts:
                    // return new EvictFreeAccountsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictDeleted:
                    // return new EvictDeletedHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictDelinquentGuildhouse:
                    // return new EvictDelinquentGuildhouseHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.GetHouseOwners:
                    // return new GetHouseOwnersHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.InsertHouseOwner:
                    // return new InsertHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.UpdateHouseOwner:
                    // return new UpdateHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.DeleteHouseOwner:
                    // return new DeleteHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.BanishIpAddress:
                    // return new BanishIpAddressHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.AddVip:
                    // return new AddVIPHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.GetAuctions:
                    // return new GetAuctionsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.StartAuction:
                    // return new StartAuctionHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.InsertHouses:
                    return new InsertHousesHandler();
                case LoginOrManagementIncomingPacketType.ClearIsOnline:
                    return new ClearIsOnlineHandler();
                case LoginOrManagementIncomingPacketType.CreatePlayerList:
                    return new CreatePlayerListHandler();
                case LoginOrManagementIncomingPacketType.LogKilledCreatures:
                    // return new LogKilledCreaturesHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.LoadPlayers:
                    return new LoadPlayersHandler();
                case LoginOrManagementIncomingPacketType.ExcludeFromAuctions:
                    // return new ExcludeFromAuctionsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.LoadWorld:
                    return new LoadWorldHandler();
                case LoginOrManagementIncomingPacketType.HighscoreUnsure:
                    // return new HighscoreHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.CreateHighscores:
                    // return new CreateHighscoresHandler();
                    return new DefaultHandler(packeType);
                default:
                    return new DefaultHandler(packeType);
            }
            */

            return handler;
        }
    }
}
