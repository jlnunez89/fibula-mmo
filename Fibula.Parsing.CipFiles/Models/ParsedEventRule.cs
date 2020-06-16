// -----------------------------------------------------------------
// <copyright file="ParsedEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Utilities;

    /// <summary>
    /// Class that represents a parsed event rule.
    /// </summary>
    public class ParsedEventRule : IEventRuleCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedEventRule"/> class.
        /// </summary>
        /// <param name="conditions">The conditions of this rule.</param>
        /// <param name="actions">The actions of this rule.</param>
        public ParsedEventRule(IEnumerable<string> conditions, IEnumerable<string> actions)
        {
            conditions.ThrowIfNull(nameof(conditions));

            this.ConditionSet = conditions.ToList();
            this.ActionSet = actions.ToList();

            this.Type = this.ConditionSet.FirstOrDefault();

            if (this.ConditionSet.Count == 0)
            {
                throw new ArgumentException($"Event rule must contain at least one condition.");
            }

            // Remove first element since we already parsed it into 'Type'.
            this.ConditionSet.RemoveAt(0);
        }

        /// <summary>
        /// Gets the type of event.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the set of conditions for this rule.
        /// </summary>
        public IList<string> ConditionSet { get; }

        /// <summary>
        /// Gets the set of actions for this rule.
        /// </summary>
        public IList<string> ActionSet { get; }
    }
}
