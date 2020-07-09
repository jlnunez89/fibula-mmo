// -----------------------------------------------------------------
// <copyright file="ITokenProvider.cs" company="2Dudes">
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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracts a token provider service.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Gets the most commonly used authentication callback: (authority, resource, scope) => async accessToken.
        /// </summary>
        Func<string, string, string, Task<string>> TokenCallback { get; }

        /// <summary>
        /// Gets an access token to access the given resource.
        /// </summary>
        /// <param name="resource">The resource to access. e.g. 'https://management.azure.com/'.</param>
        /// <param name="tenantId">If not specified, default tenant is used. Managed Service Identity REST protocols do not accept tenantId, so this can only be used with certificate and client secret based authentication.</param>
        /// <returns>The access token.</returns>
        Task<string> GetAccessTokenAsync(string resource, string tenantId = null);
    }
}
