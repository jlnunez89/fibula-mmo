// -----------------------------------------------------------------
// <copyright file="LocalPemFileRsaDecryptor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security.Encryption
{
    using System;
    using System.IO;
    using Fibula.Common.Utilities;
    using Fibula.Security.Contracts;
    using Microsoft.Extensions.Options;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.OpenSsl;

    /// <summary>
    /// Class that implements an <see cref="IRsaDecryptor"/> that loads the private key from a file specified.
    /// </summary>
    public class LocalPemFileRsaDecryptor : IRsaDecryptor
    {
        /// <summary>
        /// Stores a reference to the RSA engine to reference while decrypting.
        /// </summary>
        private readonly RsaEngine rsaEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalPemFileRsaDecryptor"/> class.
        /// </summary>
        /// <param name="options">The options for this decryptor.</param>
        public LocalPemFileRsaDecryptor(IOptions<LocalPemFileRsaDecryptorOptions> options)
        {
            options.ThrowIfNull(nameof(options));

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.Options = options.Value;
            this.rsaEngine = new RsaEngine();

            this.InitializeEngine();
        }

        /// <summary>
        /// Gets the options for this decryptor.
        /// </summary>
        public LocalPemFileRsaDecryptorOptions Options { get; }

        /// <summary>
        /// Decrypts the data supplied.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted bytes of data.</returns>
        public Span<byte> Decrypt(byte[] data)
        {
            data.ThrowIfNull(nameof(data));

            return this.rsaEngine.ProcessBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Initializes this decryptor's RSA engine.
        /// </summary>
        private void InitializeEngine()
        {
            using StreamReader reader = File.OpenText(this.Options.FilePath);

            if (!(new PemReader(reader).ReadObject() is AsymmetricCipherKeyPair cipherKeyPair))
            {
                throw new InvalidOperationException($"Failed to create an {nameof(AsymmetricCipherKeyPair)} from the contents of the specified PEM file.");
            }

            this.rsaEngine.Init(false, cipherKeyPair.Private);
        }
    }
}
