// -----------------------------------------------------------------
// <copyright file="UseItemOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Arguments
{
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="UseItemOperation"/>.
    /// </summary>
    public class UseItemOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="itemTypeId"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromIndex"></param>
        public UseItemOperationCreationArguments(uint requestorId, ushort itemTypeId, Location fromLocation, byte fromIndex)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = itemTypeId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public ushort ItemTypeId { get; }

        public Location FromLocation { get; }

        public byte FromIndex { get; }
    }
}
