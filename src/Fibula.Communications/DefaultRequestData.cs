// -----------------------------------------------------------------
// <copyright file="DefaultRequestData.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the default request data model.
    /// </summary>
    public sealed class DefaultRequestData : IBytesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestData"/> class.
        /// </summary>
        /// <param name="bytes">The bytes that represent the packet.</param>
        public DefaultRequestData(params byte[] bytes)
        {
            bytes.ThrowIfNull(nameof(bytes));

            this.Bytes = bytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestData"/> class.
        /// </summary>
        /// <param name="bytes">The bytes that represent the packet.</param>
        public DefaultRequestData(ReadOnlySpan<byte> bytes)
        {
            this.Bytes = bytes.ToArray();
        }

        /// <summary>
        /// Gets the collection of bytes that represent the packet.
        /// </summary>
        public byte[] Bytes { get; }
    }
}
