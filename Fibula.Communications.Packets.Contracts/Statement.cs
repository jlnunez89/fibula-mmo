// -----------------------------------------------------------------
// <copyright file="Statement.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using System;

    public class Statement : IStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Statement"/> class.
        /// </summary>
        /// <param name="statementId"></param>
        /// <param name="timestamp"></param>
        /// <param name="playerId"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        public Statement(uint statementId, DateTimeOffset timestamp, uint playerId, string channel, string message)
        {
            this.StatementId = statementId;
            this.Timestamp = timestamp;
            this.PlayerId = playerId;
            this.Channel = channel;
            this.Message = message;
        }

        public uint StatementId { get; }

        public DateTimeOffset Timestamp { get; }

        public uint PlayerId { get; }

        public string Channel { get; }

        public string Message { get; }
    }
}
