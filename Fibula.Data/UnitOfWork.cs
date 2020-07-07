// -----------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data
{
    using Fibula.Common.Utilities;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities;
    using Fibula.Data.Repositories;
    using Microsoft.EntityFrameworkCore;

    using IUnitOfWork = Fibula.Data.Contracts.Abstractions.IUnitOfWork<
        Fibula.Data.Entities.AccountEntity,
        Fibula.Data.Entities.CharacterEntity>;

    /// <summary>
    /// Class that represents a unit of work for the Fibula project.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// A reference to the underlying context on this unit of work.
        /// </summary>
        private readonly DbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context to work on.</param>
        public UnitOfWork(IFibulaDbContext context)
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
        public IAccountRepository<AccountEntity> Accounts { get; }

        /// <summary>
        /// Gets a reference to the characters repository.
        /// </summary>
        public ICharacterRepository<CharacterEntity> Characters { get; }

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