// -----------------------------------------------------------------
// <copyright file="DebugAssertionPacket.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class DebugAssertionPacket : IIncomingPacket, IDebugAssertionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugAssertionPacket"/> class.
        /// </summary>
        /// <param name="assertionLine"></param>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="comment"></param>
        public DebugAssertionPacket(string assertionLine, string date, string description, string comment)
        {
            assertionLine.ThrowIfNullOrWhiteSpace(nameof(assertionLine));
            date.ThrowIfNullOrWhiteSpace(nameof(date));
            description.ThrowIfNullOrWhiteSpace(nameof(description));
            comment.ThrowIfNullOrWhiteSpace(nameof(comment));

            this.AssertionLine = assertionLine;
            this.Date = date;
            this.Description = description;
            this.Comment = comment;
        }

        public string AssertionLine { get; }

        public string Date { get; }

        public string Description { get; }

        public string Comment { get; }
    }
}
