// -----------------------------------------------------------------
// <copyright file="RestoreCombatCreditOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Combat.Enumerations;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="RestoreCombatCreditOperation"/>.
    /// </summary>
    public class RestoreCombatCreditOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreCombatCreditOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="combatant">The combatant to which the credits should be restored.</param>
        /// <param name="combatCreditType">The type of credit to restore.</param>
        public RestoreCombatCreditOperationCreationArguments(ICombatant combatant, CombatCreditType combatCreditType)
        {
            combatant.ThrowIfNull(nameof(combatant));

            this.Combatant = combatant;
            this.CreditType = combatCreditType;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.RestoreCombatCredit;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Combatant.Id;

        /// <summary>
        /// Gets the combatant to restore credits to.
        /// </summary>
        public ICombatant Combatant { get; }

        /// <summary>
        /// Gets the type of credit to restore.
        /// </summary>
        public CombatCreditType CreditType { get; }
    }
}
