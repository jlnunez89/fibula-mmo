using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    internal class CreatureTurnedNotification : Notification
    {
        public ICreature Creature { get; }
        public EffectT TurnedEffect { get; }

        public CreatureTurnedNotification(Connection connection, ICreature creature, EffectT turnEffect = EffectT.None) 
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
            TurnedEffect = turnEffect;
        }

        public override void Prepare()
        {
            if (TurnedEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = TurnedEffect,
                    Location = Creature.Location
                });
            }

            ResponsePackets.Add(new CreatureTurnedPacket
            {
                Creature = Creature
            });
        }
    }
}