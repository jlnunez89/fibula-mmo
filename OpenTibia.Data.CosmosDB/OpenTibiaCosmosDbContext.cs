// -----------------------------------------------------------------
// <copyright file="OpenTibiaCosmosDbContext.cs" company="2Dudes">
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
    using System.Security;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Data.Contracts.Abstractions;
    using OpenTibia.Data.Entities;
    using OpenTibia.Providers.Contracts;

    /// <summary>
    /// Class that represents a CosmosDB context.
    /// </summary>
    public class OpenTibiaCosmosDbContext : DbContext, IOpenTibiaDbContext
    {
        /// <summary>
        /// The name for the container which holds account information.
        /// </summary>
        private const string AccountsContainerName = "Accounts";

        /// <summary>
        /// The name for the container which holds character information.
        /// </summary>
        private const string CharactersContainerName = "Characters";

        /// <summary>
        /// A lock object for <see cref="accountEndpoint"/> initialization.
        /// </summary>
        private static readonly object AccountEndpointLock = new object();

        /// <summary>
        /// A lock object for <see cref="accountkey"/> initialization.
        /// </summary>
        private static readonly object AccountkeyLock = new object();

        /// <summary>
        /// Holds the account endpoint as a secure string in memory.
        /// </summary>
        private static SecureString accountEndpoint;

        /// <summary>
        /// Holds the account key as a secure string in memory.
        /// </summary>
        private static SecureString accountkey;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTibiaCosmosDbContext"/> class.
        /// </summary>
        /// <param name="cosmosDbConfigurationOptions">A reference to the CosmosDb configuration options.</param>
        /// <param name="secretsProvider">A reference to the secrets provider.</param>
        public OpenTibiaCosmosDbContext(
            IOptions<CosmosDbConfigurationOptions> cosmosDbConfigurationOptions,
            ISecretsProvider secretsProvider)
        {
            cosmosDbConfigurationOptions.ThrowIfNull(nameof(cosmosDbConfigurationOptions));
            secretsProvider.ThrowIfNull(nameof(secretsProvider));

            this.CosmosDbConfiguration = cosmosDbConfigurationOptions.Value;
            this.SecretsProvider = secretsProvider;
        }

        /// <summary>
        /// Gets the application's context.
        /// </summary>
        public CosmosDbConfigurationOptions CosmosDbConfiguration { get; }

        /// <summary>
        /// Gets the reference to the secrets provider.
        /// </summary>
        public ISecretsProvider SecretsProvider { get; }

        private string AccountEndpoint
        {
            get
            {
                if (OpenTibiaCosmosDbContext.accountEndpoint == null)
                {
                    lock (OpenTibiaCosmosDbContext.AccountEndpointLock)
                    {
                        if (OpenTibiaCosmosDbContext.accountEndpoint == null)
                        {
                            // Attempt to retrieve secret using the provider.
                            OpenTibiaCosmosDbContext.accountEndpoint = this.SecretsProvider.GetSecretValueAsync(this.CosmosDbConfiguration.AccountEndpointSecretName).Result;
                        }
                    }
                }

                // One-liner hack to decode a secure string.
                return new System.Net.NetworkCredential(string.Empty, OpenTibiaCosmosDbContext.accountEndpoint).Password;
            }
        }

        private string AccountKey
        {
            get
            {
                if (OpenTibiaCosmosDbContext.accountkey == null)
                {
                    lock (OpenTibiaCosmosDbContext.AccountkeyLock)
                    {
                        if (OpenTibiaCosmosDbContext.accountkey == null)
                        {
                            // Attempt to retrieve secret using the provider.
                            OpenTibiaCosmosDbContext.accountkey = this.SecretsProvider.GetSecretValueAsync(this.CosmosDbConfiguration.AccountKeySecretName).Result;
                        }
                    }
                }

                // One-liner hack to decode a secure string.
                return new System.Net.NetworkCredential(string.Empty, OpenTibiaCosmosDbContext.accountkey).Password;
            }
        }

        /// <summary>
        /// Gets this context as the <see cref="DbContext"/>.
        /// </summary>
        /// <returns>This instance casted as <see cref="DbContext"/>.</returns>
        public DbContext AsDbContext()
        {
            return this;
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This actually sets up the link to CosmosDb
            optionsBuilder.UseCosmos(this.AccountEndpoint, this.AccountKey, this.CosmosDbConfiguration.DatabaseName);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>()
                .ToContainer(AccountsContainerName)
                .HasPartitionKey(a => a.Id);

            modelBuilder.Entity<CharacterEntity>()
                .ToContainer(CharactersContainerName)
                .HasPartitionKey(c => c.Id);

            // TODO: Add more entities here.
        }
    }
}
