// -----------------------------------------------------------------
// <copyright file="IRsaDecryptor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
