// -----------------------------------------------------------------
// <copyright file="MonsterTypeReadOnlyRepository.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a read-only repository for monster types.
    /// </summary>
    public class MonsterTypeReadOnlyRepository : IReadOnlyRepository<IMonsterTypeEntity>
    {
        /// <summary>
        /// Stores the map between the monster race ids and the actual monster types.
        /// </summary>
        private readonly IDictionary<ushort, IMonsterTypeEntity> monsterTypeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterTypeReadOnlyRepository"/> class.
        /// </summary>
        /// <param name="monsterTypeLoader">A reference to the monster type loader in use.</param>
        public MonsterTypeReadOnlyRepository(IMonsterTypeLoader monsterTypeLoader)
        {
            monsterTypeLoader.ThrowIfNull(nameof(monsterTypeLoader));

            this.monsterTypeCatalog = monsterTypeLoader.LoadTypes();
        }

        /// <summary>
        /// Finds all the entities in the set within the context that satisfy an expression.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<IMonsterTypeEntity> FindMany(Expression<Func<IMonsterTypeEntity, bool>> predicate)
        {
            return this.monsterTypeCatalog.Values.AsQueryable().Where(predicate);
        }

        /// <summary>
        /// Finds an entity in the set within the context that satisfies an expression.
        /// If more than one entity satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The entity found.</returns>
        public IMonsterTypeEntity FindOne(Expression<Func<IMonsterTypeEntity, bool>> predicate)
        {
            return this.monsterTypeCatalog.Values.AsQueryable().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Gets all the entities from the set in the context.
        /// </summary>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<IMonsterTypeEntity> GetAll()
        {
            return this.monsterTypeCatalog.Values;
        }

        /// <summary>
        /// Gets an entity that matches an id, from the context.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The entity found, if any.</returns>
        public IMonsterTypeEntity GetById(string id)
        {
            if (ushort.TryParse(id, out ushort raceId) && this.monsterTypeCatalog.ContainsKey(raceId))
            {
                return this.monsterTypeCatalog[raceId];
            }

            return null;
        }
    }
}
