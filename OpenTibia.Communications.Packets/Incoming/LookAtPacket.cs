// <copyright file="LookAtPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class LookAtPacket : IIncomingPacket, ILookAtInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtPacket"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="thingId"></param>
        /// <param name="stackPos"></param>
        public LookAtPacket(Location location, ushort thingId, byte stackPos)
        {
            this.Location = location;
            this.ThingId = thingId;
            this.StackPosition = stackPos;
        }

        public Location Location { get; }

        public ushort ThingId { get; }

        public byte StackPosition { get; }
    }
}
