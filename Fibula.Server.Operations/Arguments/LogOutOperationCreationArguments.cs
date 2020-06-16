// -----------------------------------------------------------------
// <copyright file="LogOutOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="LogOutOperation"/>.
    /// </summary>
    public class LogOutOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOutOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="player"></param>
        public LogOutOperationCreationArguments(IPlayer player)
        {
            player.ThrowIfNull(nameof(player));

            this.Player = player;
        }

        public IPlayer Player { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Player.Id;
    }
}
