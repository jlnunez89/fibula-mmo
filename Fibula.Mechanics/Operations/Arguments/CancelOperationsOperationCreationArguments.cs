// -----------------------------------------------------------------
// <copyright file="CancelOperationsOperationCreationArguments.cs" company="2Dudes">
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
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Operations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="CancelOperationsOperation"/>.
    /// </summary>
    public class CancelOperationsOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationsOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature to cancel operations for.</param>
        /// <param name="typeOfActionToCancel">Optional. The specific type of operation to cancel.</param>
        public CancelOperationsOperationCreationArguments(ICreature creature, Type typeOfActionToCancel = null)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.TypeToCancel = typeOfActionToCancel;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.CancelOperations;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Creature.Id;

        /// <summary>
        /// Gets the creature cancelling actions for.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the type of actions to cancel.
        /// </summary>
        public Type TypeToCancel { get; }
    }
}
