using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class LogoutHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            // no further content
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            if (Game.Instance.AttemptLogout(player))
            {
                connection.Close();
            }
            else
            {
                ResponsePackets.Add(new TextMessagePacket
                {
                    Type = MessageType.StatusSmall,
                    Message = "You may not logout (test message)"
                });
            }
        }
    }
}