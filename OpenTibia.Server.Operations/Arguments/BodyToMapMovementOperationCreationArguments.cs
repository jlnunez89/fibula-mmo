// -----------------------------------------------------------------
// <copyright file="BodyToMapMovementOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Movements;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="BodyToMapMovementOperation"/>.
    /// </summary>
    public class BodyToMapMovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToMapMovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="thingMoving">The thing moving.</param>
        /// <param name="fromCreature">The creature from which the movement is happening.</param>
        /// <param name="fromSlot">The slot from which the movement is happening.</param>
        /// <param name="toLocation">The location to which the movement is happening.</param>
        /// <param name="amount">The amount being moved.</param>
        public BodyToMapMovementOperationCreationArguments(uint requestorId, IThing thingMoving, ICreature fromCreature, Slot fromSlot, Location toLocation, byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            fromCreature.ThrowIfNull(nameof(fromCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.FromCreature = fromCreature;
            this.FromSlot = fromSlot;
            this.ToLocation = toLocation;
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
        /// Gets the creature from which the movement is happening.
        /// </summary>
        public ICreature FromCreature { get; }

        /// <summary>
        /// Gets the slot from which the movement is happening.
        /// </summary>
        public Slot FromSlot { get; }

        /// <summary>
        /// Gets the location to which the movement is happening.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the amount being moved.
        /// </summary>
        public byte Amount { get; }
    }
}
