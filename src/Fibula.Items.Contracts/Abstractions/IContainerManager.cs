// -----------------------------------------------------------------
// <copyright file="IContainerManager.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Items.Contracts.Constants;

    /// <summary>
    /// Interface for a container manager.
    /// </summary>
    public interface IContainerManager
    {
        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="forCreatureId">The id of the creature for which the container is being closed.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="atPosition">The position in which to close the container, for the player.</param>
        void CloseContainer(uint forCreatureId, IContainerItem containerItem, byte atPosition);

        /// <summary>
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="forCreatureId">The id of the creature for which the container is being opened.</param>
        /// <param name="containerItem">The container being opened.</param>
        /// <param name="atPosition">The position in which to open the container, for the player.</param>
        void OpenContainer(uint forCreatureId, IContainerItem containerItem, byte atPosition = ItemConstants.UnsetContainerPosition);

        /// <summary>
        /// Finds a container for a specific creature at the specified position.
        /// </summary>
        /// <param name="creatureId">The id of the creature for which to find the container.</param>
        /// <param name="atPosition">The position at which to look for the container.</param>
        /// <returns>The container found, or null if not found.</returns>
        IContainerItem FindForCreature(uint creatureId, byte atPosition);

        /// <summary>
        /// Finds the position of a specified container as seen by a specific creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature for which to find the container.</param>
        /// <param name="container">The container to look for.</param>
        /// <returns>The position of container found, or <see cref="ItemConstants.UnsetContainerPosition"/>> if not found.</returns>
        byte FindForCreature(uint creatureId, IContainerItem container);

        /// <summary>
        /// Finds all the containers known by the specified creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <returns>A collection of containers that the creature knows.</returns>
        IEnumerable<IContainerItem> FindAllForCreature(uint creatureId);
    }
}
