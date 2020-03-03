// -----------------------------------------------------------------
// <copyright file="MovementMoveUseEventRule.cs" company="2Dudes">
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
    public class MovementMoveUseEventRule : MoveUseEventRule, IThingMovementEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementMoveUseEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public MovementMoveUseEventRule(ILogger logger, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, conditionSet, actionSet)
        {
        }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.Movement;
    }
}