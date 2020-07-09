// -----------------------------------------------------------------
// <copyright file="IGame.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
