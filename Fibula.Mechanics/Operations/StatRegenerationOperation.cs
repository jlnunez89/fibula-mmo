// -----------------------------------------------------------------
// <copyright file="StatRegenerationOperation.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an operation to regenerate a particular stat over time.
    /// </summary>
    public class StatRegenerationOperation : Operation
    {
        private const int AmountToRegen = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatRegenerationOperation"/> class.
        /// </summary>
        /// <param name="creature">The creature for which the stat is being regenerated.</param>
        /// <param name="statId">The id of the stat to regenerate.</param>
        public StatRegenerationOperation(ICreature creature, CreatureStat statId)
            : base(creature?.Id ?? 0)
        {
            creature.ThrowIfNull(nameof(creature));

            this.CanBeCancelled = false;

            this.Creature = creature;
            this.StatId = statId;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the creature for which the stat is being regenerated.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the type of stat to regenerate.
        /// </summary>
        public CreatureStat StatId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            if (this.Creature.IsDead || !this.Creature.Stats.ContainsKey(this.StatId))
            {
                return;
            }

            this.Creature.Stats[this.StatId].Increase(AmountToRegen);

            context.Logger.Verbose($"Restored {AmountToRegen} {this.StatId} credit(s) on {this.Creature.Name}. [Id={this.Creature.Id}] [{this.Creature.Stats[this.StatId].Current}/{this.Creature.Stats[this.StatId].Maximum}]");
        }
    }
}
