// -----------------------------------------------------------------
// <copyright file="MovementOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Movements;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="MovementOperation"/>.
    /// </summary>
    public class MovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="clientId"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromIndex"></param>
        /// <param name="fromCreatureId"></param>
        /// <param name="toLocation"></param>
        /// <param name="toCreatureId"></param>
        /// <param name="amount"></param>
        public MovementOperationCreationArguments(uint requestorId, ushort clientId, Location fromLocation, byte fromIndex, uint fromCreatureId, Location toLocation, uint toCreatureId, byte amount = 1)
        {
            this.RequestorId = requestorId;
            this.ThingId = clientId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
            this.FromCreatureId = fromCreatureId;
            this.ToLocation = toLocation;
            this.ToCreatureId = toCreatureId;
            this.Amount = amount;
        }

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
