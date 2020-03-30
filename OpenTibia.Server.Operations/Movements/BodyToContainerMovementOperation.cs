// -----------------------------------------------------------------
// <copyright file="BodyToContainerMovementOperation.cs" company="2Dudes">
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
    /// public class that represents a movement operation that happens from a player's inventory to inside a container.
    /// </summary>
    public class BodyToContainerMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToContainerMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureSlot">The slot of the creature from which the movement is happening.</param>
        /// <param name="toCreatureContainerPosition">The position of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public BodyToContainerMovementOperation(
            ILogger logger,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            Slot fromCreatureSlot,
            byte toCreatureContainerPosition,
            byte toCreatureContainerIndex,
            byte amount = 1)
            : base(logger, targetCreature?.Inventory[(byte)fromCreatureSlot] as IContainerItem, context?.ContainerManager?.FindForCreature(targetCreature.Id, toCreatureContainerPosition), creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.TargetCreature = targetCreature;
            this.ToCreatureContainerIndex = toCreatureContainerIndex;
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
        /// Gets the creature within which the movement is happening.
        /// </summary>
        public ICreature TargetCreature { get; }

        /// <summary>
        /// Gets the index within the container to which the movement is happening.
        /// </summary>
        public byte ToCreatureContainerIndex { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // Declare some pre-conditions.
            var creatureHasDestinationContainerOpen = context.ContainerManager.FindForCreature(this.TargetCreature.Id, this.ToCylinder as IContainerItem) != IContainerManager.UnsetContainerPosition;

            if (!(this.ThingMoving is IItem item))
            {
                this.DispatchTextNotification(context, OperationMessage.MayNotMoveThis);
            }
            else if (!creatureHasDestinationContainerOpen)
            {
                this.DispatchTextNotification(context, OperationMessage.MustFirstOpenThatContainer);
            }
            else if (!this.PerformItemMovement(context, item, this.FromCylinder, this.ToCylinder, 0, this.ToCreatureContainerIndex, this.Amount, this.GetRequestor(context.CreatureFinder)))
            {
                // Something else went wrong.
                this.DispatchTextNotification(context);
            }
        }
    }
}