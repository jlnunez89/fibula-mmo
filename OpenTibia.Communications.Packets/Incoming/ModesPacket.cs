// -----------------------------------------------------------------
// <copyright file="ModesPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet for fight and chase modes.
    /// </summary>
    public class ModesPacket : IIncomingPacket, IModesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModesPacket"/> class.
        /// </summary>
        /// 
        public ModesPacket(FightMode fightMode, ChaseMode chaseMode, bool isSafetyEnabled)
        {
            this.FightMode = fightMode;
            this.ChaseMode = chaseMode;
            this.SafeModeOn = isSafetyEnabled;
        }

        /// <summary>
        /// Gets the fight mode.
        /// </summary>
        public FightMode FightMode { get; }

        /// <summary>
        /// Gets the chase mode.
        /// </summary>
        public ChaseMode ChaseMode { get; }

        /// <summary>
        /// Gets a value indicating whether the safe mode is on.
        /// </summary>
        public bool SafeModeOn { get; }
    }
}
