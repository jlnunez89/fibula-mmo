// -----------------------------------------------------------------
// <copyright file="BodyToBodyMovementOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Movements
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from and to a player's inventory.
    /// </summary>
    public class BodyToBodyMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToBodyMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation's context.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureSlot">The slot of the creature from which the movement is happening.</param>
        /// <param name="toCreatureSlot">The slot of the creature to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public BodyToBodyMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            Slot fromCreatureSlot,
            Slot toCreatureSlot,
            byte amount = 1)
            : base(logger, context, targetCreature?.Inventory[(byte)fromCreatureSlot] as IContainerItem, targetCreature?.Inventory[(byte)toCreatureSlot] as IContainerItem, creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets a reference to the thing moving.
        /// </summary>
        public IThing ThingMoving { get; }

        /// <summary>
        /// Gets the amount of the thing moving.
        /// </summary>
        public byte Amount { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (!(this.ThingMoving is IItem item))
            {
                this.SendFailureNotification(OperationMessage.MayNotMoveThis);
            }
            else if (!this.PerformItemMovement(item, this.FromCylinder, this.ToCylinder, 0, 0, this.Amount, this.Requestor))
            {
                // Something else went wrong.
                this.SendFailureNotification();
            }
        }
    }
}