// -----------------------------------------------------------------
// <copyright file="AccountRepository.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class that represents an accounts repository.
    /// </summary>
    public class AccountRepository : GenericRepository<AccountEntity>, IRepository<IAccountEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="context">The context to initialize the repository with.</param>
        public AccountRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Adds an account to the set in the context.
        /// </summary>
        /// <param name="entity">The account to add.</param>
        public void Add(IAccountEntity entity)
        {
            if (!(entity is AccountEntity accountEntity))
            {
                throw new ArgumentException($"The {nameof(entity)} must be of type {nameof(AccountEntity)}.", nameof(entity));
            }

            base.Add((AccountEntity)entity);
        }

        /// <summary>
        /// Adds a range of accounts to the set in the context.
        /// </summary>
        /// <param name="entities">The account range to add.</param>
        public void AddRange(IEnumerable<IAccountEntity> entities)
        {
            base.AddRange(entities.Cast<AccountEntity>());
        }

        /// <summary>
        /// Finds all the entities in the set within the context that satisfy an expression.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<IAccountEntity> FindMany(Expression<Func<IAccountEntity, bool>> predicate)
        {
            return this.FindMany(Expression.Lambda<Func<AccountEntity, bool>>(predicate.Body, predicate.Parameters));
        }

        /// <summary>
        /// Finds an account in the set within the context that satisfies an expression.
        /// If more than one account satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The account found.</returns>
        public IAccountEntity FindOne(Expression<Func<IAccountEntity, bool>> predicate)
        {
            return this.FindOne(Expression.Lambda<Func<AccountEntity, bool>>(predicate.Body, predicate.Parameters));
        }

        /// <summary>
        /// Removes an entity from the set in the context.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(IAccountEntity entity)
        {
            base.Remove((AccountEntity)entity);
        }

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        public void RemoveRange(IEnumerable<IAccountEntity> entities)
        {
            base.RemoveRange(entities.Cast<AccountEntity>());
        }

        /// <summary>
        /// Gets all the entities from the set in the context.
        /// </summary>
        /// <returns>The collection of entities retrieved.</returns>
        IEnumerable<IAccountEntity> IReadOnlyRepository<IAccountEntity>.GetAll()
        {
            return this.GetAll();
        }

        /// <summary>
        /// Gets an entity that matches an id, from the context.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The entity found, if any.</returns>
        IAccountEntity IReadOnlyRepository<IAccountEntity>.GetById(string id)
        {
            return this.GetById(id);
        }
    }
}
