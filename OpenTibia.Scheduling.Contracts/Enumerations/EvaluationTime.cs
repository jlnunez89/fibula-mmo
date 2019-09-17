// -----------------------------------------------------------------
// <copyright file="EvaluationTime.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Contracts.Enumerations
{
    using OpenTibia.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Represents options for when to evaluate the conditionals defined on an <see cref="IEvent"/>.
    /// </summary>
    public enum EvaluationTime
    {
        /// <summary>
        /// Evaluation occurs at schedule time.
        /// </summary>
        OnSchedule,

        /// <summary>
        /// Evaluation occurs right before execution time.
        /// </summary>
        OnExecute,

        /// <summary>
        /// Evaluation occurs both while being scheduled and right before execution time.
        /// </summary>
        OnBoth,
    }
}