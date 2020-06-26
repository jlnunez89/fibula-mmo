// -----------------------------------------------------------------
// <copyright file="AutoAttackOrchestratorOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
