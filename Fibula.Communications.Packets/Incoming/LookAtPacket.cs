// -----------------------------------------------------------------
// <copyright file="LookAtPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a look at packet.
    /// </summary>
    public class LookAtPacket : ILookAtInfo
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
