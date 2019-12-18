// -----------------------------------------------------------------
// <copyright file="UseItemOnEventRule.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an event rule for using an item on something else.
    /// </summary>
    internal class UseItemOnEventRule : MoveUseEventRule, IUseItemOnEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOnEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptFactory">A reference to the script factory in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public UseItemOnEventRule(ILogger logger, IScriptApi scriptFactory, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, scriptFactory, conditionSet, actionSet)
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
    }
}