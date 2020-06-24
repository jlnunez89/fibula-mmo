// -----------------------------------------------------------------
// <copyright file="FibulaCosmosDbContextOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.CosmosDB
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for CosmosDB configuration.
    /// </summary>
    public class FibulaCosmosDbContextOptions
    {
        /// <summary>
        /// Gets or sets the secret name of the account endpoint, used to retrieve the value from the secrets provider.
        /// </summary>
        [Required(ErrorMessage = "A name for the account endpoint secret is required.")]
        public string AccountEndpointSecretName { get; set; }

        /// <summary>
        /// Gets or sets the secret name of the account key, used to retrieve the value from the secrets provider.
        /// </summary>
        [Required(ErrorMessage = "A name for the account key secret is required.")]
        public string AccountKeySecretName { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [Required(ErrorMessage = "A database name is required.")]
        public string DatabaseName { get; set; }
    }
}
