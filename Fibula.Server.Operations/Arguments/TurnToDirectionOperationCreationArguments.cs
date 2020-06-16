// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="TurnToDirectionOperation"/>.
    /// </summary>
    public class TurnToDirectionOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToDirectionOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="direction"></param>
        public TurnToDirectionOperationCreationArguments(ICreature creature, Direction direction)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = creature.Id;
            this.Creature = creature;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public ICreature Creature { get; }

        public Direction Direction { get; }
    }
}
