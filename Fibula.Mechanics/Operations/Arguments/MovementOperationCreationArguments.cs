// -----------------------------------------------------------------
// <copyright file="MovementOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="MovementOperation"/>.
    /// </summary>
    public class MovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="thingId">The id of the thing moving.</param>
        /// <param name="fromLocation">The location from which the movement is happening.</param>
        /// <param name="fromIndex">The index within the location from which the movement is happening.</param>
        /// <param name="fromCreatureId">The id of the creature from which the movement is happening, if applicable.</param>
        /// <param name="toLocation">The location to which the movement is happening.</param>
        /// <param name="toCreatureId">The creature to which the movement is happening, if applicable.</param>
        /// <param name="amount">Optional. The amount of the thing being moved.</param>
        public MovementOperationCreationArguments(uint requestorId, ushort thingId, Location fromLocation, byte fromIndex, uint fromCreatureId, Location toLocation, uint toCreatureId, byte amount = 1)
        {
            this.RequestorId = requestorId;
            this.ThingId = thingId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
            this.FromCreatureId = fromCreatureId;
            this.ToLocation = toLocation;
            this.ToCreatureId = toCreatureId;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.Movement;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the id of the thing moving.
        /// </summary>
        public ushort ThingId { get; }

        /// <summary>
        /// Gets the location from which the movement is happening.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the index within the location from which the movement is happening.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Gets the id of the creature from which the movement is happening, if applicable.
        /// </summary>
        public uint FromCreatureId { get; }

        /// <summary>
        /// Gets the location to which the movement is happening.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the id of the creature to which the movement is happening, if applicable.
        /// </summary>
        public uint ToCreatureId { get; }

        /// <summary>
        /// Gets the amount of thing being moved.
        /// </summary>
        public byte Amount { get; }
    }
}
