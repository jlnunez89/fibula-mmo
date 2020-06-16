// -----------------------------------------------------------------
// <copyright file="IModesInfo.cs" company="2Dudes">
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
    using Fibula.Server.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Interface for fight and chase modes information.
    /// </summary>
    public interface IModesInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the fight mode.
        /// </summary>
        FightMode FightMode { get; }

        /// <summary>
        /// Gets the chase mode.
        /// </summary>
        ChaseMode ChaseMode { get; }

        /// <summary>
        /// Gets a value indicating whether the safe mode is on.
        /// </summary>
        bool SafeModeOn { get; }
    }
}