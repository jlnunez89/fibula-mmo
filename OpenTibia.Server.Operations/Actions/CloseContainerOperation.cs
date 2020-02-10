// -----------------------------------------------------------------
// <copyright file="CloseContainerOperation.cs" company="2Dudes">
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
    /// Class that represents an event for an container being closed.
    /// </summary>
    public class CloseContainerOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseContainerOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="player">The player who has the container open.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="asContainerId">The id of the container being closed, as seen by the player.</param>
        public CloseContainerOperation(ILogger logger, IOperationContext context, IPlayer player, IContainerItem containerItem, byte asContainerId)
            : base(logger, context, player.Id)
        {
            this.ActionsOnPass.Add(() =>
            {
                this.Context.ContainerManager.CloseContainer(player, containerItem, asContainerId);
            });
        }
    }
}
