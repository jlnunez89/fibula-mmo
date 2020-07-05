// -----------------------------------------------------------------
// <copyright file="MapExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;

    /// <summary>
    /// Helper class that provides extensions for the <see cref="IMap"/> implementations.
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// Gets the ids of any creatures that can see the given locations.
        /// </summary>
        /// <param name="map">A reference to the map.</param>
        /// <param name="locations">The locations to check if players can see.</param>
        /// <returns>A collection of connections.</returns>
        public static IEnumerable<ICreature> CreaturesThatCanSee(this IMap map, params Location[] locations)
        {
            map.ThrowIfNull(nameof(map));

            var creatures = new HashSet<ICreature>();

            foreach (var location in locations)
            {
                var xWindowStart = location.X - (MapConstants.DefaultWindowSizeX / 2);
                var xWindowEnd = location.X + (MapConstants.DefaultWindowSizeX / 2);
                var yWindowStart = location.Y - (MapConstants.DefaultWindowSizeY / 2);
                var yWindowEnd = location.Y + (MapConstants.DefaultWindowSizeY / 2);

                for (int x = xWindowStart; x <= xWindowEnd; x++)
                {
                    for (int y = yWindowStart; y <= yWindowEnd; y++)
                    {
                        var loc = new Location() { X = x, Y = y, Z = location.Z };

                        if (!map.GetTileAt(loc, out ITile tile, loadAsNeeded: false))
                        {
                            continue;
                        }

                        foreach (var creature in tile.Creatures)
                        {
                            creatures.Add(creature);
                        }
                    }
                }
            }

            foreach (var creature in creatures)
            {
                if (creature == null || !locations.Any(loc => creature.CanSee(loc)))
                {
                    continue;
                }

                yield return creature;
            }
        }

        /// <summary>
        /// Gets the ids of any players that can see the given locations.
        /// </summary>
        /// <param name="map">A reference to the map.</param>
        /// <param name="locations">The locations to check if players can see.</param>
        /// <returns>A collection of connections.</returns>
        public static IEnumerable<IPlayer> PlayersThatCanSee(this IMap map, params Location[] locations)
        {
            var creaturesThatCanSee = map.CreaturesThatCanSee(locations);

            return creaturesThatCanSee.OfType<IPlayer>();
        }
    }
}
