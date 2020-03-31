// -----------------------------------------------------------------
// <copyright file="SpawnMonstersOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Environment;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="SpawnMonstersOperation"/>.
    /// </summary>
    public class SpawnMonstersOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnMonstersOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="spawn">The spawn information.</param>
        /// <param name="creationMetadata">The metadata for monster creation.</param>
        public SpawnMonstersOperationCreationArguments(Spawn spawn, ICreatureCreationMetadata creationMetadata)
        {
            this.Spawn = spawn;
            this.MonsterCreationMetadata = creationMetadata;
        }

        /// <summary>
        /// Gets the spawn information.
        /// </summary>
        public Spawn Spawn { get; }

        /// <summary>
        /// Gets the creation metadata for all monsters in this spawn.
        /// </summary>
        public ICreatureCreationMetadata MonsterCreationMetadata { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }
    }
}
