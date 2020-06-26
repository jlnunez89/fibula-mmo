// -----------------------------------------------------------------
// <copyright file="CancelActionsOperation.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Extensions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents an operation that cancels other pending operations.
    /// </summary>
    public class CancelActionsOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelActionsOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the cancellation.</param>
        /// <param name="creature">The creature who's actions are being cancelled.</param>
        /// <param name="typeToCancel">Optional. The specific type of operation to cancel.</param>
        public CancelActionsOperation(uint requestorId, ICreature creature, Type typeToCancel = null)
            : base(requestorId)
        {
            this.Creature = creature;
            this.TypeToCancel = typeToCancel ?? typeof(IOperation);
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets the creature who's actions are being cancelled.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the type of operation to cancel.
        /// </summary>
        public Type TypeToCancel { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            context.Scheduler.CancelAllFor(this.Creature.Id, this.TypeToCancel);

            if (this.Creature is IPlayer player)
            {
                if (player is ICombatant playerAsCombatant)
                {
                    playerAsCombatant.SetAttackTarget(null);
                }

                context.Scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => player.YieldSingleItem(),
                        new GenericNotificationArguments(new PlayerWalkCancelPacket(this.Creature.Direction.GetClientSafeDirection()))));
            }
        }
    }
}
