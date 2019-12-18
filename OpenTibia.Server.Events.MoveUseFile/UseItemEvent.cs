// -----------------------------------------------------------------
// <copyright file="UseItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events.MoveUseFile;
    using Serilog;

    /// <summary>
    /// Class that represents a use item event.
    /// </summary>
    internal class UseItemEvent : MoveUseEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptFactory">A reference to the script factory in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public UseItemEvent(ILogger logger, IScriptApi scriptFactory, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, scriptFactory, conditionSet, actionSet)
        {
            // Look for a IsType condition.
            var isTypeCondition = this.Conditions.FirstOrDefault(func => MoveUseEventRule.IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {MoveUseEventRule.IsTypeFunctionName} function.");
            }

            this.ItemToUseId = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        /// <summary>
        /// Gets the id of the item to use.
        /// </summary>
        public ushort ItemToUseId { get; }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.Use;
    }
}