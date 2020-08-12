// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

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
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            bool successfulPlacement = this.PlaceCreature(context, this.AtTile, this.Creature);

            if (!successfulPlacement)
            {
                context.Logger.Warning($"Failed to place creature {this.Creature.Name} at {this.AtTile.Location}");

                return;
            }
        }
    }
}
