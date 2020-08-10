// -----------------------------------------------------------------
// <copyright file="AttackOrchestratorOperation.cs" company="2Dudes">
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
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;

    /// <summary>
    /// Class that represents a combat operation that orchestrates attack operations.
    /// </summary>
    public class AttackOrchestratorOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackOrchestratorOperation"/> class.
        /// </summary>
        /// <param name="attacker">The combatant that is attacking.</param>
        public AttackOrchestratorOperation(ICombatant attacker)
            : base(attacker?.Id ?? 0)
        {
            attacker.ThrowIfNull(nameof(attacker));

            this.Attacker = attacker;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // We should stop this recurrent operation if there is no longer a target or it is dead.
            if (this.Attacker.AutoAttackTarget == null || this.Attacker.AutoAttackTarget.IsDead || !this.Attacker.CanSee(this.Attacker.AutoAttackTarget))
            {
                return;
            }

            // We should also stop any OTHER tracked attack orchestration operation before carrying this one out.
            if (this.Attacker.TryRetrieveTrackedOperation(nameof(AttackOrchestratorOperation), out IOperation atkOrchestrationOp) && atkOrchestrationOp != this)
            {
                // Cancel it first, and remove it.
                atkOrchestrationOp.Cancel();
            }

            // Set the attack orchestration operation as this operation.
            this.Attacker.StartTrackingEvent(this);

            // Normalize the attacker's attack speed based on the global round time and round that up.
            // We do this every time because it could have changed.
            var normalizedAttackSpeed = TimeSpan.FromMilliseconds((int)Math.Round(CombatConstants.DefaultCombatRoundTimeInMs / this.Attacker.AttackSpeed));
            var autoAttackOp = new BasicAttackOperation(this.Attacker, this.Attacker.AutoAttackTarget, normalizedAttackSpeed);
            var operationDelay = TimeSpan.Zero;

            // Add delay from current exhaustion of the requestor, if any.
            // Notice that this exhaustion comes from the actual attack operation, not *this* operation.
            if (this.Attacker is ICreatureWithExhaustion creatureWithExhaustion)
            {
                TimeSpan cooldownRemaining = creatureWithExhaustion.CalculateRemainingCooldownTime(autoAttackOp.ExhaustionType, context.Scheduler.CurrentTime);

                operationDelay += cooldownRemaining;
            }

            // Schedule the actual attack operation.
            context.Scheduler.ScheduleEvent(autoAttackOp, operationDelay);

            this.RepeatAfter = normalizedAttackSpeed;
        }
    }
}
