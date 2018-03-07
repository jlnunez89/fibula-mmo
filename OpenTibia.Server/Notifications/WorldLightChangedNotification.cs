using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;

namespace OpenTibia.Server.Notifications
{
    internal class WorldLightChangedNotification : Notification
    {
        public byte LightLevel { get; }
                
        public byte LightColor { get; }

        public WorldLightChangedNotification(Connection connection, byte lightLevel, byte lightColor = (byte)OpenTibia.Data.Contracts.LightColors.White)  
            : base(connection)
        {
            LightLevel = lightLevel;
            LightColor = lightColor;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new WorldLightPacket
            {
                Level = LightLevel,
                Color = LightColor
            });
        }
    }
}