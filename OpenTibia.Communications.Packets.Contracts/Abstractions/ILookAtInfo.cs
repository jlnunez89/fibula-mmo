﻿// -----------------------------------------------------------------
// <copyright file="ILookAtInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface ILookAtInfo
    {
        Location Location { get; }

        ushort ThingId { get; }

        byte StackPosition { get; }
    }
}