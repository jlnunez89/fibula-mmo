// -----------------------------------------------------------------
// <copyright file="IHandlerSelector.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for selectors of handlers.
    /// </summary>
    public interface IHandlerSelector
    {
        /// <summary>
        /// Registers a handler for a given packet type.
        /// </summary>
        /// <param name="packetType">The type of packet to register for.</param>
        /// <param name="handler">The handler to register.</param>
        void RegisterForPacketType(Type packetType, IHandler handler);

        /// <summary>
        /// Returns the most appropriate handler for the specified packet type.
        /// </summary>
        /// <param name="packet">The packet to select the handler for.</param>
        /// <returns>An instance of an <see cref="IHandler"/> implementaion.</returns>
        IHandler SelectForPacket(IIncomingPacket packet);
    }
}
