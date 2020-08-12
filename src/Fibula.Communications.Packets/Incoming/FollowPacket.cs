// -----------------------------------------------------------------
// <copyright file="FollowPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a follow packet.
    /// </summary>
    public class FollowPacket : IFollowInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FollowPacket"/> class.
        /// </summary>
        /// <param name="targetCreatureId">The id of the creature being followed.</param>
        public FollowPacket(uint targetCreatureId)
        {
            this.TargetCreatureId = targetCreatureId;
        }

        /// <summary>
        /// Gets the target creature's id.
        /// </summary>
        public uint TargetCreatureId { get; }
    }
}
