// -----------------------------------------------------------------
// <copyright file="ITurnOnDemandInfo.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for turning on demand information supplied on a game server request.
    /// </summary>
    public interface ITurnOnDemandInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the direction to turn to.
        /// </summary>
        Direction Direction { get; }
    }
}