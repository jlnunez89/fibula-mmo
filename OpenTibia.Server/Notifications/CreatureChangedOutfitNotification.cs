using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    internal class CreatureChangedOutfitNotification : Notification
    {
        public ICreature Creature { get; }

        public CreatureChangedOutfitNotification(Connection connection, ICreature creature)  
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new CreatureChangedOutfitPacket
            {
                Creature = Creature
            });
        }
    }
}