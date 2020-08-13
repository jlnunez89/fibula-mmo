// -----------------------------------------------------------------
// <copyright file="UseItemOnPacket.cs" company="2Dudes">
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

    public class UseItemOnPacket : IIncomingPacket, IUseItemOnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOnPacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="fromItemId"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="toItemId"></param>
        /// <param name="toStackPos"></param>
        public UseItemOnPacket(Location fromLocation, ushort fromItemId, byte fromStackPos, Location toLocation, ushort toItemId, byte toStackPos)
        {
            this.FromLocation = fromLocation;
            this.FromItemClientId = fromItemId;
            this.FromIndex = fromStackPos;

            this.ToLocation = toLocation;
            this.ToItemClientId = toItemId;
            this.ToIndex = toStackPos;
        }

        public Location FromLocation { get; }

        public ushort FromItemClientId { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public ushort ToItemClientId { get; }

        public byte ToIndex { get; }
    }
}
