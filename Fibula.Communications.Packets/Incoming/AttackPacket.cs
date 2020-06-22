// -----------------------------------------------------------------
// <copyright file="AttackPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents and attack packet.
    /// </summary>
    public class AttackPacket : IAttackInfo
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
