// -----------------------------------------------------------------
// <copyright file="StatChangeOperation.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an operation to regenerate a particular stat over time.
    /// </summary>
    public class StatChangeOperation : Operation
    {
        /// <summary>
        /// The default amount of points to change in the stat.
        /// </summary>
        private const int DefaultAmountToChange = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatChangeOperation"/> class.
        /// </summary>
        /// <param name="creature">The creature for which the stat is being changed.</param>
        /// <param name="statId">The id of the stat to change.</param>
        /// <param name="amount">Optional. The amount of points to change.</param>
        public StatChangeOperation(ICreature creature, CreatureStat statId, int amount = DefaultAmountToChange)
            : base(creature?.Id ?? 0)
        {
            creature.ThrowIfNull(nameof(creature));

            this.CanBeCancelled = false;

            this.Creature = creature;
            this.StatId = statId;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the creature for which the stat is being changed.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the type of stat to change.
        /// </summary>
        public CreatureStat StatId { get; }

        /// <summary>
        /// Gets the amount of points to change the stat by.
        /// </summary>
        public int Amount { get; }

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

            this.Creature.Stats[this.StatId].Increase(this.Amount);

            context.Logger.Verbose($"{this.Creature.Name}'s {this.StatId} changed: {this.Amount}. [{this.Creature.Stats[this.StatId].Current}/{this.Creature.Stats[this.StatId].Maximum}]");
        }
    }
}
