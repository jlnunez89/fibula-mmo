// <copyright file="OutfitChangedHandler.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    public class OutfitChangedHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutfitChangedHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public OutfitChangedHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.OutfitChangeCompleted;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var outfitInfo = message.ReadOutfitInfo();

            if (!(this.Game.GetCreatureWithId(connection.PlayerId) is IPlayer player))
            {
                return;
            }

            // TODO: check if player actually has permissions to change outfit.
            player.SetOutfit(outfitInfo.Outfit);

            this.Game.NotifySpectatingPlayers(conn => new CreatureChangedOutfitNotification(conn, player), player.Location);
        }
    }
}