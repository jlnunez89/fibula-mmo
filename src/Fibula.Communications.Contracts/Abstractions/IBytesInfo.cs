// -----------------------------------------------------------------
// <copyright file="IBytesInfo.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    /// <summary>
    /// Interface for information in the form of a bytes array.
    /// </summary>
    public interface IBytesInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the information bytes.
        /// </summary>
        byte[] Bytes { get; }
    }
}
