// -----------------------------------------------------------------
// <copyright file="GenericRepository.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Fibula.Common.Utilities;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class that represents a repository for any entity in the context.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class GenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context to use with this repository.</param>
        public GenericRepository(DbContext context)
        {
            context.ThrowIfNull(nameof(context));

            this.Context = context;
        }

        /// <summary>
        /// Gets the reference to the context.
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// Gets an entity that matches an id, from the context.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The entity found, if any.</returns>
        public TEntity GetById(string id)
        {
            return this.Context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Gets all the entities from the set in the context.
        /// </summary>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<TEntity> GetAll()
        {
            var retrievalTask = this.Context.Set<TEntity>().ToListAsync();

            retrievalTask.Wait();

            return retrievalTask.Result;
        }

        /// <summary>
        /// Finds all the entities in the set within the context that satisfy an expression.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<TEntity> FindMany(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Context.Set<TEntity>().Where(predicate);
        }

        /// <summary>
        /// Finds an entity in the set within the context that satisfies an expression.
        /// If more than one entity satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The entity found.</returns>
        public TEntity FindOne(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Adds an entity to the set in the context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            this.Context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Adds a range of entities to the set in the context.
        /// </summary>
        /// <param name="entities">The entity range to add.</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            this.Context.Set<TEntity>().AddRange(entities);
        }

        /// <summary>
        /// Removes an entity from the set in the context.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(TEntity entity)
        {
            this.Context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Removes a range of entities from the set in the context.
        /// </summary>
        /// <param name="entities">The entity range to remove.</param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.Context.Set<TEntity>().RemoveRange(entities);
        }
    }
}
