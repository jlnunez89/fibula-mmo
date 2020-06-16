// -----------------------------------------------------------------
// <copyright file="DeleteItemOperationCreationArguments.cs" company="2Dudes">
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
    /// Class that represents creation arguments for a <see cref="DeleteItemOperation"/>.
    /// </summary>
    public class DeleteItemOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="itemTypeId"></param>
        /// <param name="atLocation"></param>
        public DeleteItemOperationCreationArguments(uint requestorId, ushort itemTypeId, Location atLocation)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = itemTypeId;
            this.AtLocation = atLocation;
        }

        public ushort ItemTypeId { get; }

        public Location AtLocation { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }
    }
}
