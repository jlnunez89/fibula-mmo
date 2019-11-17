// -----------------------------------------------------------------
// <copyright file="OpenTibiaUnitOfWork.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data
{
    using Microsoft.EntityFrameworkCore;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Data.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a unit of work for the OpenTibia project.
    /// </summary>
    public class OpenTibiaUnitOfWork : IOpenTibiaUnitOfWork
    {
        /// <summary>
        /// A reference to the underlying context on this unit of work.
        /// </summary>
        private readonly DbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTibiaUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context to work on.</param>
        public OpenTibiaUnitOfWork(IOpenTibiaDbContext context)
        {
            context.ThrowIfNull(nameof(context));

            this.context = context.AsDbContext();
            this.context.Database.EnsureCreatedAsync().Wait();

            this.Accounts = new AccountRepository(this.context);
            this.Characters = new CharacterRepository(this.context);
        }

        /// <summary>
        /// Gets a reference to the accounts repository.
        /// </summary>
        public IAccountRepository Accounts { get; }

        /// <summary>
        /// Gets a reference to the characters repository.
        /// </summary>
        public ICharacterRepository Characters { get; }

        /// <summary>
        /// Completes this unit of work.
        /// </summary>
        /// <returns>The number of changes saved upon completion of this unit of work.</returns>
        public int Complete()
        {
            return this.context.SaveChanges();
        }

        /// <summary>
        /// Disposes this unit of work and it's resources.
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}