using System;
using OpenTibia.Communications;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class ItemUseBattleHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;
            player?.ClearPendingActions();



        }
    }
}