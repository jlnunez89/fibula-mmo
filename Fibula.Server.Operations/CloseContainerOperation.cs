// -----------------------------------------------------------------
// <copyright file="CloseContainerOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Actions
{
    /// <summary>
    /// Class that represents an event for an container being closed.
    /// </summary>
    public class CloseContainerOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseContainerOperation"/> class.
        /// </summary>
        /// <param name="player">The player who has the container open.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="containerPosition">The position of the container being closed, as seen by the player.</param>
        public CloseContainerOperation(IPlayer player, IContainerItem containerItem, byte containerPosition)
            : base(player.Id)
        {
            this.Player = player;
            this.ContainerItem = containerItem;
            this.ContainerPosition = containerPosition;
        }

        /// <summary>
        /// Gets the reference to the player closing the container.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Gets the reference to the container being closed.
        /// </summary>
        public IContainerItem ContainerItem { get; }

        /// <summary>
        /// Gets the position of the container with respect to the player closing it.
        /// </summary>
        public byte ContainerPosition { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context) => context.ContainerManager.CloseContainer(this.Player, this.ContainerItem, this.ContainerPosition);
    }
}
