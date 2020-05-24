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

namespace OpenTibia.Communications.Protocol772
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Protocol772.Handlers;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class ConfigurationRootExtensions
    {
        /// <summary>
        /// Adds the <see cref="GameHandlerSelector"/> implementation of <see cref="IHandlerSelector"/> contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddGameHandlerSelector(this IServiceCollection services)
        {
            services.TryAddSingleton<GameHandlerSelector>();
            services.TryAddSingleton<IHandlerSelector>(s => s.GetService<GameHandlerSelector>());
        }

        /// <summary>
        /// Adds the <see cref="LoginHandlerSelector"/> implementation of <see cref="IHandlerSelector"/> contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void AddLoginHandlerSelector(this IServiceCollection services)
        {
            services.TryAddSingleton<LoginHandlerSelector>();
            services.TryAddSingleton<IHandlerSelector>(s => s.GetService<LoginHandlerSelector>());
        }

        /// <summary>
        /// Adds the <see cref="GameListener{T}"/> contained in this library to the services collection.
        /// It also configures the <see cref="GameListenerOptions"/> required by the listener.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddGameListener(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            AddGameHandlerSelector(services);

            // Configure the options required by the services we're about to add.
            services.Configure<GameListenerOptions>(configuration.GetSection(nameof(GameListenerOptions)));

            services.TryAddSingleton<GameListener<GameHandlerSelector>>();
            services.TryAddSingleton<IListener>(s => s.GetService<GameListener<GameHandlerSelector>>());

            // Since they are derived from IHostedService should be also registered as such.
            services.AddHostedService(s => s.GetService<GameListener<GameHandlerSelector>>());
        }

        /// <summary>
        /// Adds the <see cref="LoginListener{T}"/> contained in this library to the services collection.
        /// It also configures the <see cref="LoginListenerOptions"/> required by the listener.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddLoginListener(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            AddLoginHandlerSelector(services);

            // Configure the options required by the services we're about to add.
            services.Configure<LoginListenerOptions>(configuration.GetSection(nameof(LoginListenerOptions)));

            services.TryAddSingleton<LoginListener<LoginHandlerSelector>>();
            services.TryAddSingleton<IListener>(s => s.GetService<LoginListener<LoginHandlerSelector>>());

            // Since they are derived from IHostedService should be also registered as such.
            services.AddHostedService(s => s.GetService<LoginListener<LoginHandlerSelector>>());
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
