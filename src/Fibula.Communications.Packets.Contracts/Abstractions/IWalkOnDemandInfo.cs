// -----------------------------------------------------------------
// <copyright file="IWalkOnDemandInfo.cs" company="2Dudes">
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

    /// <summary>
    /// Interface for walking on demand information supplied on a game server request.
    /// </summary>
    public interface IWalkOnDemandInfo
    {
        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        Direction Direction { get; }
    }
}
