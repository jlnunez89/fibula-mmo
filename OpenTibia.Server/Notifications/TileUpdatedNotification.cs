using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Notifications
{
    internal class TileUpdatedNotification : Notification
    {
        public Location Location { get; }
        public byte[] Description { get; }

        public TileUpdatedNotification(Connection connection, Location location, byte[] description)
            : base(connection)
        {
            Location = location;
            Description = description;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new UpdateTilePacket
            {
                Location = Location,
                DescriptionBytes = Description
            });
        }
    }
}