// -----------------------------------------------------------------
// <copyright file="CreatureExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Extensions
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Extensions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Constants;

    /// <summary>
    /// Static class that provides helper methods for creature mechanics.
    /// </summary>
    public static class CreatureExtensions
    {
        /// <summary>
        /// Calculates the step duration of a creature moving from a given tile in the given direction.
        /// </summary>
        /// <param name="creature">The creature that's moving.</param>
        /// <param name="stepDirection">The direction of the step.</param>
        /// <param name="fromTile">The tile which the creature is moving from.</param>
        /// <returns>The duration time of the step.</returns>
        public static TimeSpan CalculateStepDuration(this ICreature creature, Direction stepDirection, ITile fromTile = null)
        {
            const uint Epsilon = 50;

            if (creature == null)
            {
                return TimeSpan.Zero;
            }

            // TODO: incorporate last step (i.e. if diagonal) into this calculation.
            var tilePenalty = fromTile?.Ground?.MovementPenalty ?? MechanicsConstants.DefaultGroundMovementPenaltyInMs;

            decimal totalPenalty = 1000 * tilePenalty;
            decimal stepSpeed = Math.Max(1u, creature.Speed);

            var durationInMs = (uint)Math.Ceiling(Math.Floor(totalPenalty / stepSpeed) / Epsilon) * Epsilon;

            return TimeSpan.FromMilliseconds(durationInMs);
        }
    }
}
