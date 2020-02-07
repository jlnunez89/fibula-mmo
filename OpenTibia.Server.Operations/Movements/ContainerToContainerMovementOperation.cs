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
    using OpenTibia.Server.Operations.Conditions;
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
        /// <param name="context">The context of the operation.</param>
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
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature targetCreature,
            byte fromCreatureContainerId,
            byte fromCreatureContainerIndex,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte amount = 1)
            : base(logger, context, (targetCreature as IPlayer)?.GetContainerById(fromCreatureContainerId), (targetCreature as IPlayer)?.GetContainerById(toCreatureContainerId), creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.Conditions.Add(new ContainerIsOpenEventCondition(() => targetCreature, fromCreatureContainerId));
            this.Conditions.Add(new ContainerIsOpenEventCondition(() => targetCreature, toCreatureContainerId));

            this.ActionsOnPass.Add(() =>
            {
                bool moveSuccessful = thingMoving is IItem item &&
                                      targetCreature is IPlayer targetPlayer &&
                                      this.PerformItemMovement(item, targetPlayer.GetContainerById(fromCreatureContainerId), targetPlayer.GetContainerById(toCreatureContainerId), fromCreatureContainerIndex, toCreatureContainerIndex, amount, this.Requestor);

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