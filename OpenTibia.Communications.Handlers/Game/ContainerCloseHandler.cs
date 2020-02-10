// -----------------------------------------------------------------
// <copyright file="ContainerCloseHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for a player closing a container.
    /// </summary>
    public class ContainerCloseHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerCloseHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext"></param>
        public ContainerCloseHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger, operationFactory, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ContainerClose;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var containerInfo = message.ReadContainerCloseInfo();

            if (this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player)
            {
                var item = this.Context.ContainerManager.FindForCreature(player.Id, containerInfo.ContainerId);

                if (item != null)
                {
                    this.ScheduleNewOperation(OperationType.ContainerClose, new CloseContainerOperationCreationArguments(player.Id, player, item, containerInfo.ContainerId));
                }
            }

            return null;
        }
    }
}