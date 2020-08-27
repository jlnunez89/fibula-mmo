// -----------------------------------------------------------------
// <copyright file="DeathOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents an operation for a creature's death.
    /// </summary>
    public class DeathOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeathOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="creature">The creature that died.</param>
        public DeathOperation(uint requestorId, ICreature creature)
            : base(requestorId)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the creature that died.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            if (this.Creature is IPlayer player)
            {
                this.SendNotification(context, new TextMessageNotification(() => player.YieldSingleItem(), MessageType.EventAdvance, "You are dead."));

                this.SendNotification(context, new GenericNotification(() => player.YieldSingleItem(), new PlayerCancelWalkPacket(player.Direction), new PlayerDeathPacket()));
            }

            // Give out the experience if this is a monster
            if (this.Creature is IMonster monster && monster.ExperienceToYield > 0 && this.Creature is ICombatant monsterCombatant)
            {
                ulong totalDamageDealt = (ulong)monsterCombatant.DamageTakenInSession.Sum(t => t.Damage);

                foreach (var (combatantId, damage) in monsterCombatant.DamageTakenInSession)
                {
                    if (damage == 0)
                    {
                        continue;
                    }

                    var expPercentage = Convert.ToDecimal(damage) / totalDamageDealt;
                    var expToGive = (long)Math.Round(monster.ExperienceToYield * expPercentage, 0, MidpointRounding.ToEven);

                    if (expToGive == 0)
                    {
                        continue;
                    }

                    if (context.CreatureFinder.FindCreatureById(combatantId) is ICombatant combatantGainingExp)
                    {
                        combatantGainingExp.AddExperience(expToGive);
                    }
                }
            }

            // Remove the creature...
            if (context.Map.GetTileAt(this.Creature.Location) is ITile creatureTile)
            {
                // Add the corpse.
                var corpseCreationArguments = new ItemCreationArguments()
                {
                    TypeId = this.Creature.CorpseTypeId,
                };

                if (context.ItemFactory.Create(corpseCreationArguments) is IThing corpseCreated && this.AddContentToContainerOrFallback(context, creatureTile, ref corpseCreated))
                {
                    context.GameApi.CreateItemAtLocation(creatureTile.Location, context.PredefinedItemSet.FindPoolForBloodType(this.Creature.BloodType));
                }

                this.RemoveCreature(context, this.Creature);
            }
        }
    }
}
