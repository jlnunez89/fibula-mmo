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

using System.Collections.Generic;

namespace OpenTibia.Server.Contracts.Abstractions
{
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

        IContainerItem FindForCreature(uint creatureId, byte atPosition);

        byte FindForCreature(uint creatureId, IContainerItem container);

        IEnumerable<IContainerItem> FindAllForCreature(uint creatureId);
    }
}
