// -----------------------------------------------------------------
// <copyright file="IContainerManager.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a container manager.
    /// </summary>
    public interface IContainerManager
    {
        /// <summary>
        /// The position to use for unset or new containers.
        /// </summary>
        const byte UnsetContainerPosition = 0xFF;

        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="forCreature">The creature for which the container is being closed.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="atPosition">The position in which to close the container, for the player.</param>
        void CloseContainer(ICreature forCreature, IContainerItem containerItem, byte atPosition);

        /// <summary>
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="forCreature">The creature for which the container is being opened.</param>
        /// <param name="containerItem">The container being opened.</param>
        /// <param name="atPosition">The position in which to open the container, for the player.</param>
        void OpenContainer(ICreature forCreature, IContainerItem containerItem, byte atPosition = UnsetContainerPosition);

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
        /// <returns>The position of container found, or <see cref="UnsetContainerPosition"/>> if not found.</returns>
        byte FindForCreature(uint creatureId, IContainerItem container);

        /// <summary>
        /// Finds all the containers known by the specified creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <returns>A collection of containers that the creature knows.</returns>
        IEnumerable<IContainerItem> FindAllForCreature(uint creatureId);
    }
}
