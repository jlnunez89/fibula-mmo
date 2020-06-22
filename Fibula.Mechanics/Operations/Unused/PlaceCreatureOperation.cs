// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Environment
{
    /// <summary>
    /// Class that represents an operation for placing a creature on the map.
    /// </summary>
    public class PlaceCreatureOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceCreatureOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the placement.</param>
        /// <param name="atTile">The tile at which to place the creature.</param>
        /// <param name="creature">The creature being placed.</param>
        public PlaceCreatureOperation(uint requestorId, ITile atTile, ICreature creature)
            : base(requestorId)
        {
            this.AtTile = atTile;
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the creature to place.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the tile at which to place the creature.
        /// </summary>
        public ITile AtTile { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            bool successfulPlacement = this.PlaceCreature(context, this.AtTile, this.Creature);

            if (!successfulPlacement)
            {
                // handles check for isPlayer.
                // this.NotifyOfFailure();
                return;
            }
        }
    }
}
