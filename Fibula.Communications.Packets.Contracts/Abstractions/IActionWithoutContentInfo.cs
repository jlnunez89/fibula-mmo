// -----------------------------------------------------------------
// <copyright file="IActionWithoutContentInfo.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Interface for actions without any content to read.
    /// </summary>
    public interface IActionWithoutContentInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the action to do.
        /// </summary>
        public IncomingGamePacketType Action { get; }
    }
}
