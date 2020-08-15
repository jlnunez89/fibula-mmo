// -----------------------------------------------------------------
// <copyright file="ChangeModesOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a change modes operation.
    /// </summary>
    public class ChangeModesOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeModesOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature setting the modes.</param>
        /// <param name="fightMode">The fight mode to set.</param>
        /// <param name="chaseMode">The chase mode to set.</param>
        /// <param name="safeModeOn">A value indicating whether the safety mode is on.</param>
        public ChangeModesOperation(uint requestorId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn)
            : base(requestorId)
        {
            this.FightMode = fightMode;
            this.ChaseMode = chaseMode;
            this.IsSafeModeOn = safeModeOn;
        }

        /// <summary>
        /// Gets the fight mode to set.
        /// </summary>
        public FightMode FightMode { get; }

        /// <summary>
        /// Gets the chase mode to set.
        /// </summary>
        public ChaseMode ChaseMode { get; }

        /// <summary>
        /// Gets a value indicating whether the safety mode should be set to on.
        /// </summary>
        public bool IsSafeModeOn { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var onCreature = this.GetRequestor(context.CreatureFinder);

            if (onCreature == null || !(onCreature is ICombatant combatantCreature))
            {
                return;
            }

            context.Logger.Debug($"{onCreature.Name} changed modes to {this.FightMode} and {this.ChaseMode}, safety: {this.IsSafeModeOn}.");

            combatantCreature.FightMode = this.FightMode;
            combatantCreature.ChaseMode = this.ChaseMode;

            if (this.ChaseMode == ChaseMode.Chase && combatantCreature.AutoAttackTarget != null)
            {
                combatantCreature.SetFollowTarget(combatantCreature.AutoAttackTarget);
            }
            else if (this.ChaseMode == ChaseMode.Stand)
            {
                combatantCreature.SetFollowTarget(null);
            }

            /* combatantCreature.SafeMode = this.IsSafeModeOn; */
        }
    }
}
