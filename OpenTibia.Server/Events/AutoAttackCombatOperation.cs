// -----------------------------------------------------------------
// <copyright file="AutoAttackCombatOperation.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.EventConditions;
    using Serilog;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public class AutoAttackCombatOperation : BaseCombatOperation
    {
        private const int DefaultAttackRangeDistance = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackCombatOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public AutoAttackCombatOperation(ILogger logger, IGame game, ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
            : base(logger, game, target, attacker)
        {
            attacker.ThrowIfNull(nameof(attacker));

            this.ExhaustionCost = exhaustionCost;

            this.Conditions.Add(new AttackingCorrectTargetEventCondition(attacker, attacker?.AutoAttackTarget?.Id ?? 0));
            this.Conditions.Add(new LocationsAreDistantByEventCondition(() => attacker?.Location ?? target.Location, () => target.Location, attacker?.AutoAttackRange ?? DefaultAttackRangeDistance, sameFloorOnly: true));

            // Add this next attack action to both scenarios.
            this.ActionsOnFail.Add(new GenericEventAction(() =>
            {
                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    this.Game.Request_AutoAttack(this.Attacker, this.Attacker.AutoAttackTarget);
                }
            }));

            this.ActionsOnPass.Add(new GenericEventAction(() =>
            {
                this.Game.ApplyDamage(this.Attacker, this.Target, this.AttackType, this.CalculateInflictedDamage(out bool wasBlockedByArmor, out bool wasShielded), wasBlockedByArmor, wasShielded, this.ExhaustionCost);

                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    this.Game.Request_AutoAttack(this.Attacker, this.Attacker.AutoAttackTarget);
                }
            }));
        }

        /// <summary>
        /// Gets the combat operation's attack type.
        /// </summary>
        public override AttackType AttackType => AttackType.Physical;

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.PhysicalCombat;

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        public override int MinimumDamage => 0;

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public override int MaximumDamage { get; }

        private int CalculateInflictedDamage(out bool armorBlock, out bool wasShielded)
        {
            armorBlock = false;
            wasShielded = false;

            if (this.Target.AutoDefenseCredits > 0)
            {
                wasShielded = true;

                return 0;
            }

            var rng = new Random();

            // Coin toss 25% chance to hit the armor...
            if (rng.Next(4) > 0)
            {
                return rng.Next(10) + 1;
            }

            armorBlock = true;

            return 0;
        }
    }
}