// <copyright file="LookAtHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class LookAtHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public LookAtHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.LookAt;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var lookAtInfo = message.ReadLookAtInfo();

            IThing thing = null;

            if (!(this.Game.GetCreatureWithId(connection.PlayerId) is IPlayer player))
            {
                return;
            }

            Console.WriteLine($"LookAt {lookAtInfo.ThingId}.");

            if (lookAtInfo.Location.Type != LocationType.Ground || player.CanSee(lookAtInfo.Location))
            {
                // Get thing at location
                switch (lookAtInfo.Location.Type)
                {
                    case LocationType.Ground:
                        thing = this.Game.GetTileAt(lookAtInfo.Location).GetThingAtStackPosition(lookAtInfo.StackPosition);
                        break;
                    case LocationType.Container:
                        // TODO: implement containers.
                        // Container container = player.Inventory.GetContainer(location.Container);
                        // if (container != null)
                        // {
                        //    return container.GetItem(location.ContainerPosition);
                        // }
                        break;
                    case LocationType.Slot:
                        thing = player.Inventory[(byte)lookAtInfo.Location.Slot];
                        break;
                }

                if (thing != null)
                {
                    this.ResponsePackets.Add(new TextMessagePacket(MessageType.DescriptionGreen, $"You see {thing.InspectionText}."));
                }
            }
        }
    }
}