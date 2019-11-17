// <copyright file="ContainerUpHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;

    public class ContainerUpHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerUpHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public ContainerUpHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ContainerUp;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var containerMoveUpInfo = message.ReadContainerMoveUpInfo();
            var player = this.Game.GetCreatureWithId(connection.PlayerId) as IPlayer;

            var currentContainer = player?.GetContainer(containerMoveUpInfo.ContainerId);

            if (currentContainer?.Parent == null)
            {
                return;
            }

            player?.OpenContainerAt(currentContainer.Parent, containerMoveUpInfo.ContainerId);

            this.ResponsePackets.Add(new ContainerOpenPacket(
                (byte)currentContainer.Parent.GetIdFor(connection.PlayerId),
                currentContainer.Parent.ThingId,
                currentContainer.Parent.Type.Name,
                currentContainer.Parent.Volume,
                currentContainer.Parent.Parent != null,
                currentContainer.Parent.Content));
        }
    }
}