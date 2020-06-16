// -----------------------------------------------------------------
// <copyright file="BasePacketReader.cs" company="2Dudes">
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
    /// Class that represents the base implementation for all packet readers in all protocols.
    /// </summary>
    public abstract class BasePacketReader : IPacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        protected BasePacketReader(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext(this.GetType());
        }

        /// <summary>
        /// Gets the reference to the logger in use.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Reads a packet from the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The packet read from the message.</returns>
        public abstract IIncomingPacket ReadFromMessage(INetworkMessage message);
    }
}