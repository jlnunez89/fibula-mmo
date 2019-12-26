// -----------------------------------------------------------------
// <copyright file="OpenTibiaInMemoryDatabaseContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.InMemoryDatabase
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using OpenTibia.Data.Contracts.Abstractions;
    using OpenTibia.Data.Entities;

    /// <summary>
    /// Class that represents a context for an in-memory database.
    /// </summary>
    public class OpenTibiaInMemoryDatabaseContext : DbContext, IOpenTibiaDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTibiaInMemoryDatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options to initialize this context with.</param>
        public OpenTibiaInMemoryDatabaseContext(DbContextOptions<OpenTibiaInMemoryDatabaseContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the set of accounts.
        /// </summary>
        public DbSet<AccountEntity> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the set of characters.
        /// </summary>
        public DbSet<CharacterEntity> Characters { get; set; }

        /// <summary>
        /// Gets this context as the <see cref="DbContext"/>.
        /// </summary>
        /// <returns>This instance casted as <see cref="DbContext"/>.</returns>
        public DbContext AsDbContext()
        {
            return this;
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var accountId = Guid.NewGuid().ToString();

            modelBuilder.Entity<AccountEntity>()
                .HasData(new AccountEntity()
                {
                    Id = accountId,
                    AccessLevel = 0,
                    Banished = false,
                    BanishedUntil = default,
                    Creation = DateTimeOffset.UtcNow,
                    CreationIp = "127.0.0.1",
                    Deleted = false,
                    Email = "someone@email.com",
                    LastLogin = DateTimeOffset.UtcNow,
                    LastLoginIp = null,
                    Number = 1,
                    Password = "1",
                    Premium = false,
                    PremiumDays = 0,
                });

            modelBuilder.Entity<CharacterEntity>()
                .HasData(new CharacterEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Player One",
                    AccountId = accountId,
                    Vocation = "None",
                    World = "OpenTibia",
                    Level = 1,
                    Gender = 0,
                    Creation = DateTimeOffset.UtcNow,
                    LastLogin = DateTimeOffset.UtcNow,
                    IsOnline = true,
                });
        }
    }
}
