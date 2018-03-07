using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    internal class CreatureSpokeNotification : Notification
    {
        public ICreature Creature { get; }
        public SpeechType SpeechType { get; }
        public string Message { get; }
        public ChatChannel Channel { get; }

        public CreatureSpokeNotification(Connection connection, ICreature creature, SpeechType speechType, string message, ChatChannel channel = ChatChannel.None) 
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Creature = creature;
            SpeechType = speechType;
            Message = message;
            Channel = channel;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new CreatureSpeechPacket
            {
                ChannelId = Channel,
                SenderName = Creature.Name,
                Location = Creature.Location,
                SpeechType = SpeechType,
                Text = Message,
                Time = (uint)DateTime.Now.Ticks
            });
        }
    }
}