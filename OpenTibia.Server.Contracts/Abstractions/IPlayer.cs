// -----------------------------------------------------------------
// <copyright file="IPlayer.cs" company="2Dudes">
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
    /// Interface for character players in the game.
    /// </summary>
    public interface IPlayer : ICreature
    {
        /// <summary>
        /// Gets the player's character id.
        /// </summary>
        string CharacterId { get; }

        /// <summary>
        /// Gets the player's permissions level.
        /// </summary>
        // TODO: implement.
        byte PermissionsLevel { get; }

        /// <summary>
        /// Gets the player's soul points.
        /// </summary>
        // TODO: nobody likes soulpoints... figure out what to do with them.
        byte SoulPoints { get; }

        /// <summary>
        /// Gets a value indicating whether this player is allowed to logout.
        /// </summary>
        bool IsLogoutAllowed { get; }

        /// <summary>
        /// Gets the collection of open containers tracked by this player.
        /// </summary>
        IEnumerable<IContainerItem> OpenContainers { get; }

        // IAction PendingAction { get; }

        /// <summary>
        /// Checks if this player knows the given creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature to check.</param>
        /// <returns>True if the player knows the creature, false otherwise.</returns>
        bool KnowsCreatureWithId(uint creatureId);

        /// <summary>
        /// Adds the given creature to this player's known collection.
        /// </summary>
        /// <param name="creatureId">The id of the creature to add to the known creatures collection.</param>
        void AddKnownCreature(uint creatureId);

        /// <summary>
        /// Chooses a creature to remove from this player's known creatures collection, if it has reached the collection size limit.
        /// </summary>
        /// <returns>The id of the chosen creature, if any, or <see cref="uint.MinValue"/> if no creature was chosen.</returns>
        uint ChooseCreatureToRemoveFromKnownSet();

        // void SetPendingAction(IAction action);

        // void ClearPendingActions();

        // void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs);

        /// <summary>
        /// Opens a container for this player, which tracks it.
        /// </summary>
        /// <param name="container">The container being opened.</param>
        /// <returns>The id of the container as seen by this player.</returns>
        sbyte OpenContainer(IContainerItem container);

        /// <summary>
        /// Gets the id of the given container as known by this player, if it is.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>The id of the container if known by this player.</returns>
        sbyte GetContainerId(IContainerItem container);

        /// <summary>
        /// Closes a container for this player, which stops tracking int.
        /// </summary>
        /// <param name="containerId">The id of the container being closed.</param>
        void CloseContainerWithId(byte containerId);

        /// <summary>
        /// Opens a container by placing it at the given index id.
        /// If there is a container already open at this index, it is first closed.
        /// </summary>
        /// <param name="container">The container to open.</param>
        /// <param name="index">The index at which to open the container.</param>
        void OpenContainerAt(IContainerItem container, byte index);

        /// <summary>
        /// Gets a container by the id known to this player.
        /// </summary>
        /// <param name="containerId">The id of the container.</param>
        /// <returns>The container, if found.</returns>
        IContainerItem GetContainerById(byte containerId);
    }
}
