using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class AutoMoveCancelHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            // No content.
            player?.StopWalking();
            player?.ClearPendingActions();

            if (player != null)
            {
                ResponsePackets.Add(new PlayerWalkCancelPacket
                {
                    Direction = player.ClientSafeDirection
                });
            }
        }
    }
}