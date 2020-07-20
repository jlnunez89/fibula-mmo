// -----------------------------------------------------------------
// <copyright file="RestoreCombatCreditOperation.cs" company="2Dudes">
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
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Combat.Enumerations;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an event to restore combat credits.
    /// </summary>
    public class RestoreCombatCreditOperation : Operation
    {
        private const int AmountToRestore = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreCombatCreditOperation"/> class.
        /// </summary>
        /// <param name="combatant">The combatant that is being restored.</param>
        /// <param name="creditType">The type of combat credit to restore.</param>
        public RestoreCombatCreditOperation(ICombatant combatant, CombatCreditType creditType)
            : base(combatant?.Id ?? 0)
        {
            combatant.ThrowIfNull(nameof(combatant));

            this.CanBeCancelled = false;

            this.Combatant = combatant;
            this.CreditType = creditType;
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
        /// Gets the combatant that the credit will be restored for.
        /// </summary>
        public ICombatant Combatant { get; }

        /// <summary>
        /// Gets the type of credit to restore.
        /// </summary>
        public CombatCreditType CreditType { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            if (this.Combatant.IsDead)
            {
                return;
            }

            this.Combatant.RestoreCredits(this.CreditType, AmountToRestore);

            var current = this.CreditType == CombatCreditType.Attack ? this.Combatant.AutoAttackCredits : this.Combatant.AutoDefenseCredits;
            var max = this.CreditType == CombatCreditType.Attack ? this.Combatant.AutoAttackMaximumCredits : this.Combatant.AutoDefenseMaximumCredits;

            context.Logger.Verbose($"Restored {AmountToRestore} {this.CreditType} credit(s) on {this.Combatant.Name}. [Id={this.Combatant.Id}] [{current}/{max}]");
        }
    }
}
