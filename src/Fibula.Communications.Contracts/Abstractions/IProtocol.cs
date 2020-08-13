// -----------------------------------------------------------------
// <copyright file="IProtocol.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Interface that contains methods to select the appropriate request and response handlers for a given protocol.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Registers a packet reader to this protocol.
        /// </summary>
        /// <param name="forType">The type of packet to register for.</param>
        /// <param name="packetReader">The packet reader to register.</param>
        void RegisterPacketReader(IncomingPacketType forType, IPacketReader packetReader);

        /// <summary>
        /// Registers a packet writer to this protocol.
        /// </summary>
        /// <param name="forType">The type of packet to register for.</param>
        /// <param name="packetWriter">The packet writer to register.</param>
        void RegisterPacketWriter(OutgoingPacketType forType, IPacketWriter packetWriter);

        /// <summary>
        /// Selects the most appropriate packet reader for the specified type.
        /// </summary>
        /// <param name="forPacketType">The type of packet.</param>
        /// <returns>An instance of an <see cref="IPacketReader"/> implementation.</returns>
        IPacketReader SelectPacketReader(IncomingPacketType forPacketType);

        /// <summary>
        /// Selects the most appropriate packet writer for the specified type.
        /// </summary>
        /// <param name="forPacketType">The type of packet.</param>
        /// <returns>An instance of an <see cref="IPacketWriter"/> implementation.</returns>
        IPacketWriter SelectPacketWriter(OutgoingPacketType forPacketType);

        /// <summary>
        /// Attempts to convert a byte value into a <see cref="IncomingPacketType"/>.
        /// </summary>
        /// <param name="fromByte">The byte to convert.</param>
        /// <returns>The <see cref="IncomingPacketType"/> value converted to.</returns>
        IncomingPacketType ByteToIncomingPacketType(byte fromByte);
    }
}
