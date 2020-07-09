// -----------------------------------------------------------------
// <copyright file="AutoWalkOrchestratorOperationCreationArguments.cs" company="2Dudes">
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
