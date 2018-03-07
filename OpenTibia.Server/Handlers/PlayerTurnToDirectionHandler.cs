using OpenTibia.Communications;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Handlers
{
    internal class PlayerTurnToDirectionHandler : IncomingPacketHandler
    {
        public Direction Direction { get; }

        public PlayerTurnToDirectionHandler(Direction direction)
        {
            Direction = direction;
        }

        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            // No other content in message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            player.TurnToDirection(Direction);

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureTurnedNotification(conn, player), player.Location);
        }
    }
}