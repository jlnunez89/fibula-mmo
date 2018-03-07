using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Notifications
{
    internal class AnimatedTextNotification : Notification
    {
        public Location Location { get; }
                
        public TextColor TextColor { get; }

        public string Text { get; }

        public AnimatedTextNotification(Connection connection, Location location, string text, TextColor textColor = TextColor.White)  
            : base(connection)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Location = location;
            Text = text;
            TextColor = textColor;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new AnimatedTextPacket
            {
                Location = Location,
                Text = Text,
                Color = TextColor
            });
        }
    }
}