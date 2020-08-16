// -----------------------------------------------------------------
// <copyright file="OnItemCreated.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Delegates
{
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when an item is created.
    /// </summary>
    /// <param name="item">The item created.</param>
    public delegate void OnItemCreated(IItem item);
}
