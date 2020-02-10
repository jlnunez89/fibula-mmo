// -----------------------------------------------------------------
// <copyright file="ContainerToBodyMovementOperation.cs" company="2Dudes">
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
    /// public class that represents a movement operation that happens from inside a container to a player's inventory.
    /// </summary>
    public class ContainerToBodyMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerToBodyMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureContainerId">The id of the container from which the movement is happening.</param>
        /// <param name="fromCreatureContainerIndex">The index in the container from which the movement is happening.</param>
        /// <param name="toCreatureSlot">The slot of the creature to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public ContainerToBodyMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            byte fromCreatureContainerId,
            byte fromCreatureContainerIndex,
            Slot toCreatureSlot,
            byte amount = 1)
            : base(logger, context, context?.ContainerManager?.FindForCreature(targetCreature.Id, fromCreatureContainerId), targetCreature?.Inventory[(byte)toCreatureSlot] as IContainerItem, creatureRequestingId)
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

                var creatureHasSourceContainerOpen = this.Context.ContainerManager.FindForCreature(targetCreature.Id, fromCreatureContainerId) != null;

                bool moveSuccessful = creatureHasSourceContainerOpen &&
                                      this.PerformItemMovement(item, this.FromCylinder, this.ToCylinder, fromCreatureContainerIndex, 0, amount, this.Requestor);

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