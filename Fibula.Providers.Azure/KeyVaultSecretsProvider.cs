// -----------------------------------------------------------------
// <copyright file="KeyVaultSecretsProvider.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Providers.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Fibula.Common.Utilities;
    using Fibula.Providers.Contracts;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Class that represents a secrets provider from Azure KeyVault.
    /// </summary>
    public class KeyVaultSecretsProvider : ISecretsProvider
    {
        /// <summary>
        /// The KeyVault client instance used by this provider.
        /// </summary>
        private readonly IKeyVaultClient keyVaultClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultSecretsProvider"/> class.
        /// </summary>
        /// <param name="secretsProviderOptions">A reference to the secrets provider options.</param>
        /// <param name="tokenProvider">A reference to the token provider service to use to obtain access to the vault.</param>
        /// <param name="logger">A logger for this provider.</param>
        public KeyVaultSecretsProvider(
            IOptions<KeyVaultSecretsProviderOptions> secretsProviderOptions,
            ITokenProvider tokenProvider,
            ILogger logger)
        {
            secretsProviderOptions.ThrowIfNull(nameof(secretsProviderOptions));
            tokenProvider.ThrowIfNull(nameof(tokenProvider));

            DataAnnotationsValidator.ValidateObjectRecursive(secretsProviderOptions.Value);

            this.BaseUri = new Uri(secretsProviderOptions.Value.VaultBaseUrl);
            this.keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.TokenCallback));
            this.Logger = logger.ForContext<KeyVaultSecretsProvider>();
        }

        /// <summary>
        /// Gets the KeyVault base uri currenly in use by this provider.
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// Gets the logger in use by this provider.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets a Secret's value from the KeyVault as an asynchronous operation.
        /// </summary>
        /// <param name="secretName">The name of the secret to get the value of.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<SecureString> GetSecretValueAsync(string secretName)
        {
            secretName.ThrowIfNullOrWhiteSpace(nameof(secretName));

            this.Logger.Debug($"Retrieving secret '{secretName}' from Vault...");

            // Get the secret
            var secret = (await this.keyVaultClient.GetSecretAsync(this.BaseUri.AbsoluteUri, secretName)).Value;

            // Convert secret to SecureString. 'secret' object still keeps it in plain text.
            var secureSecret = new SecureString();

            foreach (char c in secret)
            {
                secureSecret.AppendChar(c);
            }

            secureSecret.MakeReadOnly();

            return secureSecret;
        }

        /// <summary>
        /// Retrieves a list of Secret identifiers from the secret store.
        /// </summary>
        /// <returns>A list of secret idetifiers.</returns>
        public async Task<IEnumerable<string>> ListSecretIdentifiers()
        {
            this.Logger.Debug($"Getting list of secret names from Vault: {this.BaseUri}");

            List<string> secrets = new List<string>();

            var resultsPage = await this.keyVaultClient.GetSecretsAsync(this.BaseUri.AbsoluteUri);

            if (resultsPage != null)
            {
                secrets.AddRange(resultsPage.Select(item => item.Identifier.Name));
            }

            // iterate over other pages
            while (!string.IsNullOrWhiteSpace(resultsPage?.NextPageLink))
            {
                resultsPage = await this.keyVaultClient.GetSecretsNextAsync(resultsPage.NextPageLink);

                if (resultsPage != null)
                {
                    secrets.AddRange(resultsPage.Select(item => item.Identifier.Name));
                }
            }

            return secrets;
        }
    }
}
