// -----------------------------------------------------------------
// <copyright file="ConnectionMessageReadyDelegate.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Delegates
{
    using System.Collections.Generic;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Represents a delegate to call when a packet is ready to be processed.
    /// </summary>
    /// <param name="connection">The connection from which the packet is was read.</param>
    /// <param name="packet">The packet ready to be processed.</param>
    /// <returns>A collection of <see cref="IOutboundPacket"/> to respond with as a result of processing the <see cref="IIncomingPacket"/>.</returns>
    public delegate IEnumerable<IOutboundPacket> ConnectionPacketReadyDelegate(IConnection connection, IIncomingPacket packet);
}
