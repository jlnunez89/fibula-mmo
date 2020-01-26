// -----------------------------------------------------------------
// <copyright file="BaseCombatOperation.cs" company="2Dudes">
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
    using OpenTibia.Scheduling;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.EventConditions;
    using Serilog;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public abstract class BaseCombatOperation : BaseEvent, ICombatOperation
    {
        private const byte DefaultAttackCreditsCost = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCombatOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="attacker">Optional. The combatant that is attacking, if any.</param>
        /// <param name="attackCreditsCost">Optional. The cost of attack credits that this operation costs. Defaults to <see cref="DefaultAttackCreditsCost"/>.</param>
        public BaseCombatOperation(
            ILogger logger,
            IGame game,
            ICombatant target,
            ICombatant attacker = null,
            byte attackCreditsCost = DefaultAttackCreditsCost)
            : base(logger, attacker?.Id ?? 0)
        {
            game.ThrowIfNull(nameof(game));
            target.ThrowIfNull(nameof(target));

            this.Game = game;
            this.Target = target;

            this.Attacker = attacker;

            this.Conditions.Add(new EnoughAttackCreditsEventCondition(attacker, attackCreditsCost));
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
        public abstract AttackType AttackType { get; }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public abstract ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public abstract TimeSpan ExhaustionCost { get; }

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