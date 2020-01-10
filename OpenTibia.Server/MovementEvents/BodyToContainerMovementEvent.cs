// -----------------------------------------------------------------
// <copyright file="BodyToContainerMovementEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.MovementEvents.EventConditions;
    using Serilog;

    /// <summary>
    /// Internal class that represents a movement event that happens from a player's inventory to inside a container.
    /// </summary>
    internal class BodyToContainerMovementEvent : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToContainerMovementEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="creatureFinder">A reference to the creture finder in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetCreatureId">The id of the creature in which the movement is happening.</param>
        /// <param name="fromCreatureSlot">The slot of the creature from which the movement is happening.</param>
        /// <param name="toCreatureContainerId">The id of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        /// <param name="evaluationTime">Optional. The evaluation time policy for this event. Defaults to <see cref="EvaluationTime.OnBoth"/>.</param>
        public BodyToContainerMovementEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint creatureRequestingId,
            IThing thingMoving,
            uint targetCreatureId,
            Slot fromCreatureSlot,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte amount = 1,
            EvaluationTime evaluationTime = EvaluationTime.OnBoth)
            : base(logger, game, connectionManager, creatureFinder, creatureRequestingId, evaluationTime)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.Conditions.Add(new ContainerIsOpenEventCondition(() => creatureFinder.FindCreatureById(targetCreatureId), toCreatureContainerId));

            var onPassAction = new GenericEventAction(() =>
            {
                bool moveSuccessful = thingMoving is IItem item &&
                                      creatureFinder.FindCreatureById(targetCreatureId) is IPlayer targetPlayer &&
                                      this.Game.PerformItemMovement(item, targetPlayer.Inventory[fromCreatureSlot] as IContainerItem, targetPlayer.GetContainerById(toCreatureContainerId), 0, toCreatureContainerIndex, amount);

                if (!moveSuccessful)
                {
                    // handles check for isPlayer.
                    this.NotifyOfFailure();

                    return;
                }

                this.Game.EvaluateMovementEventRules(thingMoving, this.Requestor);
            });

            this.ActionsOnPass.Add(onPassAction);
        }
    }
}