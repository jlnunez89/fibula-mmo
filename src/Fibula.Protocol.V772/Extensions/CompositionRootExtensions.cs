// -----------------------------------------------------------------
// <copyright file="CompositionRootExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.Extensions
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Listeners;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Protocol.V772.PacketReaders;
    using Fibula.Protocol.V772.PacketWriters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class CompositionRootExtensions
    {
        /// <summary>
        /// Adds all the game server components related to protocol 7.72 contained in this library to the services collection.
        /// It also configures any <see cref="IOptions{T}"/> required by any such components.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddProtocol772GameServerComponents(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            // Configure the options required by the services we're about to add.
            services.Configure<GameListenerOptions>(configuration.GetSection(nameof(GameListenerOptions)));

            // Add all handlers
            services.TryAddSingleton<GameLogInPacketReader>();

            var packetReadersToAdd = new Dictionary<IncomingPacketType, Type>()
            {
                { IncomingPacketType.Attack, typeof(AttackPacketReader) },
                { IncomingPacketType.AutoMove, typeof(AutoMovePacketReader) },
                { IncomingPacketType.AutoMoveCancel, typeof(AutoMoveCancelPacketReader) },
                { IncomingPacketType.ChangeModes, typeof(ChangeModesPacketReader) },
                { IncomingPacketType.Follow, typeof(FollowPacketReader) },
                { IncomingPacketType.Heartbeat, typeof(HeartbeatPacketReader) },
                { IncomingPacketType.HeartbeatResponse, typeof(HeartbeatResponsePacketReader) },
                { IncomingPacketType.LogIn, typeof(GameLogInPacketReader) },
                { IncomingPacketType.LogOut, typeof(GameLogOutPacketReader) },
                { IncomingPacketType.LookAt, typeof(LookAtPacketReader) },
                { IncomingPacketType.Speech, typeof(SpeechPacketReader) },
                { IncomingPacketType.StopAllActions, typeof(StopAllActionsPacketReader) },
                { IncomingPacketType.TurnNorth, typeof(TurnNorthPacketReader) },
                { IncomingPacketType.TurnEast, typeof(TurnEastPacketReader) },
                { IncomingPacketType.TurnSouth, typeof(TurnSouthPacketReader) },
                { IncomingPacketType.TurnWest, typeof(TurnWestPacketReader) },
                { IncomingPacketType.WalkNorth, typeof(WalkNorthPacketReader) },
                { IncomingPacketType.WalkNortheast, typeof(WalkNortheastPacketReader) },
                { IncomingPacketType.WalkEast, typeof(WalkEastPacketReader) },
                { IncomingPacketType.WalkSoutheast, typeof(WalkSoutheastPacketReader) },
                { IncomingPacketType.WalkSouth, typeof(WalkSouthPacketReader) },
                { IncomingPacketType.WalkSouthwest, typeof(WalkSouthwestPacketReader) },
                { IncomingPacketType.WalkWest, typeof(WalkWestPacketReader) },
                { IncomingPacketType.WalkNorthwest, typeof(WalkNorthwestPacketReader) },
            };

            foreach (var (packetType, type) in packetReadersToAdd)
            {
                services.TryAddSingleton(type);
            }

            var packetWritersToAdd = new Dictionary<OutgoingPacketType, Type>()
            {
                { OutgoingPacketType.AnimatedText, typeof(AnimatedTextPacketWriter) },
                { OutgoingPacketType.AddThing, typeof(AddCreaturePacketWriter) },
                { OutgoingPacketType.ContainerClose, typeof(ContainerClosePacketWriter) },
                { OutgoingPacketType.ContainerOpen, typeof(ContainerOpenPacketWriter) },
                { OutgoingPacketType.ContainerAddItem, typeof(ContainerAddItemPacketWriter) },
                { OutgoingPacketType.ContainerRemoveItem, typeof(ContainerRemoveItemPacketWriter) },
                { OutgoingPacketType.ContainerUpdateItem, typeof(ContainerUpdateItemPacketWriter) },
                { OutgoingPacketType.CreatureHealth, typeof(CreatureHealthPacketWriter) },
                { OutgoingPacketType.CreatureLight, typeof(CreatureLightPacketWriter) },
                { OutgoingPacketType.CreatureMoved, typeof(CreatureMovedPacketWriter) },
                { OutgoingPacketType.UpdateThing, typeof(CreatureTurnedPacketWriter) },
                { OutgoingPacketType.CreatureSpeech, typeof(CreatureSpeechPacketWriter) },
                { OutgoingPacketType.GameDisconnect, typeof(GameServerDisconnectPacketWriter) },
                { OutgoingPacketType.Heartbeat, typeof(HeartbeatPacketWriter) },
                { OutgoingPacketType.HeartbeatResponse, typeof(HeartbeatResponsePacketWriter) },
                { OutgoingPacketType.MagicEffect, typeof(MagicEffectPacketWriter) },
                { OutgoingPacketType.MapDescription, typeof(MapDescriptionPacketWriter) },
                { OutgoingPacketType.MapSliceNorth, typeof(MapPartialDescriptionPacketWriter) },
                { OutgoingPacketType.MapSliceEast, typeof(MapPartialDescriptionPacketWriter) },
                { OutgoingPacketType.MapSliceSouth, typeof(MapPartialDescriptionPacketWriter) },
                { OutgoingPacketType.MapSliceWest, typeof(MapPartialDescriptionPacketWriter) },
                { OutgoingPacketType.PlayerConditions, typeof(PlayerConditionsPacketWriter) },
                { OutgoingPacketType.InventoryEmpty, typeof(PlayerInventoryClearSlotPacketWriter) },
                { OutgoingPacketType.InventoryItem, typeof(PlayerInventorySetSlotPacketWriter) },
                { OutgoingPacketType.PlayerSkills, typeof(PlayerSkillsPacketWriter) },
                { OutgoingPacketType.PlayerStats, typeof(PlayerStatsPacketWriter) },
                { OutgoingPacketType.ProjectileEffect, typeof(ProjectilePacketWriter) },
                { OutgoingPacketType.RemoveThing, typeof(RemoveAtPositionPacketWriter) },
                { OutgoingPacketType.Square, typeof(SquarePacketWriter) },
                { OutgoingPacketType.PlayerLogin, typeof(PlayerLoginPacketWriter) },
                { OutgoingPacketType.TextMessage, typeof(TextMessagePacketWriter) },
                { OutgoingPacketType.TileUpdate, typeof(TileUpdatePacketWriter) },
                { OutgoingPacketType.CancelAttack, typeof(PlayerCancelAttackPacketWriter) },
                { OutgoingPacketType.CancelWalk, typeof(PlayerCancelWalkPacketWriter) },
                { OutgoingPacketType.WorldLight, typeof(WorldLightPacketWriter) },
            };

            foreach (var (packetType, type) in packetWritersToAdd)
            {
                services.TryAddSingleton(type);
            }

            services.AddSingleton(s =>
            {
                var protocol = new GameProtocol_v772(s.GetRequiredService<ILogger>());

                foreach (var (packetType, type) in packetReadersToAdd)
                {
                    protocol.RegisterPacketReader(packetType, s.GetRequiredService(type) as IPacketReader);
                }

                foreach (var (packetType, type) in packetWritersToAdd)
                {
                    protocol.RegisterPacketWriter(packetType, s.GetRequiredService(type) as IPacketWriter);
                }

                return protocol;
            });

            services.TryAddSingleton<IProtocolTileDescriptor, TileDescriptor_v772>();

            services.TryAddSingleton<IPredefinedItemSet, PredefinedItemSet_v772>();

            services.TryAddSingleton<ClientConnectionFactory<GameProtocol_v772>>();
            services.TryAddSingleton<ISocketConnectionFactory>(s => s.GetService<ClientConnectionFactory<GameProtocol_v772>>());

            services.TryAddSingleton<GameListener<ClientConnectionFactory<GameProtocol_v772>>>();
            services.AddSingleton<IListener>(s => s.GetService<GameListener<ClientConnectionFactory<GameProtocol_v772>>>());

            // Since they are derived from IHostedService should be also registered as such.
            services.AddHostedService(s => s.GetService<GameListener<ClientConnectionFactory<GameProtocol_v772>>>());
        }

        /// <summary>
        /// Adds all the gateway server components related to protocol 7.72 contained in this library to the services collection.
        /// It also configures any <see cref="IOptions{T}"/> required by any such components.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddProtocol772GatewayServerComponents(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            // Configure the options required by the services we're about to add.
            services.Configure<GatewayListenerOptions>(configuration.GetSection(nameof(GatewayListenerOptions)));

            // Add all handlers
            services.TryAddSingleton<GatewayLogInPacketReader>();

            var packetWritersToAdd = new Dictionary<OutgoingPacketType, Type>()
            {
                { OutgoingPacketType.CharacterList, typeof(CharacterListPacketWriter) },
                { OutgoingPacketType.GatewayDisconnect, typeof(GatewayServerDisconnectPacketWriter) },
                { OutgoingPacketType.MessageOfTheDay, typeof(MessageOfTheDayPacketWriter) },
            };

            foreach (var (packetType, type) in packetWritersToAdd)
            {
                services.TryAddSingleton(type);
            }

            services.AddSingleton(s =>
            {
                var protocol = new GatewayProtocol_v772(s.GetRequiredService<ILogger>());

                protocol.RegisterPacketReader(IncomingPacketType.LogIn, s.GetRequiredService<GatewayLogInPacketReader>());

                foreach (var (packetType, type) in packetWritersToAdd)
                {
                    protocol.RegisterPacketWriter(packetType, s.GetRequiredService(type) as IPacketWriter);
                }

                return protocol;
            });

            services.TryAddSingleton<ClientConnectionFactory<GatewayProtocol_v772>>();
            services.TryAddSingleton<ISocketConnectionFactory>(s => s.GetService<ClientConnectionFactory<GatewayProtocol_v772>>());

            services.TryAddSingleton<GatewayListener<ClientConnectionFactory<GatewayProtocol_v772>>>();
            services.AddSingleton<IListener>(s => s.GetService<GatewayListener<ClientConnectionFactory<GatewayProtocol_v772>>>());

            // Since they are derived from IHostedService should be also registered as such.
            services.AddHostedService(s => s.GetService<GatewayListener<ClientConnectionFactory<GatewayProtocol_v772>>>());
        }
    }
}
