// -----------------------------------------------------------------
// <copyright file="IOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for a game operation.
    /// </summary>
    public interface IOperation : IEvent
    {
        /// <summary>
        /// Event delegate that is called when a rule event evaluation is requested.
        /// </summary>
        event EventRulesEvaluationTriggered EventRulesEvaluationTriggered;

        /// <summary>
        /// Gets the reference to this operation's context.
        /// </summary>
        IOperationContext Context { get; }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets the exhaustion cost of this operation.
        /// </summary>
        TimeSpan ExhaustionCost { get; }
    }
}
