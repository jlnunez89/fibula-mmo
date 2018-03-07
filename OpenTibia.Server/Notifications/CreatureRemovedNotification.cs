using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    internal class CreatureRemovedNotification : Notification
    {
        public EffectT RemoveEffect { get; }

        public byte OldStackPosition { get; }
                
        public ICreature Creature { get; }

        public CreatureRemovedNotification(Connection connection, ICreature creature, byte oldStackPos, EffectT removeEffect = EffectT.None) 
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
            OldStackPosition = oldStackPos;
            RemoveEffect = removeEffect;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId);

            if (player == null || !player.CanSee(Creature) || !player.CanSee(Creature.Location))
            {
                return;
            }

            ResponsePackets.Add(new RemoveAtStackposPacket
            {
                Location = Creature.Location,
                Stackpos = OldStackPosition
            });

            if (RemoveEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Location = Creature.Location,
                    Effect = EffectT.Puff
                });
            }
        }
    }
}