// -----------------------------------------------------------------
// <copyright file="IHasInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for any entity in the game that keeps an inventory.
    /// </summary>
    public interface IHasInventory
    {
        /// <summary>
        /// Gets the inventory for the entity.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="SkillType"/>, and the value is a <see cref="ISkill"/>.
        /// </remarks>
        IInventory Inventory { get; }
    }
}