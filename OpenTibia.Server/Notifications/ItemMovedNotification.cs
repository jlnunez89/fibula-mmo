using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Notifications
{
    internal class ItemMovedNotification : Notification
    {
        public bool WasTeleport { get; }

        public byte FromStackpos { get; }
        public byte ToStackpos { get; }

        public Location FromLocation { get; }
        public Location ToLocation { get; }

        public IItem Item { get; }

        public ItemMovedNotification(Connection connection, IItem item, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport) : base(connection)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Item = item;
            FromLocation = fromLocation;
            FromStackpos = fromStackPos;
            ToLocation = toLocation;
            ToStackpos = toStackPos;
            WasTeleport = wasTeleport;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId);
            
            if (player.CanSee(FromLocation) && FromStackpos < 10)
            {
                ResponsePackets.Add(new RemoveAtStackposPacket
                {
                    Location = FromLocation,
                    Stackpos = FromStackpos
                });
            }

            if (player.CanSee(ToLocation))
            {
                ResponsePackets.Add(new AddItemPacket
                {
                    Location = ToLocation,
                    Item = Item
                });
            }
        }
    }
}