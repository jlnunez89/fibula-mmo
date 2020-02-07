// -----------------------------------------------------------------
// <copyright file="ChangeItemOperationCreationArguments.cs" company="2Dudes">
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

    public class ChangeItemOperationCreationArguments : IOperationCreationArguments
    {
        public ChangeItemOperationCreationArguments(
            uint requestorId,
            ushort fromTypeId,
            Location fromLocation,
            ushort toTypeId,
            byte fromStackPos = 255,
            byte index = 1,
            ICreature carrierCreature = null)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = fromTypeId;
            this.FromLocation = fromLocation;
            this.ToTypeId = toTypeId;
            this.FromStackPos = fromStackPos;
            this.Index = index;
            this.Carrier = carrierCreature;
        }

        public ushort ItemTypeId { get; }

        public Location FromLocation { get; }

        public ushort ToTypeId { get; }

        public byte FromStackPos { get; }

        public byte Index { get; }

        public ICreature Carrier { get; }

        public uint RequestorId { get; }
    }
}
