// -----------------------------------------------------------------
// <copyright file="AadTokenMsiBasedProvider.cs" company="2Dudes">
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
    using System.Threading.Tasks;
    using Fibula.Providers.Contracts;
    using Microsoft.Azure.Services.AppAuthentication;

    /// <summary>
    /// Class that adapts the concrete <see cref="AzureServiceTokenProvider"/> implementation to the <see cref="ITokenProvider"/> interface.
    /// </summary>
    public class AadTokenMsiBasedProvider : ITokenProvider
    {
        /// <summary>
        /// The Azure token service instance to work with.
        /// </summary>
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AadTokenMsiBasedProvider"/> class.
        /// </summary>
        public AadTokenMsiBasedProvider()
        {
            this.azureServiceTokenProvider = new AzureServiceTokenProvider();
        }

        /// <summary>
        /// Gets the most commonly used authentication callback: (authority, resource, scope) => async accessToken.
        /// </summary>
        public Func<string, string, string, Task<string>> TokenCallback => (authority, resource, scope) =>
        {
            return this.azureServiceTokenProvider.KeyVaultTokenCallback(authority, resource, scope);
        };

        /// <summary>
        /// Gets an access token to access the given Azure resource.
        /// </summary>
        /// <param name="resource">The resource to access. e.g. 'https://management.azure.com/'.</param>
        /// <param name="tenantId">If not specified, default tenant is used. Managed Service Identity REST protocols do not accept tenantId, so this can only be used with certificate and client secret based authentication.</param>
        /// <returns>The access token.</returns>
        public async Task<string> GetAccessTokenAsync(string resource, string tenantId = null)
        {
            return await this.azureServiceTokenProvider.GetAccessTokenAsync(resource, tenantId);
        }
    }
}
