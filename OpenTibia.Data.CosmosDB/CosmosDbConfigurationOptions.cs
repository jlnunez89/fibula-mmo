// -----------------------------------------------------------------
// <copyright file="CosmosDbConfigurationOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.CosmosDB
{
    /// <summary>
    /// Class that represents options for CosmosDB configuration.
    /// </summary>
    public class CosmosDbConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the secret name of the account endpoint.
        /// </summary>
        public string AccountEndpointSecretName { get; set; }

        /// <summary>
        /// Gets or sets the secret name of the account key.
        /// </summary>
        public string AccountKeySecretName { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
