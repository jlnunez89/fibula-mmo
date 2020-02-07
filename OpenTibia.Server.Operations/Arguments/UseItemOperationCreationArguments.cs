// -----------------------------------------------------------------
// <copyright file="UseItemOperationCreationArguments.cs" company="2Dudes">
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

    public class UseItemOperationCreationArguments : IOperationCreationArguments
    {
        public UseItemOperationCreationArguments(uint requestorId, ushort itemTypeId, Location fromLocation, byte fromStackPos = 255, byte index = 1)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = itemTypeId;
            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;
            this.Index = index;
        }

        public uint RequestorId { get; }

        public ushort ItemTypeId { get; }

        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public byte Index { get; }
    }
}
