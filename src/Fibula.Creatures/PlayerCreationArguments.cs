// -----------------------------------------------------------------
// <copyright file="PlayerCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using Fibula.Client.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for players.
    /// </summary>
    public class PlayerCreationArguments : CreatureCreationArguments
    {
        /// <summary>
        /// Gets or sets the client to initialize the player with.
        /// </summary>
        public IClient Client { get; set; }
    }
}
