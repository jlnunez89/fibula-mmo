using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Handlers
{
    internal class OutfitChangedHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var packet = new OutfitChangedPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: if player actually has permissions to change outfit.

            player.SetOutfit(packet.Outfit);

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureChangedOutfitNotification(conn, player), player.Location);
        }
    }
}