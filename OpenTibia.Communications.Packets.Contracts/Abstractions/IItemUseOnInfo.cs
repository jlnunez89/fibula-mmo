// -----------------------------------------------------------------
// <copyright file="IItemUseOnInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface IItemUseOnInfo
    {
        Location FromLocation { get; }

        ushort FromSpriteId { get; }

        byte FromStackPosition { get; }

        Location ToLocation { get; }

        ushort ToSpriteId { get; }

        byte ToStackPosition { get; }
    }
}