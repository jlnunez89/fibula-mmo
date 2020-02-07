// -----------------------------------------------------------------
// <copyright file="LogOutOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    public class LogOutOperationCreationArguments : IOperationCreationArguments
    {
        public LogOutOperationCreationArguments(IPlayer player)
        {
            player.ThrowIfNull(nameof(player));

            this.Player = player;
        }

        public IPlayer Player { get; }

        public uint RequestorId => this.Player.Id;
    }
}
