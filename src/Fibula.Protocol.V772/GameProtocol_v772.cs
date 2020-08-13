// -----------------------------------------------------------------
// <copyright file="GameProtocol_v772.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a game protocol for version 7.72.
    /// </summary>
    public class GameProtocol_v772 : IProtocol
    {
        /// <summary>
        /// Stores a reference to the logger in use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The known packet readers to pick from.
        /// </summary>
        private readonly IDictionary<IncomingPacketType, IPacketReader> packetReadersMap;

        /// <summary>
        /// The known packet writers to pick from.
        /// </summary>
        private readonly IDictionary<OutgoingPacketType, IPacketWriter> packetWritersMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameProtocol_v772"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public GameProtocol_v772(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.logger = logger.ForContext<GameProtocol_v772>();

            this.packetReadersMap = new Dictionary<IncomingPacketType, IPacketReader>();
            this.packetWritersMap = new Dictionary<OutgoingPacketType, IPacketWriter>();
        }

        /// <summary>
        /// Registers a packet reader to this protocol.
        /// </summary>
        /// <param name="forType">The type of packet to register for.</param>
        /// <param name="packetReader">The packet reader to register.</param>
        public void RegisterPacketReader(IncomingPacketType forType, IPacketReader packetReader)
        {
            packetReader.ThrowIfNull(nameof(packetReader));

            if (this.packetReadersMap.ContainsKey(forType))
            {
                throw new InvalidOperationException($"There is already a reader registered for the packet type {forType}.");
            }

            this.logger.Verbose($"Registered packet reader for type {forType}.");

            this.packetReadersMap[forType] = packetReader;
        }

        /// <summary>
        /// Registers a packet writer to this protocol.
        /// </summary>
        /// <param name="forType">The type of packet to register for.</param>
        /// <param name="packetWriter">The packet writer to register.</param>
        public void RegisterPacketWriter(OutgoingPacketType forType, IPacketWriter packetWriter)
        {
            packetWriter.ThrowIfNull(nameof(packetWriter));

            if (this.packetWritersMap.ContainsKey(forType))
            {
                throw new InvalidOperationException($"There is already a writer registered for the packet type: {forType}.");
            }

            this.logger.Verbose($"Registered packet writer for type {forType}.");

            this.packetWritersMap[forType] = packetWriter;
        }

        /// <summary>
        /// Selects the most appropriate packet reader for the specified type.
        /// </summary>
        /// <param name="forPacketType">The type of packet.</param>
        /// <returns>An instance of an <see cref="IPacketReader"/> implementation.</returns>
        public IPacketReader SelectPacketReader(IncomingPacketType forPacketType)
        {
            if (this.packetReadersMap.TryGetValue(forPacketType, out IPacketReader reader))
            {
                return reader;
            }

            return null;
        }

        /// <summary>
        /// Selects the most appropriate packet writer for the specified type.
        /// </summary>
        /// <param name="forPacketType">The type of packet.</param>
        /// <returns>An instance of an <see cref="IPacketWriter"/> implementation.</returns>
        public IPacketWriter SelectPacketWriter(OutgoingPacketType forPacketType)
        {
            if (this.packetWritersMap.TryGetValue(forPacketType, out IPacketWriter writer))
            {
                return writer;
            }

            return null;
        }

        /// <summary>
        /// Attempts to convert a byte value into an <see cref="IncomingPacketType"/>.
        /// </summary>
        /// <param name="fromByte">The byte to convert.</param>
        /// <returns>The <see cref="IncomingPacketType"/> value converted to.</returns>
        public IncomingPacketType ByteToIncomingPacketType(byte fromByte)
        {
            return fromByte switch
            {
                0x0A => IncomingPacketType.LogIn,
                0x14 => IncomingPacketType.LogOut,
                0x1D => IncomingPacketType.HeartbeatResponse,
                0x1E => IncomingPacketType.Heartbeat,
                0x64 => IncomingPacketType.AutoMove,
                0x65 => IncomingPacketType.WalkNorth,
                0x66 => IncomingPacketType.WalkEast,
                0x67 => IncomingPacketType.WalkSouth,
                0x68 => IncomingPacketType.WalkWest,
                0x69 => IncomingPacketType.AutoMoveCancel,
                0x6A => IncomingPacketType.WalkNortheast,
                0x6B => IncomingPacketType.WalkSoutheast,
                0x6C => IncomingPacketType.WalkSouthwest,
                0x6D => IncomingPacketType.WalkNorthwest,
                0x6F => IncomingPacketType.TurnNorth,
                0x70 => IncomingPacketType.TurnEast,
                0x71 => IncomingPacketType.TurnSouth,
                0x72 => IncomingPacketType.TurnWest,
                0x78 => IncomingPacketType.MoveThing,
                0x7D => IncomingPacketType.TradeRequest,
                0x7E => IncomingPacketType.TradeLook,
                0x7F => IncomingPacketType.TradeAccept,
                0x80 => IncomingPacketType.TradeCancel,
                0x82 => IncomingPacketType.ItemUse,
                0x83 => IncomingPacketType.ItemUseOn,
                0x84 => IncomingPacketType.ItemUseThroughBattleWindow,
                0x85 => IncomingPacketType.ItemRotate,
                0x87 => IncomingPacketType.ContainerClose,
                0x88 => IncomingPacketType.ContainerUp,
                0x89 => IncomingPacketType.WindowText,
                0x8A => IncomingPacketType.WindowHouse,
                0x8C => IncomingPacketType.LookAt,
                0x8D => IncomingPacketType.LookThroughBattleWindow,
                0x96 => IncomingPacketType.Speech,
                0x97 => IncomingPacketType.ChannelListRequest,
                0x98 => IncomingPacketType.ChannelOpen,
                0x99 => IncomingPacketType.ChannelClose,
                0x9A => IncomingPacketType.ChannelOpenDirect,
                0x9B => IncomingPacketType.ReportStart,
                0x9C => IncomingPacketType.ReportClose,
                0x9D => IncomingPacketType.ReportCancel,
                0xA0 => IncomingPacketType.ChangeModes,
                0xA1 => IncomingPacketType.Attack,
                0xA2 => IncomingPacketType.Follow,
                0xA3 => IncomingPacketType.PartyInvite,
                0xA4 => IncomingPacketType.PartyAcceptInvitation,
                0xA5 => IncomingPacketType.PartyKick,
                0xA6 => IncomingPacketType.PartyPassLeadership,
                0xA7 => IncomingPacketType.PartyLeave,
                0xAA => IncomingPacketType.ChannelCreatePrivate,
                0xAB => IncomingPacketType.ChannelInvite,
                0xAC => IncomingPacketType.ChannelExclude,
                0xBE => IncomingPacketType.StopAllActions,
                0xC9 => IncomingPacketType.ResendTile,
                0xCA => IncomingPacketType.ResendContainer,
                0xCC => IncomingPacketType.FindInContainer,
                0xD2 => IncomingPacketType.StartOutfitChange,
                0xD3 => IncomingPacketType.SubmitOutfitChange,
                0xDC => IncomingPacketType.AddVip,
                0xDD => IncomingPacketType.RemoveVip,
                0xE6 => IncomingPacketType.ReportBug,
                0xE7 => IncomingPacketType.ReportViolation,
                0xE8 => IncomingPacketType.ReportDebugAssertion,
                _ => IncomingPacketType.Unsupported,
            };
        }
    }
}
