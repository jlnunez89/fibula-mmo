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
            services.AddSingleton<IHandler, AttackHandler>();
            services.AddSingleton<IHandler, AutoMoveCancelHandler>();
            services.AddSingleton<IHandler, AutoMoveHandler>();
            services.AddSingleton<IHandler, ContainerCloseHandler>();
            services.AddSingleton<IHandler, ContainerMoveUpHandler>();
            services.AddSingleton<IHandler, KeepAliveRequestHandler>();
            services.AddSingleton<IHandler, LogInHandler>();
            services.AddSingleton<IHandler, LogOutHandler>();
            services.AddSingleton<IHandler, LookAtHandler>();
            services.AddSingleton<IHandler, MoveThingHandler>();
            services.AddSingleton<IHandler, RotateItemHandler>();
            services.AddSingleton<IHandler, SetModeHandler>();
            services.AddSingleton<IHandler, SpeechHandler>();
            services.AddSingleton<IHandler, StopAllActionsHandler>();
            services.AddSingleton<IHandler, TurnEastHandler>();
            services.AddSingleton<IHandler, TurnNorthHandler>();
            services.AddSingleton<IHandler, TurnSouthHandler>();
            services.AddSingleton<IHandler, TurnWestHandler>();
            services.AddSingleton<IHandler, UseItemHandler>();
            services.AddSingleton<IHandler, UseItemOnHandler>();
            services.AddSingleton<IHandler, WalkEastHandler>();
            services.AddSingleton<IHandler, WalkNortheastHandler>();
            services.AddSingleton<IHandler, WalkNorthHandler>();
            services.AddSingleton<IHandler, WalkNorthWestHandler>();
            services.AddSingleton<IHandler, WalkSoutheastHandler>();
            services.AddSingleton<IHandler, WalkSouthHandler>();
            services.AddSingleton<IHandler, WalkSouthwestHandler>();
            services.AddSingleton<IHandler, WalkWestHandler>();

            //services.AddSingleton<IHandler, ChannelCloseHandler>();
            //services.AddSingleton<IHandler, ChannelCreateOwnHandler>();
            //services.AddSingleton<IHandler, ChannelInviteHandler>();
            //services.AddSingleton<IHandler, ChannelKickHandler>();
            //services.AddSingleton<IHandler, ChannelListRequestHandler>();
            //services.AddSingleton<IHandler, ChannelOpenPrivateHandler>();
            //services.AddSingleton<IHandler, ChannelOpenPublicHandler>();
            //services.AddSingleton<IHandler, FollowHandler>();
            //services.AddSingleton<IHandler, HouseWindowPostHandler>();
            //services.AddSingleton<IHandler, ItemUseBattleHandler>();
            //services.AddSingleton<IHandler, OutfitChangedHandler>();
            //services.AddSingleton<IHandler, OutfitChangeRequestHandler>();
            //services.AddSingleton<IHandler, PartyAcceptHandler>();
            //services.AddSingleton<IHandler, PartyLeaveHandler>();
            //services.AddSingleton<IHandler, PartyPassLeadershipHandler>();
            //services.AddSingleton<IHandler, PartyRejectHandler>();
            //services.AddSingleton<IHandler, PartyRequestHandler>();
            //services.AddSingleton<IHandler, ReportBugHandler>();
            //services.AddSingleton<IHandler, ReportCancelHandler>();
            //services.AddSingleton<IHandler, ReportCloseHandler>();
            //services.AddSingleton<IHandler, ReportDebugAssertionHandler>();
            //services.AddSingleton<IHandler, ReportProcessHandler>();
            //services.AddSingleton<IHandler, ReportViolationHandler>();
            //services.AddSingleton<IHandler, ReSendContainerRequestHandler>();
            //services.AddSingleton<IHandler, ReSendTileRequestHandler>();
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
