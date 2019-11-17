// -----------------------------------------------------------------
// <copyright file="IItemMoveInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface IItemMoveInfo
    {
        ushort ClientId { get; }

        Location FromLocation { get; }

        byte FromStackPos { get; }

        Location ToLocation { get; }

        byte Count { get; }
    }
}