// -----------------------------------------------------------------
// <copyright file="CreateItemOperationCreationArguments.cs" company="2Dudes">
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

    public class CreateItemOperationCreationArguments : IOperationCreationArguments
    {
        public CreateItemOperationCreationArguments(uint requestorId, ushort itemTypeId, Location atLocation)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = itemTypeId;
            this.AtLocation = atLocation;
        }

        public ushort ItemTypeId { get; }

        public Location AtLocation { get; }

        public uint RequestorId { get; }
    }
}
