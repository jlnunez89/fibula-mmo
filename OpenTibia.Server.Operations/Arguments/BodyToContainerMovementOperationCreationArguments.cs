// -----------------------------------------------------------------
// <copyright file="BodyToContainerMovementOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Movements;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="BodyToContainerMovementOperation"/>.
    /// </summary>
    public class BodyToContainerMovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToContainerMovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="thingMoving">The thing moving.</param>
        /// <param name="targetCreature">The creature within which the movement is happening.</param>
        /// <param name="fromSlot">The slot from which the movement is happening.</param>
        /// <param name="toContainerPosition">The position of the container to which the movement is happening, as seen by the creature.</param>
        /// <param name="toContainerIndex">The index within the container to which the movement is happening.</param>
        /// <param name="amount">The amount being moved.</param>
        public BodyToContainerMovementOperationCreationArguments(uint requestorId, IThing thingMoving, ICreature targetCreature, Slot fromSlot, byte toContainerPosition, byte toContainerIndex, byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            targetCreature.ThrowIfNull(nameof(targetCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.TargetCreature = targetCreature;
            this.FromSlot = fromSlot;
            this.ToContainerPosition = toContainerPosition;
            this.ToContainerIndex = toContainerIndex;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the thing being moved.
        /// </summary>
        public IThing ThingMoving { get; }

        /// <summary>
        /// Gets the creature within which the movement is happening.
        /// </summary>
        public ICreature TargetCreature { get; }

        /// <summary>
        /// Gets the slot from which the movement is happening.
        /// </summary>
        public Slot FromSlot { get; }

        /// <summary>
        /// Gets the position of the container to which the movement is happening, as seen by the creature.
        /// </summary>
        public byte ToContainerPosition { get; }

        /// <summary>
        /// Gets the index within the container to which the movement is happening.
        /// </summary>
        public byte ToContainerIndex { get; }

        /// <summary>
        /// Gets the amount being moved.
        /// </summary>
        public byte Amount { get; }
    }
}
