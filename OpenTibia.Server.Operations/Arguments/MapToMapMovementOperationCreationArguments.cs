// -----------------------------------------------------------------
// <copyright file="MapToMapMovementOperationCreationArguments.cs" company="2Dudes">
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

    public class MapToMapMovementOperationCreationArguments : IOperationCreationArguments
    {
        public MapToMapMovementOperationCreationArguments(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte fromStackPos = 255, byte amount = 1, bool isTeleport = false)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.FromStackPos = fromStackPos;
            this.Amount = amount;
            this.IsTeleport = isTeleport;
        }

        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public byte FromStackPos { get; }

        public byte Amount { get; }

        public bool IsTeleport { get; }
    }
}
