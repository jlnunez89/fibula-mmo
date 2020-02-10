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
        /// <param name="context">The operation's context.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureSlot">The slot of the creature from which the movement is happening.</param>
        /// <param name="toCreatureContainerId">The id of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public BodyToContainerMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            Slot fromCreatureSlot,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte amount = 1)
            : base(logger, context, targetCreature?.Inventory[(byte)fromCreatureSlot] as IContainerItem, context?.ContainerManager?.FindForCreature(targetCreature.Id, toCreatureContainerId), creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ActionsOnPass.Add(() =>
            {
                if (!(thingMoving is IItem item))
                {
                    // You may not move this.
                    return;
                }

                var creatureHasDestinationContainerOpen = this.Context.ContainerManager.FindForCreature(targetCreature.Id, toCreatureContainerId) != null;

                bool moveSuccessful = creatureHasDestinationContainerOpen &&
                                      this.PerformItemMovement(item, this.FromCylinder, this.ToCylinder, 0, toCreatureContainerIndex, amount, this.Requestor);

                if (!moveSuccessful)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }
    }
}