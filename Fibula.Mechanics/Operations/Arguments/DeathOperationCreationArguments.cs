// -----------------------------------------------------------------
// <copyright file="DeathOperationCreationArguments.cs" company="2Dudes">
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
    using Fibula.Mechanics.Operations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="DeathOperation"/>.
    /// </summary>
    public class DeathOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeathOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="creature">The creature that died.</param>
        public DeathOperationCreationArguments(uint requestorId, ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = requestorId;
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.Death;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the the creature that died.
        /// </summary>
        public ICreature Creature { get; }
    }
}
