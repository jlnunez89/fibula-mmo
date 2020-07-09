// -----------------------------------------------------------------
// <copyright file="FibulaCosmosDbContextOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
