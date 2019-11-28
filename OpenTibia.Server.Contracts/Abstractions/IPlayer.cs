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
        byte PermissionsLevel { get; } // TODO: implement.

        /// <summary>
        /// Gets the player's soul points.
        /// </summary>
        byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them.

        /// <summary>
        /// Gets a value indicating whether this player is allowed to logout.
        /// </summary>
        bool IsLogoutAllowed { get; }

        // IAction PendingAction { get; }

        uint ChooseToRemoveFromKnownSet();

        bool KnowsCreatureWithId(uint creatureId);

        void AddKnownCreature(uint creatureId);

        // void SetPendingAction(IAction action);

        // void ClearPendingActions();

        // void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs);

        // sbyte OpenContainer(IContainerItem thingAsContainer);

        // sbyte GetContainerId(IContainerItem thingAsContainer);

        // void CloseContainerWithId(byte openContainerIds);

        // void OpenContainerAt(IContainerItem thingAsContainer, byte index);

        // IContainerItem GetContainer(byte container);
    }
}
