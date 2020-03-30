// -----------------------------------------------------------------
// <copyright file="MapToContainerMovementOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="MapToContainerMovementOperation"/>.
    /// </summary>
    public class MapToContainerMovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToContainerMovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toCreature"></param>
        /// <param name="toContainerId"></param>
        /// <param name="toContainerIndex"></param>
        /// <param name="amount"></param>
        public MapToContainerMovementOperationCreationArguments(uint requestorId, IThing thingMoving, Location fromLocation, ICreature toCreature, byte toContainerId, byte toContainerIndex, byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            toCreature.ThrowIfNull(nameof(toCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.ToCreature = toCreature;
            this.FromLocation = fromLocation;
            this.ToContainerId = toContainerId;
            this.ToContainerIndex = toContainerIndex;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public ICreature ToCreature { get; }

        public Location FromLocation { get; }

        public byte ToContainerId { get; }

        public byte ToContainerIndex { get; }

        public byte Amount { get; }
    }
}
