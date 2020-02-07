// -----------------------------------------------------------------
// <copyright file="ContainerToMapMovementOperationCreationArguments.cs" company="2Dudes">
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

    public class ContainerToMapMovementOperationCreationArguments : IOperationCreationArguments
    {
        public ContainerToMapMovementOperationCreationArguments(
            uint requestorId,
            IThing thingMoving,
            ICreature fromCreature,
            byte fromContainerId,
            byte fromContainerIndex,
            Location toLocation,
            byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            fromCreature.ThrowIfNull(nameof(fromCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.FromCreature = fromCreature;
            this.FromContainerId = fromContainerId;
            this.FromContainerIndex = fromContainerIndex;
            this.ToLocation = toLocation;
            this.Amount = amount;
        }

        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public ICreature FromCreature { get; }

        public byte FromContainerId { get; }

        public byte FromContainerIndex { get; }

        public Location ToLocation { get; }

        public byte Amount { get; }
    }
}
