// <copyright file="FinishAuctionsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;

    public class FinishAuctionsPacket : IIncomingPacket//, IFinishAuctionsInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishAuctionsPacket"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FinishAuctionsPacket(INetworkMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
