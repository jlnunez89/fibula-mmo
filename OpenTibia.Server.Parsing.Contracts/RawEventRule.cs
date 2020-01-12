// -----------------------------------------------------------------
// <copyright file="RawEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class that represents a raw rule.
    /// </summary>
    public class RawEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawEventRule"/> class.
        /// </summary>
        /// <param name="conditions">The conditions of this rule.</param>
        /// <param name="actions">The actions of this rule.</param>
        public RawEventRule(IEnumerable<string> conditions, IEnumerable<string> actions)
        {
            this.ConditionSet = conditions.ToList();
            this.ActionSet = actions.ToList();

            this.Type = this.ConditionSet.FirstOrDefault();

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
