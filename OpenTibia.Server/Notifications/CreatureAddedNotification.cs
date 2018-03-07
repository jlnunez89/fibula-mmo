using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    internal class CreatureAddedNotification : Notification
    {
        public ICreature Creature { get; }
        public EffectT AddedEffect { get; }

        public CreatureAddedNotification(Connection connection, ICreature creature, EffectT addEffect = EffectT.None) 
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
            AddedEffect = addEffect;
        }

        public override void Prepare()
        {
            if(Creature.CreatureId == Connection.PlayerId)
            {
                return;
            }

            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId) as IPlayer;

            if (player == null)
            {
                return;
            }

            if (AddedEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = AddedEffect,
                    Location = Creature.Location
                });
            }

            ResponsePackets.Add(new AddCreaturePacket
            {
                Creature = Creature,
                Location = Creature.Location,
                AsKnown = player.KnowsCreatureWithId(Creature.CreatureId),
                RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet()
            });
        }
    }
}