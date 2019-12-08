// -----------------------------------------------------------------
// <copyright file="ConfigurationRootExtensions.cs" company="2Dudes">
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
    using Microsoft.Extensions.DependencyInjection;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers.Game;
    using OpenTibia.Communications.Handlers.Management;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class ConfigurationRootExtensions
    {
        /// <summary>
        /// Adds all implementations of <see cref="IHandlerSelector"/> contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddHandlerSelectors(this IServiceCollection services)
        {
            services.AddSingleton<IHandlerSelector, GameHandlerSelector>();
            services.AddSingleton<IHandlerSelector, ManagementHandlerSelector>();
        }

        /// <summary>
        /// Adds all implementations of <see cref="IHandler"/> related to the game, contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddGameHandlers(this IServiceCollection services)
        {
            // Game handlers only
            //services.AddSingleton<IHandler, AttackHandler>();
            //services.AddSingleton<IHandler, AutoMoveCancelHandler>();
            //services.AddSingleton<IHandler, AutoMoveHandler>();
            //services.AddSingleton<IHandler, ChannelCloseHandler>();
            //services.AddSingleton<IHandler, ChannelCreateOwnHandler>();
            //services.AddSingleton<IHandler, ChannelInviteHandler>();
            //services.AddSingleton<IHandler, ChannelKickHandler>();
            //services.AddSingleton<IHandler, ChannelListRequestHandler>();
            //services.AddSingleton<IHandler, ChannelOpenPrivateHandler>();
            //services.AddSingleton<IHandler, ChannelOpenPublicHandler>();
            //services.AddSingleton<IHandler, ContainerCloseHandler>();
            //services.AddSingleton<IHandler, ContainerUpHandler>();
            //services.AddSingleton<IHandler, FollowHandler>();
            //services.AddSingleton<IHandler, HouseWindowPostHandler>();
            services.AddSingleton<IHandler, ItemMoveHandler>();
            //services.AddSingleton<IHandler, ItemRotateHandler>();
            //services.AddSingleton<IHandler, ItemUseBattleHandler>();
            //services.AddSingleton<IHandler, ItemUseHandler>();
            //services.AddSingleton<IHandler, ItemUseOnHandler>();
            services.AddSingleton<IHandler, LogoutHandler>();
            services.AddSingleton<IHandler, LookAtHandler>();
            //services.AddSingleton<IHandler, OutfitChangedHandler>();
            //services.AddSingleton<IHandler, OutfitChangeRequestHandler>();
            //services.AddSingleton<IHandler, PartyAcceptHandler>();
            //services.AddSingleton<IHandler, PartyLeaveHandler>();
            //services.AddSingleton<IHandler, PartyPassLeadershipHandler>();
            //services.AddSingleton<IHandler, PartyRejectHandler>();
            //services.AddSingleton<IHandler, PartyRequestHandler>();
            services.AddSingleton<IHandler, PingRequestHandler>();
            services.AddSingleton<IHandler, PlayerLoginHandler>();
            //services.AddSingleton<IHandler, PlayerSetModeHandler>();
            services.AddSingleton<IHandler, PlayerTurnEastHandler>();
            services.AddSingleton<IHandler, PlayerTurnNorthHandler>();
            services.AddSingleton<IHandler, PlayerTurnSouthHandler>();
            services.AddSingleton<IHandler, PlayerTurnWestHandler>();
            services.AddSingleton<IHandler, PlayerWalkEastHandler>();
            services.AddSingleton<IHandler, PlayerWalkNortheastHandler>();
            services.AddSingleton<IHandler, PlayerWalkNorthHandler>();
            services.AddSingleton<IHandler, PlayerWalkNorthWestHandler>();
            services.AddSingleton<IHandler, PlayerWalkSoutheastHandler>();
            services.AddSingleton<IHandler, PlayerWalkSouthHandler>();
            services.AddSingleton<IHandler, PlayerWalkSouthwestHandler>();
            services.AddSingleton<IHandler, PlayerWalkWestHandler>();
            //services.AddSingleton<IHandler, ReportBugHandler>();
            //services.AddSingleton<IHandler, ReportCancelHandler>();
            //services.AddSingleton<IHandler, ReportCloseHandler>();
            //services.AddSingleton<IHandler, ReportDebugAssertionHandler>();
            //services.AddSingleton<IHandler, ReportProcessHandler>();
            //services.AddSingleton<IHandler, ReportViolationHandler>();
            //services.AddSingleton<IHandler, ReSendContainerRequestHandler>();
            //services.AddSingleton<IHandler, ReSendTileRequestHandler>();
            //services.AddSingleton<IHandler, SpeechHandler>();
            //services.AddSingleton<IHandler, StopAllActionsHandler>();
            //services.AddSingleton<IHandler, TextWindowPostHandler>();
            //services.AddSingleton<IHandler, TradeAcceptHandler>();
            //services.AddSingleton<IHandler, TradeCancelHandler>();
            //services.AddSingleton<IHandler, TradeLookHandler>();
            //services.AddSingleton<IHandler, TradeRequestHandler>();
            //services.AddSingleton<IHandler, VipAddHandler>();
            //services.AddSingleton<IHandler, VipRemoveHandler>();

            services.AddSingleton<IHandlerSelector, GameHandlerSelector>();
        }

        /// <summary>
        /// Adds all implementations of <see cref="IHandler"/> related to management, contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddManagementHandlers(this IServiceCollection services)
        {
            // Management handlers
            // services.AddSingleton<IHandler, AuthenticationHandler>();
            // services.AddSingleton<IHandler, BanismentHandler>();
            // services.AddSingleton<IHandler, CharacterDeathHandler>();
            // services.AddSingleton<IHandler, ClearIsOnlineHandler>();
            // services.AddSingleton<IHandler, CreatePlayerListHandler>();
            // services.AddSingleton<IHandler, FinishAuctionsHandler>();
            // services.AddSingleton<IHandler, InsertHousesHandler>();
            // services.AddSingleton<IHandler, LoadPlayersHandler>();
            // services.AddSingleton<IHandler, LoadWorldHandler>();
            // services.AddSingleton<IHandler, NotationHandler>();
            // services.AddSingleton<IHandler, PlayerLoggedInHandler>();
            // services.AddSingleton<IHandler, PlayerLoggedOutHandler>();
            // services.AddSingleton<IHandler, ReportStatementHandler>();
            // services.AddSingleton<IHandler, ServerStatusHandler>();

            services.AddSingleton<IHandlerSelector, ManagementHandlerSelector>();
        }

        /// <summary>
        /// Adds all implementations of <see cref="IHandler"/> related to login, contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddLoginHandlers(this IServiceCollection services)
        {
            // Login handlers
            services.AddSingleton<IHandler, NewConnectionHandler>();

            services.AddSingleton<IHandlerSelector, LoginHandlerSelector>();
        }
    }
}
