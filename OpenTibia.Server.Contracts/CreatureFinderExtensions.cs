// -----------------------------------------------------------------
// <copyright file="CreatureFinderExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Helper class that provides extensions for the <see cref="ICreatureFinder"/> implementations.
    /// </summary>
    public static class CreatureFinderExtensions
    {
        /// <summary>
        /// Gets the ids of any creatures that can see the given locations.
        /// </summary>
        /// <param name="creatureFinder">The reference to the creature finder.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="locations">The locations to check if players can see.</param>
        /// <returns>A collection of connections.</returns>
        public static IEnumerable<ICreature> CreaturesThatCanSee(this ICreatureFinder creatureFinder, ITileAccessor tileAccessor, params Location[] locations)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            var creatureIds = new List<uint>();

            foreach (var location in locations)
            {
                var xWindowStart = location.X - (IMap.DefaultWindowSizeX / 2);
                var xWindowEnd = location.X + (IMap.DefaultWindowSizeX / 2);
                var yWindowStart = location.Y - (IMap.DefaultWindowSizeY / 2);
                var yWindowEnd = location.Y + (IMap.DefaultWindowSizeY / 2);

                for (int x = xWindowStart; x <= xWindowEnd; x++)
                {
                    for (int y = yWindowStart; y <= yWindowEnd; y++)
                    {
                        var loc = new Location() { X = x, Y = y, Z = location.Z };

                        if (!tileAccessor.GetTileAt(loc, out ITile tile, loadAsNeeded: false))
                        {
                            continue;
                        }

                        creatureIds.AddRange(tile.CreatureIds);
                    }
                }
            }

            foreach (var creatureId in creatureIds)
            {
                var creature = creatureFinder.FindCreatureById(creatureId);

                if (creature == null || !locations.Any(loc => creature.CanSee(loc)))
                {
                    continue;
                }

                yield return creature;
            }
        }
    }
}
