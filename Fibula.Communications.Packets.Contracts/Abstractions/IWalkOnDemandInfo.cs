// -----------------------------------------------------------------
// <copyright file="IWalkOnDemandInfo.cs" company="2Dudes">
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
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for walking on demand information supplied on a game server request.
    /// </summary>
    public interface IWalkOnDemandInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        Direction Direction { get; }
    }
}