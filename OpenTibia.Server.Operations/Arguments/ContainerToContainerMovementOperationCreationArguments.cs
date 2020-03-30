// -----------------------------------------------------------------
// <copyright file="ContainerToContainerMovementOperationCreationArguments.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents creation arguments for a <see cref="ContainerToContainerMovementOperation"/>.
    /// </summary>
    public class ContainerToContainerMovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerToContainerMovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="targetCreature"></param>
        /// <param name="fromContainerId"></param>
        /// <param name="fromContainerIndex"></param>
        /// <param name="toContainerId"></param>
        /// <param name="toContainerIndex"></param>
        /// <param name="amount"></param>
        public ContainerToContainerMovementOperationCreationArguments(
            uint requestorId,
            IThing thingMoving,
            ICreature targetCreature,
            byte fromContainerId,
            byte fromContainerIndex,
            byte toContainerId,
            byte toContainerIndex,
            byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            targetCreature.ThrowIfNull(nameof(targetCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.TargetCreature = targetCreature;
            this.FromContainerId = fromContainerId;
            this.FromContainerIndex = fromContainerIndex;
            this.ToContainerId = toContainerId;
            this.ToContainerIndex = toContainerIndex;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public ICreature TargetCreature { get; }

        public byte FromContainerId { get; }

        public byte FromContainerIndex { get; }

        public byte ToContainerId { get; }

        public byte ToContainerIndex { get; }

        public byte Amount { get; }
    }
}
