using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class AttackHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var attackPacket = new AttackPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            player?.SetAttackTarget(attackPacket.CreatureId);
        }
    }
}