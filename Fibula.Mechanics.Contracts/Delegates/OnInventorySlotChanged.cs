// -----------------------------------------------------------------
// <copyright file="OnInventorySlotChanged.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    using Fibula.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for when content changes in a slot within an inventory.
    /// </summary>
    /// <param name="inventory">The inventory that changed.</param>
    /// <param name="slot">The slot that changed.</param>
    /// <param name="item">The item in the slot, if any.</param>
    public delegate void OnInventorySlotChanged(IInventory inventory, Slot slot, IItem item);
}
