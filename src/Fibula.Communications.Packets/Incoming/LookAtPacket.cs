// -----------------------------------------------------------------
// <copyright file="LookAtPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a look at packet.
    /// </summary>
    public class LookAtPacket : IIncomingPacket, ILookAtInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtPacket"/> class.
        /// </summary>
        /// <param name="location">The location we're looking at.</param>
        /// <param name="thingId">The id of the thing being looked at.</param>
        /// <param name="stackPos">The position in the stack of the thing being looked at.</param>
        public LookAtPacket(Location location, ushort thingId, byte stackPos)
        {
            this.Location = location;
            this.ThingId = thingId;
            this.StackPosition = stackPos;
        }

        /// <summary>
        /// Gets the location of the thing being looked at.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the if of the thing being looked at.
        /// </summary>
        public ushort ThingId { get; }

        /// <summary>
        /// Gets the position in the stack of the thing being looked at.
        /// </summary>
        public byte StackPosition { get; }
    }
}
