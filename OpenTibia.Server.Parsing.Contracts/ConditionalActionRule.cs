// -----------------------------------------------------------------
// <copyright file="ConditionalActionRule.cs" company="2Dudes">
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

    public class ConditionalActionRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalActionRule"/> class.
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="actions"></param>
        public ConditionalActionRule(IEnumerable<string> conditions, IEnumerable<string> actions)
        {
            this.ConditionSet = conditions.ToList();
            this.ActionSet = actions.ToList();
        }

        public IList<string> ConditionSet { get; }

        public IList<string> ActionSet { get; }
    }
}
