// -----------------------------------------------------------------
// <copyright file="GenericRepository.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using Fibula.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class that represents a repository for any entity in the context.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class GenericRepository<TEntity> : GenericReadOnlyRepository<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context to use with this repository.</param>
        public GenericRepository(DbContext context)
            : base(context)
        {
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
