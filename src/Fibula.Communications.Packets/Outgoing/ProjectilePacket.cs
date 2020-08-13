// -----------------------------------------------------------------
// <copyright file="ProjectilePacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a projectile packet.
    /// </summary>
    public class ProjectilePacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePacket"/> class.
        /// </summary>
        /// <param name="fromLocation">The location from which the projectile is launched.</param>
        /// <param name="toLocation">The location to which the projectile impacts.</param>
        /// <param name="projectileType">The projectile type.</param>
        public ProjectilePacket(Location fromLocation, Location toLocation, ProjectileType projectileType)
        {
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.Effect = projectileType;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.ProjectileEffect;

        /// <summary>
        /// Gets the location of origin.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the location of impact.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the actual projectile effect.
        /// </summary>
        public ProjectileType Effect { get; }
    }
}
