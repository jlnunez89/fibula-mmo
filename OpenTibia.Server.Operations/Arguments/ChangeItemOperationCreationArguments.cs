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
    using OpenTibia.Server.Operations.Actions;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="ChangeItemOperation"/>.
    /// </summary>
    public class ChangeItemOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="fromTypeId"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toTypeId"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="index"></param>
        /// <param name="carrierCreature"></param>
        public ChangeItemOperationCreationArguments(
            uint requestorId,
            ushort fromTypeId,
            Location fromLocation,
            ushort toTypeId,
            ICreature carrierCreature = null)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = fromTypeId;
            this.FromLocation = fromLocation;
            this.ToTypeId = toTypeId;
            this.Carrier = carrierCreature;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public ushort ItemTypeId { get; }

        public Location FromLocation { get; }

        public ushort ToTypeId { get; }

        public ICreature Carrier { get; }
    }
}
