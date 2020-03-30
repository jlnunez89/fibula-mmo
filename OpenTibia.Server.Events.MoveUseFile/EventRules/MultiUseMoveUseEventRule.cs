﻿// -----------------------------------------------------------------
// <copyright file="MultiUseMoveUseEventRule.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an event rule for using an item on something else.
    /// </summary>
    internal class MultiUseMoveUseEventRule : MoveUseEventRule, IUseItemOnEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiUseMoveUseEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameApi">A reference to the game API.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public MultiUseMoveUseEventRule(ILogger logger, IGameApi gameApi, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, gameApi, conditionSet, actionSet)
        {
            // Look for a IsType condition.
            var isTypeConditions = this.Conditions.Where(func => IsTypeFunctionName.Equals(func.FunctionName));

            var typeConditionsList = isTypeConditions as IList<IEventRuleFunction> ?? isTypeConditions.ToList();
            var firstTypeCondition = typeConditionsList.FirstOrDefault();
            var secondTypeCondition = typeConditionsList.Skip(1).FirstOrDefault();

            if (firstTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find first {IsTypeFunctionName} function.");
            }

            if (secondTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find second {IsTypeFunctionName} function.");
            }

            this.ItemToUseId = Convert.ToUInt16(firstTypeCondition.Parameters[1]);
            this.ThingToUseOnId = Convert.ToUInt16(secondTypeCondition.Parameters[1]);
        }

        /// <summary>
        /// Gets the id of the item to use.
        /// </summary>
        public ushort ItemToUseId { get; }

        /// <summary>
        /// Gets the id of the thing to use on.
        /// </summary>
        public ushort ThingToUseOnId { get; }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.MultiUse;

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        public override bool CanBeExecuted(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (context.Arguments is MultiUseEventRuleArguments multiUseEventRuleArguments)
            {
                if (multiUseEventRuleArguments.ItemUsed == null ||
                    multiUseEventRuleArguments.ItemUsed.ThingId != this.ItemToUseId ||
                    multiUseEventRuleArguments.UseOnThing == null ||
                    multiUseEventRuleArguments.UseOnThing.ThingId != this.ThingToUseOnId)
                {
                    return false;
                }

                return this.Conditions.All(condition =>
                    this.InvokeCondition(
                        multiUseEventRuleArguments.ItemUsed,
                        multiUseEventRuleArguments.UseOnThing,
                        multiUseEventRuleArguments.Requestor as IPlayer,
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

            if (!(context.Arguments is MultiUseEventRuleArguments multiUseEventRuleArguments))
            {
                return;
            }

            IThing primaryThing = multiUseEventRuleArguments.ItemUsed;
            IThing secondaryThing = multiUseEventRuleArguments.UseOnThing;
            IPlayer user = multiUseEventRuleArguments.Requestor as IPlayer;

            foreach (var action in this.Actions)
            {
                this.InvokeAction(ref primaryThing, ref secondaryThing, ref user, action.FunctionName, action.Parameters);
            }

            // Re-map the arguments.
            multiUseEventRuleArguments.ItemUsed = primaryThing as IItem;
            multiUseEventRuleArguments.UseOnThing = secondaryThing;
            multiUseEventRuleArguments.Requestor = user;

            context.Arguments = multiUseEventRuleArguments;

            if (this.RemainingExecutionCount != IEventRule.UnlimitedExecutions && this.RemainingExecutionCount > 0)
            {
                this.RemainingExecutionCount--;
            }
        }
    }
}