// -----------------------------------------------------------------
// <copyright file="EventRulesEvaluationTriggered.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    using Fibula.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for when an event rules evaluation event is triggered.
    /// </summary>
    /// <param name="byEvent">The event that triggered this evaluation.</param>
    /// <param name="ruleType">The type of rules that require evaluation.</param>
    /// <param name="ruleArguments">The arguments with which to perform the evaluation.</param>
    public delegate bool EventRulesEvaluationTriggered(IEvent byEvent, EventRuleType ruleType, IEventRuleArguments ruleArguments);
}
