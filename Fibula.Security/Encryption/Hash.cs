// -----------------------------------------------------------------
// <copyright file="Hash.cs" company="2Dudes">
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
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Static class with helper methods for hashing.
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// An instance of the SHA 256 algorithm.
        /// </summary>
        private static readonly SHA256 Sha = new SHA256Managed();

        /// <summary>
        /// Encrypts a string using the SHA256 (Secure Hash Algorithm) algorithm.
        /// Details: http://www.itl.nist.gov/fipspubs/fip180-1.htm
        /// This works in the same manner as MD5, providing however 256bit encryption.
        /// </summary>
        /// <param name="data">A string containing the data to encrypt.</param>
        /// <returns>A string containing the string, encrypted with the SHA256 algorithm.</returns>
        public static string Sha256Hash(string data)
        {
            byte[] hash = Sha.ComputeHash(Encoding.Unicode.GetBytes(data));

            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
        }
    }
}
