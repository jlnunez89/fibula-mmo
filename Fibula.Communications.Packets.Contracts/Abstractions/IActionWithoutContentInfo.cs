// -----------------------------------------------------------------
// <copyright file="IActionWithoutContentInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Interface for actions without any content to read.
    /// </summary>
    public interface IActionWithoutContentInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the action to do.
        /// </summary>
        public IncomingGamePacketType Action { get; }
    }
}
