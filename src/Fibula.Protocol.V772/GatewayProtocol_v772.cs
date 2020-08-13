// -----------------------------------------------------------------
// <copyright file="GatewayProtocol_v772.cs" company="2Dudes">
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
    /// Class that represents a gateway protocol for version 7.72.
    /// </summary>
    public class GatewayProtocol_v772 : IProtocol
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
        /// Initializes a new instance of the <see cref="GatewayProtocol_v772"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public GatewayProtocol_v772(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.logger = logger.ForContext<GatewayProtocol_v772>();

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
                0x01 => IncomingPacketType.LogIn,
                0xFF => IncomingPacketType.ServerStatus,
                _ => IncomingPacketType.Unsupported,
            };
        }
    }
}
