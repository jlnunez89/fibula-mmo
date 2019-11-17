// <copyright file="CharacterDeathHandler.cs" company="2Dudes">
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
    using OpenTibia.Communications.Packets.Outgoing;

    public class CharacterDeathHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.CharacterDeath;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var characterDeathInfo = message.ReadCharacterDeathInfo();

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerKilledPlayer = characterDeathInfo.KillerId > 0;

                otContext.Deaths.Add(new Death
                {
                    PlayerId = characterDeathInfo.VictimId,
                    Level = characterDeathInfo.VictimLevel,
                    ByPeekay = (byte)(playerKilledPlayer ? 1 : 0),
                    PeekayId = playerKilledPlayer ? characterDeathInfo.KillerId : 0,
                    CreatureString = characterDeathInfo.KillerName,
                    Unjust = (byte)(characterDeathInfo.Unjustified ? 0x01 : 0x00),
                    Timestamp = characterDeathInfo.Timestamp.ToUnixTimeSeconds(),
                });

                otContext.SaveChanges();

                this.ResponsePackets.Add(new DefaultNoErrorPacket());
            }
        }
    }
}