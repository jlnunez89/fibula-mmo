using System;
using OpenTibia.Communications;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class PlayerSetModeHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            // No other content in message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            var rawFightMode = message.GetByte(); //1 - offensive, 2 - balanced, 3 - defensive
            var rawChaseMode = message.GetByte(); // 0 - stand while fightning, 1 - chase opponent
            var rawSafeMode = message.GetByte();

            var fightMode = (FightMode)rawFightMode;

            // TODO: correctly implement.

            Console.WriteLine($"PlayerId {player.Name} changed modes to {fightMode}.");
        }
    }
}