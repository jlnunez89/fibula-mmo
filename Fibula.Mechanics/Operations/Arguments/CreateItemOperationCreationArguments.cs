// -----------------------------------------------------------------
// <copyright file="CreateItemOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="CreateItemOperation"/>.
    /// </summary>
    public class CreateItemOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="itemTypeId">The type id of the item to create.</param>
        /// <param name="atLocation">The location at which to create the item.</param>
        /// <param name="attributes">The attributes to set on the new item.</param>
        public CreateItemOperationCreationArguments(uint requestorId, ushort itemTypeId, Location atLocation, IReadOnlyCollection<(ItemAttribute, IConvertible)> attributes)
        {
            this.RequestorId = requestorId;
            this.ItemTypeId = itemTypeId;
            this.AtLocation = atLocation;
            this.Attributes = attributes;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.CreateItem;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the type id of the item to create.
        /// </summary>
        public ushort ItemTypeId { get; }

        /// <summary>
        /// Gets the location at which to create the item.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Gets the attributes to set in the new item.
        /// </summary>
        public IReadOnlyCollection<(ItemAttribute, IConvertible)> Attributes { get; }
    }
}
