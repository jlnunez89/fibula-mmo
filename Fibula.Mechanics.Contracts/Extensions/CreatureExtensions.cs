// -----------------------------------------------------------------
// <copyright file="CreatureExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
            if (creature == null)
            {
                return TimeSpan.Zero;
            }

            // TODO: incorporate last step (i.e. if diagonal) into this calculation.
            var tilePenalty = fromTile?.Ground?.MovementPenalty ?? MechanicsConstants.DefaultGroundMovementPenaltyInMs;

            const uint Epsilon = 25;
            decimal totalPenalty = 1000 * tilePenalty;
            decimal stepSpeed = Math.Max(1u, creature.Speed);

            var durationInMs = (uint)Math.Ceiling(totalPenalty / stepSpeed / Epsilon) * Epsilon;
            var adjustedDurationInMs = durationInMs * (stepDirection.IsDiagonal() ? 2 : 1);

            return TimeSpan.FromMilliseconds(adjustedDurationInMs);
        }
    }
}
