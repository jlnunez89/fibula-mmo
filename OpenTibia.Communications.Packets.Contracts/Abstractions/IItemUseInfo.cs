// -----------------------------------------------------------------
// <copyright file="IItemUseInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface IItemUseInfo
    {
        Location FromLocation { get; }

        byte FromStackPos { get; }

        ushort ClientId { get; }

        byte Index { get; }
    }
}