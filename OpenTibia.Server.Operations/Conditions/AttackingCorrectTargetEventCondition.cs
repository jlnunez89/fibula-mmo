// -----------------------------------------------------------------
// <copyright file="AttackingCorrectTargetEventCondition.cs" company="2Dudes">
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
    /// Class that represents an event condition that evaluates whether the attacker is still attacking the target they were when the operation was scheduled.
    /// </summary>
    public class AttackingCorrectTargetEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingCorrectTargetEventCondition"/> class.
        /// </summary>
        /// <param name="attacker">The attacker, if any.</param>
        /// <param name="targetId">The id of the target, to check for.</param>
        public AttackingCorrectTargetEventCondition(ICombatant attacker, uint targetId)
        {
            this.Attacker = attacker;
            this.LastKnownTargetId = targetId;
        }

        /// <inheritdoc/>
        public string ErrorMessage => string.Empty;

        /// <summary>
        /// Gets the current attacker, if any.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Gets the last known target id.
        /// </summary>
        public uint LastKnownTargetId { get; }

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Attacker == null)
            {
                return false;
            }

            return this.Attacker.AutoAttackTarget?.Id == this.LastKnownTargetId;
        }
    }
}