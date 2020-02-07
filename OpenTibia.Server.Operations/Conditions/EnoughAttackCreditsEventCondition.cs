// -----------------------------------------------------------------
// <copyright file="EnoughAttackCreditsEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Conditions
{
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an event condition that evaluates whether the attacker has enough attack credits to perform the attack.
    /// </summary>
    public class EnoughAttackCreditsEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnoughAttackCreditsEventCondition"/> class.
        /// </summary>
        /// <param name="attacker">The attacker, if any.</param>
        /// <param name="creditsCost">The minumum number of credits that the attacker must have available.</param>
        public EnoughAttackCreditsEventCondition(ICombatant attacker, byte creditsCost)
        {
            this.Attacker = attacker;
            this.MinimumAttackCredits = creditsCost;
        }

        /// <inheritdoc/>
        public string ErrorMessage => string.Empty;

        /// <summary>
        /// Gets the current attacker, if any.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Gets the minimum number of credits that the attacker must have.
        /// </summary>
        public byte MinimumAttackCredits { get; }

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Attacker == null)
            {
                return false;
            }

            return this.Attacker.AutoAttackCredits >= this.MinimumAttackCredits;
        }
    }
}