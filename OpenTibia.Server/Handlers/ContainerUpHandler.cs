using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class ContainerUpHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var packet = new ContainerUpPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            var currentContainer = player?.GetContainer(packet.ContainerId);

            if (currentContainer?.Parent == null)
            {
                return;
            }

            player?.OpenContainerAt(currentContainer.Parent, packet.ContainerId);

            ResponsePackets.Add(new ContainerOpenPacket
            {
                ContainerId = (byte)currentContainer.Parent.GetIdFor(connection.PlayerId),
                ClientItemId = currentContainer.Parent.ThingId,
                HasParent = currentContainer.Parent.Parent != null,
                Name = currentContainer.Parent.Type.Name,
                Volume = currentContainer.Parent.Volume,
                Contents = currentContainer.Parent.Content
            });
        }
    }
}