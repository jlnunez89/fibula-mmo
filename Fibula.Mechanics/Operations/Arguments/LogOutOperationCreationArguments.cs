// -----------------------------------------------------------------
// <copyright file="LogOutOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Operations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="LogOutOperation"/>.
    /// </summary>
    public class LogOutOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOutOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="player">The player attempting to log out.</param>
        public LogOutOperationCreationArguments(IPlayer player)
        {
            player.ThrowIfNull(nameof(player));

            this.Player = player;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.LogOut;

        /// <summary>
        /// Gets the id of the player logging out.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Player.Id;
    }
}
