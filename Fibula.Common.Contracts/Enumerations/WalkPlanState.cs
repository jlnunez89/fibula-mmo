// -----------------------------------------------------------------
// <copyright file="WalkPlanState.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        /// The plan is in progress and we should continue.
        /// </summary>
        InProgress,

        /// <summary>
        /// The plan was successfully traversed and we're now
        /// at the goal location.
        /// </summary>
        AtGoal,

        /// <summary>
        /// The plan was aborted.
        /// </summary>
        Aborted,
    }
}
