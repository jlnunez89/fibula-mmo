// -----------------------------------------------------------------
// <copyright file="ManagementPlayerLogoutPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    public class ManagementPlayerLogoutPacket : IIncomingPacket, IManagementPlayerLogoutInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementPlayerLogoutPacket"/> class.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="level"></param>
        /// <param name="vocation"></param>
        /// <param name="residence"></param>
        /// <param name="lastLogin"></param>
        public ManagementPlayerLogoutPacket(uint accountId, ushort level, string vocation, string residence, DateTimeOffset lastLogin)
        {
            this.AccountId = accountId;
            this.Level = level;
            this.Vocation = vocation;
            this.Residence = residence;
            this.LastLogin = lastLogin;
        }

        public uint AccountId { get; }

        public ushort Level { get; }

        public string Vocation { get; }

        public string Residence { get; }

        public DateTimeOffset LastLogin { get; }
    }
}
