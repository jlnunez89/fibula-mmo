// <copyright file="ClearIsOnlineHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Management
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Data;

    public class ClearIsOnlineHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.ClearIsOnline;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var defaultInfo = message.ReadDefaultInfo();

            using (var unitOfWork = new UnitOfWork(new OpenTibiaContext()))
            {
                //var onlineCharacters = unitOfWork.Characters.Find(c => c.IsOnline).ToList();

                //foreach (var character in onlineCharacters)
                //{
                //    character.Online = 0;
                //}

                //unitOfWork.Complete();

                //this.ResponsePackets.Add(new ClearOnlinePlayersResultPacket((ushort)onlineCharacters.Count));
            }
        }
    }
}