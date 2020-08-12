// -----------------------------------------------------------------
// <copyright file="MapExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Extensions
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;

    /// <summary>
    /// Helper class for extension methods of the map.
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// Checks if a throw between two map locations is valid.
        /// </summary>
        /// <param name="map">A reference to the map.</param>
        /// <param name="fromLocation">The first location.</param>
        /// <param name="toLocation">The second location.</param>
        /// <param name="checkLineOfSight">Optional. A value indicating whether to consider line of sight.</param>
        /// <returns>True if the throw is valid, false otherwise.</returns>
        public static bool CanThrowBetweenLocations(this IMap map, Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            map.ThrowIfNull(nameof(map));

            if (fromLocation == toLocation)
            {
                return true;
            }

            if (fromLocation.Type != LocationType.Map || toLocation.Type != LocationType.Map)
            {
                return false;
            }

            // Cannot throw across the surface boundary (floor 7).
            if ((fromLocation.Z >= 8 && toLocation.Z <= 7) || (toLocation.Z >= 8 && fromLocation.Z <= 7))
            {
                return false;
            }

            var deltaX = Math.Abs(fromLocation.X - toLocation.X);
            var deltaY = Math.Abs(fromLocation.Y - toLocation.Y);
            var deltaZ = Math.Abs(fromLocation.Z - toLocation.Z);

            // distance checks
            if (deltaX - deltaZ >= (MapConstants.DefaultWindowSizeX / 2) || deltaY - deltaZ >= (MapConstants.DefaultWindowSizeY / 2))
            {
                return false;
            }

            return !checkLineOfSight || map.AreInLineOfSight(fromLocation, toLocation) || map.AreInLineOfSight(toLocation, fromLocation);
        }

        /// <summary>
        /// Checks if two map locations are line of sight.
        /// </summary>
        /// <param name="map">A reference to the map.</param>
        /// <param name="firstLocation">The first location.</param>
        /// <param name="secondLocation">The second location.</param>
        /// <returns>True if the second location is considered within the line of sight of the first location, false otherwise.</returns>
        public static bool AreInLineOfSight(this IMap map, Location firstLocation, Location secondLocation)
        {
            map.ThrowIfNull(nameof(map));

            if (firstLocation == secondLocation)
            {
                return true;
            }

            if (firstLocation.Type != LocationType.Map || secondLocation.Type != LocationType.Map)
            {
                return false;
            }

            // Normalize so that the check always happens from 'high to low' floors.
            var origin = firstLocation.Z > secondLocation.Z ? secondLocation : firstLocation;
            var target = firstLocation.Z > secondLocation.Z ? firstLocation : secondLocation;

            // Define positive or negative steps, depending on where the target location is wrt the origin location.
            var stepX = (sbyte)(origin.X < target.X ? 1 : origin.X == target.X ? 0 : -1);
            var stepY = (sbyte)(origin.Y < target.Y ? 1 : origin.Y == target.Y ? 0 : -1);

            var a = target.Y - origin.Y;
            var b = origin.X - target.X;
            var c = -((a * target.X) + (b * target.Y));

            while ((origin - target).MaxValueIn2D != 0)
            {
                var moveHorizontal = Math.Abs((a * (origin.X + stepX)) + (b * origin.Y) + c);
                var moveVertical = Math.Abs((a * origin.X) + (b * (origin.Y + stepY)) + c);
                var moveCross = Math.Abs((a * (origin.X + stepX)) + (b * (origin.Y + stepY)) + c);

                if (origin.Y != target.Y && (origin.X == target.X || moveHorizontal > moveVertical || moveHorizontal > moveCross))
                {
                    origin.Y += stepY;
                }

                if (origin.X != target.X && (origin.Y == target.Y || moveVertical > moveHorizontal || moveVertical > moveCross))
                {
                    origin.X += stepX;
                }

                if (map.GetTileAt(origin, out ITile tile) && tile.BlocksThrow)
                {
                    return false;
                }
            }

            while (origin.Z != target.Z)
            {
                // now we need to perform a jump between floors to see if everything is clear (literally)
                if (map.GetTileAt(origin, out ITile tile) && tile.Ground != null)
                {
                    return false;
                }

                origin.Z++;
            }

            return true;
        }
    }
}
