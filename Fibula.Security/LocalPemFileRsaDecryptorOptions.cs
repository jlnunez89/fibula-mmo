// -----------------------------------------------------------------
// <copyright file="LocalPemFileRsaDecryptorOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
