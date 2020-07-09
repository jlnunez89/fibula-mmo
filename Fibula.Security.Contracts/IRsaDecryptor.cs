// -----------------------------------------------------------------
// <copyright file="IRsaDecryptor.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security.Contracts
{
    using System;

    /// <summary>
    /// Interface for all RSA decryptor implementations.
    /// </summary>
    public interface IRsaDecryptor
    {
        /// <summary>
        /// Decrypts the data supplied.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted bytes of data.</returns>
        Span<byte> Decrypt(byte[] data);
    }
}
