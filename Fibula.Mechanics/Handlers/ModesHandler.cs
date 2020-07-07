// -----------------------------------------------------------------
// <copyright file="ModesHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Handlers
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for changing modes.
    /// </summary>
    public class ModesHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModesHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public ModesHandler(ILogger logger, IGame gameInstance)
            : base(logger, gameInstance)
        {
        }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client)
        {
            incomingPacket.ThrowIfNull(nameof(incomingPacket));
            client.ThrowIfNull(nameof(client));

            if (!(incomingPacket is IModesInfo modesInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IModesInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            this.Game.CreatureChangeModes(client.PlayerId, modesInfo.FightMode, modesInfo.ChaseMode, modesInfo.SafeModeOn);

            return null;
        }
    }
}
