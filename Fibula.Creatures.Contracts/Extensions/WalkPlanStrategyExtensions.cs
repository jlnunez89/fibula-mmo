// -----------------------------------------------------------------
// <copyright file="WalkPlanStrategyExtensions.cs" company="2Dudes">
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
    /// Helper class for extension methods of walk plan strategies.
    /// </summary>
    public static class WalkPlanStrategyExtensions
    {
        /// <summary>
        /// Checks if a given walk strategy is considered static.
        /// </summary>
        /// <param name="strategy">The strategy to evaluate.</param>
        /// <returns>True if the strategy is considered static, false otherwise.</returns>
        public static bool IsStatic(this WalkPlanStrategy strategy)
        {
            return strategy == WalkPlanStrategy.DoNotRecalculate || strategy == WalkPlanStrategy.RecalculateOnInterruption;
        }
    }
}
