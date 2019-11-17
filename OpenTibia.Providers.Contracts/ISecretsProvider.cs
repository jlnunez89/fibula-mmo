// -----------------------------------------------------------------
// <copyright file="ISecretsProvider.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Providers.Contracts
{
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a Secrets provider.
    /// </summary>
    public interface ISecretsProvider
    {
        /// <summary>
        /// Retrieves a list of Secret identifiers from the secret store.
        /// </summary>
        /// <returns>A list of secret idetifiers.</returns>
        Task<IEnumerable<string>> ListSecrets();

        /// <summary>
        /// Retrieves the specified Secret's value from the secret store.
        /// </summary>
        /// <param name="secretName">The Secret's name to lookup.</param>
        /// <returns>A <see cref="Task"/> representig the asynchronous operation.</returns>
        Task<SecureString> GetSecretValueAsync(string secretName);
    }
}
