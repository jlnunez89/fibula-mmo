// -----------------------------------------------------------------
// <copyright file="IStatement.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using System;

    public interface IStatement
    {
        uint StatementId { get; }

        DateTimeOffset Timestamp { get; }

        uint PlayerId { get; }

        string Channel { get; }

        string Message { get; }
    }
}
