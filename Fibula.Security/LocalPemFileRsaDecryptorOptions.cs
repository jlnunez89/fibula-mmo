// -----------------------------------------------------------------
// <copyright file="LocalPemFileRsaDecryptorOptions.cs" company="2Dudes">
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="LocalPemFileRsaDecryptor"/>.
    /// </summary>
    public class LocalPemFileRsaDecryptorOptions
    {
        /// <summary>
        /// Gets or sets the path to the PEM file.
        /// </summary>
        [Required(ErrorMessage = "A path for the .pem file must be speficied.")]
        public string FilePath { get; set; }
    }
}