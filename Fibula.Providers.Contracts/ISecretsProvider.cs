// -----------------------------------------------------------------
// <copyright file="ISecretsProvider.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Providers.Contracts
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
        Task<IEnumerable<string>> ListSecretIdentifiers();

        /// <summary>
        /// Retrieves the specified Secret's value from the secret store.
        /// </summary>
        /// <param name="secretName">The Secret's name to lookup.</param>
        /// <returns>A <see cref="Task"/> representig the asynchronous operation.</returns>
        Task<SecureString> GetSecretValueAsync(string secretName);
    }
}
