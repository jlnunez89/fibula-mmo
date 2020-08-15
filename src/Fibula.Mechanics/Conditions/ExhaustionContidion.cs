// -----------------------------------------------------------------
// <copyright file="ExhaustionContidion.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Conditions
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a condition for exhaustion.
    /// </summary>
    public class ExhaustionContidion : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustionContidion"/> class.
        /// </summary>
        /// <param name="conditionType">The type of exhaustion.</param>
        /// <param name="endTime">The date and time at which the condition is set to end.</param>
        public ExhaustionContidion(ConditionType conditionType, DateTimeOffset endTime)
            : base(conditionType, endTime)
        {
            if (conditionType != ConditionType.ExhaustedAction &&
                conditionType != ConditionType.ExhaustedCombat &&
                conditionType != ConditionType.ExhaustedMagic &&
                conditionType != ConditionType.ExhaustedMovement)
            {
                throw new ArgumentException($"Invalid condition type {conditionType}.", nameof(conditionType));
            }

            this.ExcludeFromTelemetry = true;
        }

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        protected override void Pulse(IEventContext context)
        {
            // Nothing done on exhaustion.
        }
    }
}
