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
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a condition for exhaustion.
    /// </summary>
    public class ExhaustionContidion : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustionContidion"/> class.
        /// </summary>
        /// <param name="exhaustionType">The type of exhaustion.</param>
        /// <param name="endTime">The date and time at which the condition is set to end.</param>
        public ExhaustionContidion(ConditionType exhaustionType, DateTimeOffset endTime)
            : base(exhaustionType, endTime)
        {
            if (exhaustionType != ConditionType.ExhaustedAction &&
                exhaustionType != ConditionType.ExhaustedCombat &&
                exhaustionType != ConditionType.ExhaustedMagic &&
                exhaustionType != ConditionType.ExhaustedMovement)
            {
                throw new ArgumentException($"Invalid condition type {exhaustionType}.", nameof(exhaustionType));
            }

            this.ExcludeFromTelemetry = true;
        }

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context for this condition.</param>
        protected override void Pulse(IConditionContext context)
        {
            // Nothing done on exhaustion.
        }
    }
}
