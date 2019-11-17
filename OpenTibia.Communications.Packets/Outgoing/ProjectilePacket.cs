// <copyright file="ProjectilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class ProjectilePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="shootType"></param>
        public ProjectilePacket(Location fromLocation, Location toLocation, ProjectileType shootType)
        {
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.ShootType = shootType;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.CreatureMoved;

        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public ProjectileType ShootType { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteProjectilePacket(this);
        }
    }
}
