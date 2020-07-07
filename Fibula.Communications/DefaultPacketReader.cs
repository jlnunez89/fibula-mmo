// -----------------------------------------------------------------
// <copyright file="DefaultPacketReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications
{
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents the default packet reader.
    /// </summary>
    public sealed class DefaultPacketReader : BasePacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public DefaultPacketReader(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Reads a packet from the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The packet read from the message.</returns>
        public override IIncomingPacket ReadFromMessage(INetworkMessage message)
        {
            message.ThrowIfNull(nameof(message));

            return message.ReadAsBytesInfo();
        }
    }
}
