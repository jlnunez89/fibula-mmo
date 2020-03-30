// -----------------------------------------------------------------
// <copyright file="MapToBodyMovementOperationCreationArguments.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents creation arguments for a <see cref="MapToBodyMovementOperation"/>.
    /// </summary>
    public class MapToBodyMovementOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToBodyMovementOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toCreature"></param>
        /// <param name="toSlot"></param>
        /// <param name="amount"></param>
        public MapToBodyMovementOperationCreationArguments(uint requestorId, IThing thingMoving, Location fromLocation, ICreature toCreature, Slot toSlot, byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            toCreature.ThrowIfNull(nameof(toCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.ToCreature = toCreature;
            this.FromLocation = fromLocation;
            this.ToSlot = toSlot;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public ICreature ToCreature { get; }

        public Location FromLocation { get; }

        public Slot ToSlot { get; }

        public byte Amount { get; }
    }
}
