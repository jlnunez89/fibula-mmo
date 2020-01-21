// -----------------------------------------------------------------
// <copyright file="StandardAttackOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.MovementEvents.EventConditions;
    using Serilog;

    /// <summary>
    /// Class that represents a standard combat operation.
    /// </summary>
    internal class StandardAttackOperation : BaseEvent, ICombatOperation
    {
        private const int DefaultAttackRangeDistance = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardAttackOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        public StandardAttackOperation(
            ILogger logger,
            IGame game,
            ICombatant attacker,
            ICombatant target)
            : base(logger, attacker?.Id ?? 0)
        {
            game.ThrowIfNull(nameof(game));
            target.ThrowIfNull(nameof(target));

            this.Game = game;
            this.Attacker = attacker;
            this.Target = target;

            this.Conditions.Add(new AttackingCorrectTargetEventCondition(attacker, attacker?.AutoAttackTargetId ?? 0));
            this.Conditions.Add(new LocationsAreDistantByEventCondition(() => attacker?.Location ?? target.Location, () => target.Location, attacker?.AutoAttackRange ?? DefaultAttackRangeDistance));

            // Add this next attack action to both scenarios.
            this.ActionsOnFail.Add(new GenericEventAction(() =>
            {
                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTargetId > 0)
                {
                    this.Game.Request_AutoAttack(this.Attacker, this.Attacker.AutoAttackTargetId);
                }
            }));

            this.ActionsOnPass.Add(new GenericEventAction(() =>
            {
                this.Game.ApplyDamage(this.Attacker, this.Target, this.AttackType, this.CalculateInflictedDamage(out bool wasBlockedByArmor, out bool wasShielded), wasBlockedByArmor, wasShielded, this.ExhaustionCost);

                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTargetId > 0)
                {
                    this.Game.Request_AutoAttack(this.Attacker, this.Attacker.AutoAttackTargetId);
                }
            }));
        }

        /// <summary>
        /// Gets a reference to the game instance.
        /// </summary>
        public IGame Game { get; }

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
        public AttackType AttackType => AttackType.Physical;

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public TimeSpan ExhaustionCost => TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        public int MinimumDamage => throw new NotImplementedException();

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public int MaximumDamage => throw new NotImplementedException();

        private int CalculateInflictedDamage(out bool armorBlock, out bool wasShielded)
        {
            armorBlock = false;
            wasShielded = false;

            var rng = new Random();
            var inflicted = 0;

            switch (rng.Next(4))
            {
                default:
                    // damage
                    inflicted = rng.Next(10) + 1;
                    break;
                case 3:
                    armorBlock = true;
                    break;
                case 4:
                    wasShielded = true;
                    break;
            }

            return inflicted;
        }
    }
}