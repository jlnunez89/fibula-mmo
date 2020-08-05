// -----------------------------------------------------------------
// <copyright file="WalkPlanStateExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Extensions
{
    using Fibula.Creatures.Contracts.Enumerations;

    /// <summary>
    /// Helper class for extension methods of walk plan states.
    /// </summary>
    public static class WalkPlanStateExtensions
    {
        /// <summary>
        /// Checks if a given walk plan state implies recalculation is needed.
        /// </summary>
        /// <param name="state">The state to evaluate.</param>
        /// <returns>True if the state implies recalculation is needed, false otherwise.</returns>
        public static bool ImpliesRecalculationNeeded(this WalkPlanState state)
        {
            return state == WalkPlanState.Blocked || state == WalkPlanState.NeedsToRecalculate;
        }
    }
}
