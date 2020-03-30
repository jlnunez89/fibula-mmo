// -----------------------------------------------------------------
// <copyright file="RestoreCombatCreditOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Combat
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

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
        public override TimeSpan ExhaustionCost
        {
            get => TimeSpan.Zero;

            protected set => throw new NotImplementedException();
        }

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
        protected override void Execute(IOperationContext context) => this.Combatant.RestoreCredits(this.CreditType, AmountToRestore);
    }
}