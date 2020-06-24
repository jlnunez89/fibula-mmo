// -----------------------------------------------------------------
// <copyright file="AccountRepository.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Repositories
{
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class that represents an accounts repository.
    /// </summary>
    public class AccountRepository : GenericRepository<AccountEntity>, IAccountRepository<AccountEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="context">The context to initialize the repository with.</param>
        public AccountRepository(DbContext context)
            : base(context)
        {
        }
    }
}
