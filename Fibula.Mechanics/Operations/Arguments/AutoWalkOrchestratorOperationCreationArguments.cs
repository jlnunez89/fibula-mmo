// -----------------------------------------------------------------
// <copyright file="AutoWalkOrchestratorOperationCreationArguments.cs" company="2Dudes">
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
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="AutoWalkOrchestratorOperation"/>.
    /// </summary>
    public class AutoWalkOrchestratorOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWalkOrchestratorOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="creature">The combatant to trigger auto attacks for.</param>
        public AutoWalkOrchestratorOperationCreationArguments(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.AutoWalkOrchestrator;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Creature.Id;

        /// <summary>
        /// Gets the creature that's auto walking.
        /// </summary>
        public ICreature Creature { get; }
    }
}
