// -----------------------------------------------------------------
// <copyright file="ItemCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Class that implements <see cref="IThingCreationArguments"/>, for the creation of an item.
    /// </summary>
    public class ItemCreationArguments : IThingCreationArguments
    {
        /// <summary>
        /// Gets or sets the id of type for the item to create.
        /// </summary>
        public ushort TypeId { get; set; }

        /// <summary>
        /// Gets or sets the attributes to set in the item to create.
        /// </summary>
        public IReadOnlyCollection<(ItemAttribute, IConvertible)> Attributes { get; set; }
    }
}
