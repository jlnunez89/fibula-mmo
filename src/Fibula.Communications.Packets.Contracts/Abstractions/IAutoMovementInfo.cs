// -----------------------------------------------------------------
// <copyright file="IAutoMovementInfo.cs" company="2Dudes">
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
    /// Interface that represents the auto movement information.
    /// </summary>
    public interface IAutoMovementInfo
    {
        /// <summary>
        /// Gets the movement directions.
        /// </summary>
        Direction[] Directions { get; }
    }
}
