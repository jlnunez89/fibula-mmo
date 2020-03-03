// -----------------------------------------------------------------
// <copyright file="OpenContainerOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Actions
{
    using OpenTibia.Server.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an container being opened.
    /// </summary>
    public class OpenContainerOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenContainerOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="player">The player who has the container open.</param>
        /// <param name="containerItem">The container being opened.</param>
        /// <param name="containerPosition">Optional. The position at which the container is being opened, as seen by the player.</param>
        public OpenContainerOperation(ILogger logger, IOperationContext context, IPlayer player, IContainerItem containerItem, byte containerPosition = 0xFF)
            : base(logger, context, player.Id)
        {
            this.Player = player;
            this.ContainerItem = containerItem;
            this.ContainerPosition = containerPosition;
        }

        /// <summary>
        /// Gets the reference to the player opening the container.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Gets the reference to the container being opened.
        /// </summary>
        public IContainerItem ContainerItem { get; }

        /// <summary>
        /// Gets the position of the container with respect to the player opening it.
        /// </summary>
        public byte ContainerPosition { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute() => this.Context.ContainerManager.OpenContainer(this.Player, this.ContainerItem, this.ContainerPosition);
    }
}
