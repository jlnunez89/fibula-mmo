// -----------------------------------------------------------------
// <copyright file="ThingMovementEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile.EventRules
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an item movement event rule.
    /// </summary>
    public class ThingMovementEventRule : MoveUseEventRule, IThingMovementEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingMovementEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptFactory">A reference to the script factory in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public ThingMovementEventRule(ILogger logger, IScriptApi scriptFactory, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, scriptFactory, conditionSet, actionSet)
        {
        }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.Movement;
    }
}