// -----------------------------------------------------------------
// <copyright file="ModesPacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet for fight and chase modes.
    /// </summary>
    public class ModesPacket : IIncomingPacket, IModesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModesPacket"/> class.
        /// </summary>
        /// <param name="fightMode">The fight mode selected.</param>
        /// <param name="chaseMode">The chase mode selected.</param>
        /// <param name="isSafetyEnabled">A value indicating whether the safety mode is on.</param>
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
