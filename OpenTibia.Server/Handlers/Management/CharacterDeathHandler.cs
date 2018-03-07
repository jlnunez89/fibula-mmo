using System.Collections.Generic;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data;
using OpenTibia.Data.Models;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class CharacterDeathHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var characterDeathPacket = new CharacterDeathPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerKilledPlayer = characterDeathPacket.KillerId > 0;

                otContext.Deaths.Add(new Death
                {
                    PlayerId = characterDeathPacket.VictimId,
                    Level = characterDeathPacket.VictimLevel,
                    ByPeekay = (byte)(playerKilledPlayer ? 1 : 0),
                    PeekayId = playerKilledPlayer ? characterDeathPacket.KillerId : 0,
                    CreatureString = characterDeathPacket.KillerName,
                    Unjust = characterDeathPacket.Unjustified,
                    Timestamp = characterDeathPacket.Timestamp
                });
                
                otContext.SaveChanges();

                ResponsePackets.Add(new DefaultNoErrorPacket());
            }
        }
    }
}