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
    public interface IPlayer : ICombatant
    {
        /// <summary>
        /// Gets the player's character id.
        /// </summary>
        string CharacterId { get; }

        /// <summary>
        /// Gets the player's permissions level.
        /// </summary>
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
    }
}
