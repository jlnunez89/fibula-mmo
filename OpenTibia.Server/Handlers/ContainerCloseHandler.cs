using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class ContainerCloseHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var packet = new ContainerCloseRequestPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            player?.CloseContainerWithId(packet.ContainerId);
        }
    }
}