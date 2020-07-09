// -----------------------------------------------------------------
// <copyright file="RemoveAtLocationPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a remove at positiion packet.
    /// </summary>
    public class RemoveAtLocationPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAtLocationPacket"/> class.
        /// </summary>
        /// <param name="location">The location at which the removal happened.</param>
        /// <param name="stackpos">The position in the stack within the location at which the removal happened.</param>
        public RemoveAtLocationPacket(Location location, byte stackpos)
        {
            this.Location = location;
            this.Stackpos = stackpos;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.RemoveThing;

        /// <summary>
        /// Gets the location at which the removal happened.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the position in the stack within the location at which the removal happened.
        /// </summary>
        public byte Stackpos { get; }
    }
}
