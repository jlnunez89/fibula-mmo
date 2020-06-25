// -----------------------------------------------------------------
// <copyright file="IGame.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Interface for the game service.
    /// </summary>
    public interface IGame : IHostedService, IGameOperationsApi, ICombatOperationsApi
    {
        /// <summary>
        /// Gets the game world's information.
        /// </summary>
        IWorldInformation WorldInfo { get; }

        // void OnPlayerInventoryChanged(IInventory inventory, Slot slot, IItem item);
    }
}