// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Environment;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="PlaceCreatureOperation"/>.
    /// </summary>
    public class PlaceCreatureOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceCreatureOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="atTile"></param>
        /// <param name="creature"></param>
        public PlaceCreatureOperationCreationArguments(uint requestorId, ITile atTile, ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = requestorId;
            this.AtTile = atTile;
            this.Creature = creature;
        }

        public ITile AtTile { get; }

        public ICreature Creature { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }
    }
}
