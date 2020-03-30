// -----------------------------------------------------------------
// <copyright file="UseItemOnOperationCreationArguments.cs" company="2Dudes">
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
    /// Class that represents creation arguments for a <see cref="UseItemOnOperation"/>.
    /// </summary>
    public class UseItemOnOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOnOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="fromItemTypeId"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toThingId"></param>
        /// <param name="toLocation"></param>
        /// <param name="toIndex"></param>
        public UseItemOnOperationCreationArguments(uint requestorId, ushort fromItemTypeId, Location fromLocation, byte fromIndex, ushort toThingId, Location toLocation, byte toIndex)
        {
            this.RequestorId = requestorId;
            this.FromItemTypeId = fromItemTypeId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
            this.ToThingId = toThingId;
            this.ToLocation = toLocation;
            this.ToIndex = toIndex;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the type id of the item that is being used.
        /// </summary>
        public ushort FromItemTypeId { get; }

        /// <summary>
        /// Gets the location of the item that is being used.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the index within the location of the item that is being used.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Gets the id of the thing on which the item is being used.
        /// </summary>
        public ushort ToThingId { get; }

        /// <summary>
        /// Gets the location of the thing on which the item is being used.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the index within the location of the thing on which the item is being used.
        /// </summary>
        public byte ToIndex { get; }
    }
}
