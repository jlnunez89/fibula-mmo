// -----------------------------------------------------------------
// <copyright file="StatementPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Collections.Generic;
    using Fibula.Communications.Contracts.Abstractions;
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
