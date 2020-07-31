// -----------------------------------------------------------------
// <copyright file="IFollowInfo.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface that represents follow information.
    /// </summary>
    public interface IFollowInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the id of the creature being followed.
        /// </summary>
        uint TargetCreatureId { get; }
    }
}
