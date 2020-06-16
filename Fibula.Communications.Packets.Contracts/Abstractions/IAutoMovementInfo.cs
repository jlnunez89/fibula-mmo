// -----------------------------------------------------------------
// <copyright file="IAutoMovementInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions.RequestInfo
{
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface that represents the auto movement information.
    /// </summary>
    public interface IAutoMovementInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the movement directions.
        /// </summary>
        Direction[] Directions { get; }
    }
}