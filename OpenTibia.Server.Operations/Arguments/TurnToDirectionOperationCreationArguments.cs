// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Actions;

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
