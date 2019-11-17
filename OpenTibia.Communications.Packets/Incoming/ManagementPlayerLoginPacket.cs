// <copyright file="ManagementPlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Net;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class ManagementPlayerLoginPacket : IIncomingPacket, IManagementPlayerLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementPlayerLoginPacket"/> class.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="characterName"></param>
        /// <param name="password"></param>
        /// <param name="ipAddress"></param>
        public ManagementPlayerLoginPacket(uint accountNumber, string characterName, string password, string ipAddress)
        {
            this.AccountNumber = accountNumber;
            this.CharacterName = characterName;
            this.Password = password;
            this.IpAddress = IPAddress.Parse(ipAddress);
        }

        public uint AccountNumber { get; }

        public string CharacterName { get; }

        public string Password { get; }

        public IPAddress IpAddress { get; }
    }
}
