// -----------------------------------------------------------------
// <copyright file="IPlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using Fibula.Client.Contracts.Abstractions;

    /// <summary>
    /// Interface for character players in the game.
    /// </summary>
    public interface IPlayer : ICreature
    {
        /// <summary>
        /// Gets this player's client.
        /// </summary>
        IClient Client { get; }

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
        bool IsAllowedToLogOut { get; }
    }
}
