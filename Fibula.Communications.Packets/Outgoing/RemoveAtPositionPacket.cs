// -----------------------------------------------------------------
// <copyright file="RemoveAtPositionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    public class RemoveAtPositionPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAtPositionPacket"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="stackpos"></param>
        public RemoveAtPositionPacket(Location location, byte stackpos)
        {
            this.Location = location;
            this.Stackpos = stackpos;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.RemoveAtStackpos;

        public Location Location { get; }

        public byte Stackpos { get; }
    }
}
