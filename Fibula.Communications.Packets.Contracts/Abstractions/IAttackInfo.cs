// -----------------------------------------------------------------
// <copyright file="IAttackInfo.cs" company="2Dudes">
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
    /// <summary>
    /// Interface that represents attack information.
    /// </summary>
    public interface IAttackInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the id of the creature being attacked.
        /// </summary>
        uint TargetCreatureId { get; }
    }
}