using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class OutfitChangeRequestHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            // No further content on message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: if player actually has permissions to change outfit.

            // TODO: get these based on sex and premium

            ushort chooseFromId = 128;
            ushort chooseToId = 134;

            ResponsePackets.Add(new PlayerChooseOutfitPacket
            {
                CurrentOutfit = player.Outfit,
                ChooseFromId = chooseFromId,
                ChooseToId = chooseToId
            });
        }
    }
}