// <copyright file="StatementPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class StatementPacket : IIncomingPacket, IStatementListInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatementPacket"/> class.
        /// </summary>
        /// <param name="statements"></param>
        public StatementPacket(IList<IStatement> statements)
        {
            this.Statements = statements;
        }

        public IList<IStatement> Statements { get; }
    }
}
