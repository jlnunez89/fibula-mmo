// -----------------------------------------------------------------
// <copyright file="CharacterRepository.cs" company="2Dudes">
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
    /// Class that represents a character repository.
    /// </summary>
    public class CharacterRepository : GenericRepository<CharacterEntity>, IRepository<ICharacterEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterRepository"/> class.
        /// </summary>
        /// <param name="context">The context to initialize the repository with.</param>
        public CharacterRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Adds an character to the set in the context.
        /// </summary>
        /// <param name="entity">The character to add.</param>
        public void Add(ICharacterEntity entity)
        {
            if (!(entity is CharacterEntity characterEntity))
            {
                throw new ArgumentException($"The {nameof(entity)} must be of type {nameof(CharacterEntity)}.", nameof(entity));
            }

            base.Add((CharacterEntity)entity);
        }

        /// <summary>
        /// Adds a range of characters to the set in the context.
        /// </summary>
        /// <param name="entities">The character range to add.</param>
        public void AddRange(IEnumerable<ICharacterEntity> entities)
        {
            base.AddRange(entities.Cast<CharacterEntity>());
        }

        /// <summary>
        /// Finds all the entities in the set within the context that satisfy an expression.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<ICharacterEntity> FindMany(Expression<Func<ICharacterEntity, bool>> predicate)
        {
            return this.FindMany(Expression.Lambda<Func<CharacterEntity, bool>>(predicate.Body, predicate.Parameters));
        }

        /// <summary>
        /// Finds an character in the set within the context that satisfies an expression.
        /// If more than one character satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The character found.</returns>
        public ICharacterEntity FindOne(Expression<Func<ICharacterEntity, bool>> predicate)
        {
            return this.FindOne(Expression.Lambda<Func<CharacterEntity, bool>>(predicate.Body, predicate.Parameters));
        }

        /// <summary>
        /// Removes an entity from the set in the context.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(ICharacterEntity entity)
        {
            base.Remove((CharacterEntity)entity);
        }

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        public void RemoveRange(IEnumerable<ICharacterEntity> entities)
        {
            base.RemoveRange(entities.Cast<CharacterEntity>());
        }

        /// <summary>
        /// Gets all the entities from the set in the context.
        /// </summary>
        /// <returns>The collection of entities retrieved.</returns>
        IEnumerable<ICharacterEntity> IReadOnlyRepository<ICharacterEntity>.GetAll()
        {
            return this.GetAll();
        }

        /// <summary>
        /// Gets an entity that matches an id, from the context.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The entity found, if any.</returns>
        ICharacterEntity IReadOnlyRepository<ICharacterEntity>.GetById(string id)
        {
            return this.GetById(id);
        }
    }
}
