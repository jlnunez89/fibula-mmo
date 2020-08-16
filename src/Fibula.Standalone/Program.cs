// -----------------------------------------------------------------
// <copyright file="Program.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Standalone
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
    using Fibula.Creatures.MonstersDbFile;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.InMemoryDatabase;
    using Fibula.Data.Loaders.MonFiles;
    using Fibula.Data.Loaders.ObjectsFile;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.SectorFiles;
    using Fibula.Mechanics;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Handlers;
    using Fibula.Mechanics.Operations;
    using Fibula.PathFinding.AStar;
    using Fibula.Protocol.V772.Extensions;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Security;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
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
                .UseSerilog((context, services, loggerConfig) =>
                {
                    var telemetryConfigOptions = services.GetRequiredService<IOptions<TelemetryConfiguration>>();

                    loggerConfig.ReadFrom.Configuration(configuration)
                                .WriteTo.ApplicationInsights(telemetryConfigOptions.Value, TelemetryConverter.Traces, Serilog.Events.LogEventLevel.Information)
                                .Enrich.FromLogContext();
                })
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

                clientMap.Add(connection, new Client(Log.Logger, connection));

                connection.PacketReady += OnPacketReady;

                connection.Closed += OnConnectionClosed;
            }

            foreach (var tcpListener in host.Services.GetServices<IListener>())
            {
                tcpListener.NewConnection += OnNewConnection;
            }

            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;

            await host.RunAsync(Program.MasterCancellationTokenSource.Token).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles an unobserved task exception.
        /// </summary>
        /// <param name="sender">The sender of the exception.</param>
        /// <param name="e">The exception arguments.</param>
        private static void HandleUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
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

            services.AddApplicationInsightsTelemetryWorkerService();

            // Add known instances of configuration and logger.
            services.AddSingleton(hostingContext.Configuration);
            services.AddSingleton(Log.Logger);

            services.AddSingleton<IApplicationContext, ApplicationContext>();
            services.AddSingleton<IScheduler, Scheduler>();

            services.AddSingleton<IGame, Game>();

            services.AddSimpleDosDefender(hostingContext.Configuration);
            services.AddLocalPemFileRsaDecryptor(hostingContext.Configuration);

            services.AddHandlers();

            ConfigureMap(hostingContext, services);

            ConfigureItems(hostingContext, services);

            ConfigureCreatures(hostingContext, services);

            ConfigureDatabaseContext(hostingContext, services);

            ConfigureOperations(services);

            ConfigureHostedServices(services);

            // ConfigureEventRules(hostingContext, services);
            ConfigurePathFindingAlgorithm(hostingContext, services);
            ConfigureExtraServices(hostingContext, services);

            // Choose a server version here.
            services.AddProtocol772GameServerComponents(hostingContext.Configuration);
            services.AddProtocol772GatewayServerComponents(hostingContext.Configuration);
        }

        private static void AddHandlers(this IServiceCollection services)
        {
            var packetTypeToHandlersMap = new Dictionary<Type, Type>()
            {
                { typeof(IAttackInfo), typeof(AttackHandler) },
                { typeof(IAutoMovementInfo), typeof(AutoMoveHandler) },
                { typeof(IActionWithoutContentInfo), typeof(ActionWithoutContentHandler) },
                { typeof(IBytesInfo), typeof(DefaultRequestHandler) },
                { typeof(IFollowInfo), typeof(FollowHandler) },
                { typeof(IGameLogInInfo), typeof(GameLogInHandler) },
                { typeof(IGatewayLoginInfo), typeof(GatewayLogInHandler) },
                { typeof(ILookAtInfo), typeof(LookAtHandler) },
                { typeof(IModesInfo), typeof(ModesHandler) },
                { typeof(ISpeechInfo), typeof(SpeechHandler) },
                { typeof(ITurnOnDemandInfo), typeof(TurnOnDemandHandler) },
                { typeof(IWalkOnDemandInfo), typeof(WalkOnDemandHandler) },
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
            services.AddSingleton<IOperationContext, OperationContext>();
            services.AddSingleton<IElevatedOperationContext, ElevatedOperationContext>();
        }

        private static void ConfigureHostedServices(IServiceCollection services)
        {
            services.AddSingleton<Game>();
            services.AddSingleton<IGame>(s => s.GetService<Game>());

            // Those executing should derive from IHostedService and be added using AddHostedService.
            services.AddHostedService(s => s.GetService<Game>());
        }

        private static void ConfigureDatabaseContext(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Chose a type of Database context:
            // services.AddCosmosDBDatabaseContext(hostingContext.Configuration);
            services.AddInMemoryDatabaseContext(hostingContext.Configuration);

            // IFibulaDbContext itself is added by the Add<DatabaseProvider>() call above.
            // We add Func<IFibulaDbContext> to let callers retrieve a transient instance of this from the Application context,
            // rather than save an actual copy of the DB context in the app context.
            services.AddSingleton<Func<IFibulaDbContext>>(s => s.GetService<IFibulaDbContext>);
        }

        private static void ConfigurePathFindingAlgorithm(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddAStarPathFinder(hostingContext.Configuration);
        }

        private static void ConfigureCreatures(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddSingleton<ICreatureFactory, CreatureFactory>();
            services.AddSingleton<ICreatureManager, CreatureManager>();
            services.AddSingleton<ICreatureFinder>(s => s.GetService<ICreatureManager>());

            // Chose a type of monster types (catalog) loader:
            services.AddMonFilesMonsterTypeLoader(hostingContext.Configuration);

            // Chose a type of monster spawns loader:
            services.AddMonsterDbFileMonsterSpawnLoader(hostingContext.Configuration);
        }

        private static void ConfigureItems(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Note: A IPredefinedItemSet component must be registered by the protocol version.
            services.AddSingleton<IItemFactory, ItemFactory>();

            services.AddSingleton<IContainerManager, ContainerManager>();

            // Chose a type of item types (catalog) loader:
            services.AddObjectsFileItemTypeLoader(hostingContext.Configuration);
        }

        private static void ConfigureMap(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Note that IProtocolTileDescriptor implementations are, by definition, protocol specific and
            // should be injected by the protocol library selected.
            services.AddSingleton<IMap, Map>();
            services.AddSingleton<IMapDescriptor, MapDescriptor>();

            // Choose a type of map loader:
            // services.AddGrassOnlyDummyMapLoader(hostingContext.Configuration);
            // services.AddOtbmMapLoader(hostingContext.Configuration);
            services.AddSectorFilesMapLoader(hostingContext.Configuration);
        }

        private static void ConfigureExtraServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            /*
            // Azure providers for Azure VM hosting and storing secrets in KeyVault.
            services.AddAzureProviders(hostingContext.Configuration);
            */

            services.Configure<TelemetryConfiguration>(hostingContext.Configuration.GetSection(nameof(TelemetryConfiguration)));
        }

        // private static void ConfigureEventRules(HostBuilderContext hostingContext, IServiceCollection services)
        // {
        //    services.AddSingleton<IEventRulesApi>(s => s.GetService<Game>());

        //// Chose a type of event rules loader:
        //    services.AddMoveUseEventRulesLoader(hostingContext.Configuration);
        // }
    }
}
