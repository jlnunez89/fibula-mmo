// -----------------------------------------------------------------
// <copyright file="Program.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Standalone
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Client;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Models;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Creatures;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.InMemoryDatabase;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.ObjectsFile;
    using Fibula.Map;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.GrassOnly;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Security;
    using Fibula.Server.Mechanics;
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Handlers;
    using Fibula.Server.Operations;
    using Fibula.Server.Operations.Contracts.Abstractions;
    using Fibula.Server.Protocol772;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    /// <summary>
    /// Class that represents a standalone Fibula server instance.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The cancellation token source for the entire application.
        /// </summary>
        private static readonly CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// The handler selector used to bind incoming packets to handlers, and process them.
        /// </summary>
        private static IHandlerSelector handlerSelector;

        /// <summary>
        /// Stores a mapping of conenctions to their respective clients.
        /// </summary>
        private static IDictionary<IConnection, IClient> clientMap;

        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The arguments for this program.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("logsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.ForContext(typeof(Program)).Information("Building host...");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
                    configHost.AddEnvironmentVariables(prefix: "OTS_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configApp.AddEnvironmentVariables(prefix: "OTS_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices(ConfigureServices)
                .UseSerilog()
                .Build();

            // Bind the TCP listener events with handler picking.
            // This is needed for now because we don't have a separate listening pipeline for generic (non TCP client requests).
            handlerSelector = host.Services.GetRequiredService<IHandlerSelector>();
            clientMap = new Dictionary<IConnection, IClient>();

            static IEnumerable<IOutboundPacket> OnPacketReady(IConnection connection, IIncomingPacket packet)
            {
                var handler = handlerSelector.SelectForPacket(packet);

                if (handler != null && clientMap.TryGetValue(connection, out IClient client))
                {
                    return handler.HandleRequest(packet, client);
                }

                return null;
            }

            static void OnConnectionClosed(IConnection connection)
            {
                // Clean up the event listeners we set up here.
                connection.PacketReady -= OnPacketReady;
                connection.Closed -= OnConnectionClosed;

                if (!clientMap.ContainsKey(connection))
                {
                    return;
                }

                clientMap.Remove(connection);
            }

            static void OnNewConnection(IConnection connection)
            {
                if (clientMap.ContainsKey(connection))
                {
                    return;
                }

                clientMap.Add(connection, new TcpClient(connection));

                connection.PacketReady += OnPacketReady;

                connection.Closed += OnConnectionClosed;
            }

            foreach (var tcpListener in host.Services.GetServices<ITcpListener>())
            {
                tcpListener.NewConnection += OnNewConnection;
            }

            await host.RunAsync(Program.MasterCancellationTokenSource.Token).ConfigureAwait(false);

            // Clean up.
            foreach (var tcpListener in host.Services.GetServices<ITcpListener>())
            {
                tcpListener.NewConnection -= OnNewConnection;
            }
        }

        /// <summary>
        /// Configuration root, where services are configured and added into the service collection, often depending on the configuration set.
        /// </summary>
        /// <param name="hostingContext">The hosting context.</param>
        /// <param name="services">The services collection.</param>
        private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            hostingContext.ThrowIfNull(nameof(hostingContext));
            services.ThrowIfNull(nameof(services));

            // Configure options here
            services.Configure<ApplicationContextOptions>(hostingContext.Configuration.GetSection(nameof(ApplicationContextOptions)));

            // Add the master cancellation token source of the entire service.
            services.AddSingleton(Program.MasterCancellationTokenSource);

            // Add known instances of configuration and logger.
            services.AddSingleton(hostingContext.Configuration);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<TelemetryClient>();

            services.AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>();

            services.AddSingleton<IApplicationContext, ApplicationContext>();
            services.AddSingleton<IScheduler, Scheduler>();

            //services.AddSingleton<IConnectionManager, ConnectionManager>();
            //services.AddSingleton<IConnectionFinder>(s => s.GetService<IConnectionManager>());

            services.AddSingleton<IGame, Game>();

            services.AddProtocol772GameComponents(hostingContext.Configuration);
            services.AddProtocol772GatewayComponents(hostingContext.Configuration);

            services.AddSimpleDosDefender(hostingContext.Configuration);
            services.AddLocalPemFileRsaDecryptor(hostingContext.Configuration);

            ConfigureHandlers(hostingContext, services);

            //ConfigureEventRules(hostingContext, services);

            ConfigureMap(hostingContext, services);

            ConfigureItems(hostingContext, services);

            ConfigureCreatures(hostingContext, services);

            //ConfigurePathFindingAlgorithm(hostingContext, services);

            ConfigureDatabaseContext(hostingContext, services);

            ConfigureOperations(services);

            ConfigureHostedServices(services, hostingContext.Configuration);

            //ConfigureExtraServices(hostingContext, services);
        }

        private static void ConfigureHandlers(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Add all handlers
            services.TryAddSingleton<DefaultRequestHandler>();

            var packetTypeToHandlersMap = new Dictionary<Type, Type>()
            {
                { typeof(IActionWithoutContentInfo), typeof(ActionWithoutContentHandler) },
                { typeof(IGameLogInInfo), typeof(GameLogInHandler) },
                { typeof(IGatewayLoginInfo), typeof(GatewayLogInHandler) },
                { typeof(IWalkOnDemandInfo), typeof(WalkOnDemandHandler) },
                { typeof(ISpeechInfo), typeof(SpeechHandler) },
            };

            foreach (var (packetType, type) in packetTypeToHandlersMap)
            {
                services.TryAddSingleton(type);
            }

            services.AddSingleton<IHandlerSelector>(s =>
            {
                var handlerSelector = new HandlerSelector(s.GetRequiredService<ILogger>());

                foreach (var (packetType, type) in packetTypeToHandlersMap)
                {
                    handlerSelector.RegisterForPacketType(packetType, s.GetRequiredService(type) as IHandler);
                }

                return handlerSelector;
            });
        }

        private static void ConfigureOperations(IServiceCollection services)
        {
            services.AddSingleton<IOperationFactory, OperationFactory>();
            services.AddSingleton<IOperationContext, OperationContext>();
            services.AddSingleton<IElevatedOperationContext, ElevatedOperationContext>();
        }

        private static void ConfigureHostedServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Game>();
            services.AddSingleton<IGame>(s => s.GetService<Game>());
            // services.AddSingleton<ICombatApi>(s => s.GetService<Game>());

            // Those executing should derive from IHostedService and be added using AddHostedService.
            services.AddHostedService(s => s.GetService<Game>());
        }

        private static void ConfigureDatabaseContext(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Chose a type of Database context:
            // services.AddCosmosDBDatabaseContext(hostingContext.Configuration);
            services.AddInMemoryDatabaseContext(hostingContext.Configuration);

            // IOpenTibiaDbContext itself is added by the Add<DatabaseProvider>() call above.
            // We add Func<IOpenTibiaDbContext> to let callers retrieve a transient instance of this from the Application context,
            // rather than save an actual copy of the DB context in the app context.
            services.AddSingleton<Func<IFibulaDbContext>>(s => s.GetService<IFibulaDbContext>);
        }

        //private static void ConfigurePathFindingAlgorithm(HostBuilderContext hostingContext, IServiceCollection services)
        //{
        //    services.AddAStarPathFinder(hostingContext.Configuration);
        //}

        private static void ConfigureCreatures(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddSingleton<ICreatureFactory, CreatureFactory>();
            services.AddSingleton<ICreatureManager, CreatureManager>();
            services.AddSingleton<ICreatureFinder>(s => s.GetService<ICreatureManager>());

            //// Chose a type of monster types (catalog) loader:
            //services.AddMonFilesMonsterTypeLoader(hostingContext.Configuration);

            //// Chose a type of monster spawns loader:
            //services.AddMonsterDbFileMonsterSpawnLoader(hostingContext.Configuration);
        }

        private static void ConfigureItems(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddSingleton<IItemFactory, ItemFactory>();

            // Chose a type of item types (catalog) loader:
            services.AddObjectsFileItemTypeLoader(hostingContext.Configuration);
        }

        private static void ConfigureMap(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddSingleton<IMap, Map>();
            services.AddSingleton<IMapDescriptor, MapDescriptor>();
            services.AddSingleton<ITileAccessor>(s => s.GetService<IMap>());

            // Chose a type of map loader:
            //services.AddSectorFilesMapLoader(hostingContext.Configuration);
            services.AddGrassOnlyDummyMapLoader(hostingContext.Configuration);
        }

        //private static void ConfigureExtraServices(HostBuilderContext hostingContext, IServiceCollection services)
        //{
        //    // Azure providers for Azure VM hosting and storing secrets in KeyVault.
        //    services.AddAzureProviders(hostingContext.Configuration);
        //}

        //private static void ConfigureEventRules(HostBuilderContext hostingContext, IServiceCollection services)
        //{
        //    services.AddSingleton<IEventRulesApi>(s => s.GetService<Game>());

        //    // Chose a type of event rules loader:
        //    services.AddMoveUseEventRulesLoader(hostingContext.Configuration);
        //}
    }
}
