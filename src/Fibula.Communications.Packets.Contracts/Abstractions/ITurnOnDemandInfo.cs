// -----------------------------------------------------------------
// <copyright file="ITurnOnDemandInfo.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
