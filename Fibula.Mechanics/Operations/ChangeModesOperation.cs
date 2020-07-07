// -----------------------------------------------------------------
// <copyright file="ChangeModesOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

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
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

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

            // combatantCreature.SafeMode = this.IsSafeModeOn;
        }
    }
}
