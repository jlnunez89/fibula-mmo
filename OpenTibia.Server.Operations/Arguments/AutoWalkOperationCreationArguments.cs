// -----------------------------------------------------------------
// <copyright file="AutoWalkOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Movements;

    /// <summary>
    /// Class that represents creation arguments for an <see cref="AutoWalkOperation"/>.
    /// </summary>
    public class AutoWalkOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWalkOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="creature"></param>
        /// <param name="directions"></param>
        public AutoWalkOperationCreationArguments(uint requestorId, ICreature creature, params Direction[] directions)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = requestorId;
            this.Creature = creature;
            this.Directions = directions;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public ICreature Creature { get; }

        public Direction[] Directions { get; }
    }
}
