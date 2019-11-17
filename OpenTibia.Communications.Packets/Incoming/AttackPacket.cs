// <copyright file="AttackPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents and attack packet.
    /// </summary>
    public class AttackPacket : IIncomingPacket, IAttackInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackPacket"/> class.
        /// </summary>
        /// <param name="targetCreatureId">The id of the creature being attacked.</param>
        public AttackPacket(uint targetCreatureId)
        {
            this.TargetCreatureId = targetCreatureId;
        }

        /// <summary>
        /// Gets the target creature's id.
        /// </summary>
        public uint TargetCreatureId { get; }
    }
}
