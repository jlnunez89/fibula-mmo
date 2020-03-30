// -----------------------------------------------------------------
// <copyright file="ContainerToContainerMovementOperation.cs" company="2Dudes">
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
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens inside a container.
    /// </summary>
    public class ContainerToContainerMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerToContainerMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureContainerId">The id of the container from which the movement is happening.</param>
        /// <param name="fromCreatureContainerIndex">The index in the container from which the movement is happening.</param>
        /// <param name="toCreatureContainerId">The id of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public ContainerToContainerMovementOperation(
            ILogger logger,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            byte fromCreatureContainerId,
            byte fromCreatureContainerIndex,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte amount = 1)
            : base(logger, context?.ContainerManager?.FindForCreature(targetCreature.Id, fromCreatureContainerId), context?.ContainerManager?.FindForCreature(targetCreature.Id, toCreatureContainerId), creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.FromCreatureContainerIndex = fromCreatureContainerIndex;
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
        /// Gets the index within the container from which the movement is happening.
        /// </summary>
        public byte FromCreatureContainerIndex { get; }

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
            var creatureHasSourceContainerOpen = this.FromCylinder != null;
            var creatureHasDestinationContainerOpen = this.ToCylinder != null;

            if (!(this.ThingMoving is IItem item))
            {
                this.DispatchTextNotification(context, OperationMessage.MayNotMoveThis);
            }
            else if (!creatureHasSourceContainerOpen)
            {
                this.DispatchTextNotification(context, OperationMessage.MustFirstOpenThatContainer);
            }
            else if (!creatureHasDestinationContainerOpen)
            {
                this.DispatchTextNotification(context, OperationMessage.MustFirstOpenThatContainer);
            }
            else if (!this.PerformItemMovement(context, item, this.FromCylinder, this.ToCylinder, this.FromCreatureContainerIndex, this.ToCreatureContainerIndex, this.Amount, this.GetRequestor(context.CreatureFinder)))
            {
                // Something else went wrong.
                this.DispatchTextNotification(context);
            }
        }
    }
}