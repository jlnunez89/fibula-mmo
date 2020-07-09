// -----------------------------------------------------------------
// <copyright file="AutoAttackOrchestratorOperationCreationArguments.cs" company="2Dudes">
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
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="AutoAttackOrchestratorOperation"/>.
    /// </summary>
    public class AutoAttackOrchestratorOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackOrchestratorOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="combatant">The combatant to trigger auto attacks for.</param>
        public AutoAttackOrchestratorOperationCreationArguments(ICombatant combatant)
        {
            combatant.ThrowIfNull(nameof(combatant));

            this.Combatant = combatant;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.AutoAttackOrchestrator;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Combatant.Id;

        /// <summary>
        /// Gets the creature that's auto attacking.
        /// </summary>
        public ICombatant Combatant { get; }
    }
}
