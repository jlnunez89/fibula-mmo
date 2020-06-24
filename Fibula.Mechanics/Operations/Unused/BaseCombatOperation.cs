// -----------------------------------------------------------------
// <copyright file="BaseCombatOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Combat
{
    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public abstract class BaseCombatOperation : Operation, ICombatOperation
    {
        private const byte DefaultAttackCreditsCost = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCombatOperation"/> class.
        /// </summary>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="attacker">Optional. The combatant that is attacking, if any.</param>
        /// <param name="attackCreditsCost">Optional. The attack credits that this operation costs. Defaults to <see cref="DefaultAttackCreditsCost"/>.</param>
        public BaseCombatOperation(ICombatant target, ICombatant attacker = null, byte attackCreditsCost = DefaultAttackCreditsCost)
            : base(attacker?.Id ?? 0)
        {
            target.ThrowIfNull(nameof(target));

            this.Target = target;
            this.Attacker = attacker;
            this.AttackCreditsCost = attackCreditsCost;
        }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Gets the combatant that is the target on this operation.
        /// </summary>
        public ICombatant Target { get; }

        /// <summary>
        /// Gets the combat operation's attack type.
        /// </summary>
        public abstract AttackType AttackType { get; }

        /// <summary>
        /// Gets the number of credits that the attacker must have to perform this operation.
        /// </summary>
        public byte AttackCreditsCost { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        public abstract int MinimumDamage { get; }

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public abstract int MaximumDamage { get; }
    }
}