// -----------------------------------------------------------------
// <copyright file="IEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for event rules.
    /// </summary>
    public interface IEventRule
    {
        /// <summary>
        /// The value to use for unlimited number of executions.
        /// </summary>
        public const int UnlimitedExecutions = -1;

        /// <summary>
        /// Gets the identifier of this rule.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the type of this rule.
        /// </summary>
        EventRuleType Type { get; }

        /// <summary>
        /// Gets the remaining number of executions for this rule.
        /// </summary>
        int RemainingExecutionCount { get; }

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        bool CanBeExecuted(IEventRuleContext context);

        /// <summary>
        /// Executes this event rule.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        void Execute(IEventRuleContext context);
    }
}