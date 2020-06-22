// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperationCreationArguments.cs" company="2Dudes">
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
    using Fibula.Server.Operations.Contracts.Abstractions;

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
