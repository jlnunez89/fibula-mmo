using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class AutoMovePacket : PacketIncoming
    {
        public Direction[] Directions { get; private set; }

        public AutoMovePacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            try
            {
                var movesCount = message.GetByte();

                Directions = new Direction[movesCount];

                for (var i = 0; i < movesCount; i++)
                {
                    Direction direction;
                    var dir = message.GetByte();

                    switch (dir)
                    {
                        case 1:
                            direction = Direction.East;
                            break;
                        case 2:
                            direction = Direction.NorthEast;
                            break;
                        case 3:
                            direction = Direction.North;
                            break;
                        case 4:
                            direction = Direction.NorthWest;
                            break;
                        case 5:
                            direction = Direction.West;
                            break;
                        case 6:
                            direction = Direction.SouthWest;
                            break;
                        case 7:
                            direction = Direction.South;
                            break;
                        case 8:
                            direction = Direction.SouthEast;
                            break;
                        default: continue;
                    }

                    Directions[i] = direction;
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
