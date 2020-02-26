﻿// -----------------------------------------------------------------
// <copyright file="IEventRulesEvaluator.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for an event rules evaluator.
    /// </summary>
    public interface IEventRulesEvaluator
    {
        /// <summary>
        /// Evaluates any rules of the given type using the supplied arguments.
        /// </summary>
        /// <param name="caller">The evaluation requestor.</param>
        /// <param name="type">The type of rules to evaluate.</param>
        /// <param name="eventRuleArguments">The arguments to evaluate with.</param>
        /// <returns>True if at least one rule was matched and executed, false otherwise.</returns>
        bool EvaluateRules(object caller, EventRuleType type, IEventRuleArguments eventRuleArguments);
    }
}
