// -----------------------------------------------------------------
// <copyright file="WalkPlanState.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible states of a walk plan.
    /// </summary>
    public enum WalkPlanState
    {
        /// <summary>
        /// The plan is to continue, and there is no need to recalculate the path.
        /// </summary>
        OnTrack,

        /// <summary>
        /// The plan is to continue, but the path should be recalculated.
        /// </summary>
        NeedsToRecalculate,

        /// <summary>
        /// The plan was successfully traversed and we're now at the goal location.
        /// </summary>
        AtGoal,

        /// <summary>
        /// The plan was aborted.
        /// </summary>
        Aborted,
    }
}
