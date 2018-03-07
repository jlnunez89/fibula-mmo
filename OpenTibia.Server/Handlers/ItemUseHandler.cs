using System.Linq;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Actions;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Handlers
{
    internal class ItemUseHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var itemUsePacket = new ItemUsePacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            player.ClearPendingActions();

            // Before actually using the item, check if we're close enough to use it.
            if (itemUsePacket.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemUsePacket.FromLocation - player.Location;

                if (locationDiff.Z != 0) // it's on a different floor...
                {
                    ResponsePackets.Add(new TextMessagePacket
                    {
                        Type = MessageType.StatusSmall,
                        Message = "There is no way."
                    });

                    return;
                }

                if (locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away to use it. 
                    Location retryLoc;
                    var directions = Game.Instance.Pathfind(player.Location, itemUsePacket.FromLocation, out retryLoc).ToArray();

                    player.SetPendingAction(new UseItemAction(player, itemUsePacket, retryLoc));

                    if (directions.Any())
                    {
                        player.AutoWalk(directions);
                    }
                    else // we found no way...
                    {
                        ResponsePackets.Add(new TextMessagePacket
                        {
                            Type = MessageType.StatusSmall,
                            Message = "There is no way."
                        });
                    }

                    return;
                }
            }

            new UseItemAction(player, itemUsePacket, itemUsePacket.FromLocation).Perform();
        }
    }
}