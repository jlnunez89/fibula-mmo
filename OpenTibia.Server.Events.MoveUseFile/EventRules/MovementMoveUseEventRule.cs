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
    using System.Linq;
    using OpenTibia.Common.Utilities;
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
        /// <param name="gameApi">A reference to the game API.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public MovementMoveUseEventRule(ILogger logger, IGameApi gameApi, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, gameApi, conditionSet, actionSet)
        {
        }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.Movement;

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        public override bool CanBeExecuted(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (context.Arguments is MovementEventRuleArguments movementEventRuleArguments)
            {
                if (movementEventRuleArguments.ThingMoving == null)
                {
                    return false;
                }

                return this.Conditions.All(condition =>
                    this.InvokeCondition(
                        movementEventRuleArguments.ThingMoving,
                        null,
                        movementEventRuleArguments.Requestor as IPlayer,
                        condition.FunctionName,
                        condition.Parameters));
            }

            return false;
        }

        /// <summary>
        /// Executes this event rule.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        public override void Execute(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!(context.Arguments is MovementEventRuleArguments movementEventRuleArguments))
            {
                return;
            }

            IThing primaryThing = movementEventRuleArguments.ThingMoving;
            IThing secondaryThing = null;
            IPlayer user = movementEventRuleArguments.Requestor as IPlayer;

            foreach (var action in this.Actions)
            {
                this.InvokeAction(ref primaryThing, ref secondaryThing, ref user, action.FunctionName, action.Parameters);
            }

            // Re-map the arguments.
            movementEventRuleArguments.ThingMoving = primaryThing;
            movementEventRuleArguments.Requestor = user;

            context.Arguments = movementEventRuleArguments;

            if (this.RemainingExecutionCount != IEventRule.UnlimitedExecutions && this.RemainingExecutionCount > 0)
            {
                this.RemainingExecutionCount--;
            }
        }
    }
}