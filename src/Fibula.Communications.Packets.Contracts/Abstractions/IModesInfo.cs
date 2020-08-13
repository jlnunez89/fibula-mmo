// -----------------------------------------------------------------
// <copyright file="IModesInfo.cs" company="2Dudes">
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
    /// Interface for fight and chase modes information.
    /// </summary>
    public interface IModesInfo
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
