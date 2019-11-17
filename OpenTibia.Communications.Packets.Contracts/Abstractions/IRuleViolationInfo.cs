// <copyright file="IRuleViolationInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using System.Net;

    public interface IRuleViolationInfo
    {
        uint GamemasterId { get; }

        string CharacterName { get; }

        IPAddress IpAddress { get; }

        string Reason { get; }

        string Comment { get; }
    }
}