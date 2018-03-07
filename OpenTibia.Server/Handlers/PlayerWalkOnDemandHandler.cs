using System;
using System.Threading.Tasks;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class PlayerWalkOnDemandHandler : IncomingPacketHandler
    {
        public Direction Direction { get; }

        public PlayerWalkOnDemandHandler(Direction direction)
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

            player.ClearPendingActions();
            var cooldownRemaining = player.CalculateRemainingCooldownTime(CooldownType.Move, Game.Instance.MovementSynchronizationTime);

            if (cooldownRemaining == TimeSpan.Zero)
            {
                if (Game.Instance.RequestCreatureWalkToDirection(player, Direction))
                {
                    return;
                }

                ResponsePackets.Add(new PlayerWalkCancelPacket
                {
                    Direction = player.Direction
                });

                ResponsePackets.Add(new TextMessagePacket
                {
                    Message = "Sorry, not possible.",
                    Type = MessageType.StatusSmall
                });
            }
            else
            {
                // schedule de walk.
                Task.Delay(cooldownRemaining)
                    .ContinueWith(previous =>
                    {
                        player.AutoWalk(Direction);
                    });
            }
        }
    }
}