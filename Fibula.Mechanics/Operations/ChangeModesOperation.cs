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

    /// <summary>
    /// Class that represents a change modes operation.
    /// </summary>
    public class ChangeModesOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeModesOperation"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="fightMode"></param>
        /// <param name="chaseMode"></param>
        /// <param name="safeModeOn"></param>
        public ChangeModesOperation(uint requestorId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn)
            : base(requestorId)
        {
            this.FightMode = fightMode;
            this.ChaseMode = chaseMode;
            this.IsSafeModeOn = safeModeOn;
        }

        ///// <summary>
        ///// Gets the type of exhaustion that this operation produces.
        ///// </summary>
        //public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        public FightMode FightMode { get; }

        public ChaseMode ChaseMode { get; }

        public bool IsSafeModeOn { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var onCreature = this.GetRequestor(context.CreatureFinder);

            if (onCreature == null)
            {
                return;
            }

            context.Logger.Debug($"{onCreature.Name} changed modes to {this.FightMode} and {this.ChaseMode}, safety: {this.IsSafeModeOn}.");

            // TODO: update creature modes here.
        }
    }
}