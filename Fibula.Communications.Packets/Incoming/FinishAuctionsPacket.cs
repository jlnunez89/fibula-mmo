// -----------------------------------------------------------------
// <copyright file="FinishAuctionsPacket.cs" company="2Dudes">
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
