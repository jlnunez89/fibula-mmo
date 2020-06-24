// -----------------------------------------------------------------
// <copyright file="DirectionExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Extensions
{
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Static class that contains extension methods for a <see cref="Direction"/>.
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Converts given direction into one of the four directions safe to send to the Tibia client.
        /// </summary>
        /// <param name="direction">The direction to convert.</param>
        /// <returns>The client-safe direction.</returns>
        public static Direction GetClientSafeDirection(this Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.South:
                    return Direction.South;

                case Direction.North:
                    return Direction.North;

                case Direction.East:
                case Direction.NorthEast:
                case Direction.SouthEast:
                    return Direction.East;

                case Direction.West:
                case Direction.NorthWest:
                case Direction.SouthWest:
                    return Direction.West;
            }
        }

        /// <summary>
        /// Determines if this direction is a diagonal one.
        /// </summary>
        /// <param name="direction">The direction to evaluate.</param>
        /// <returns>True if the direction is diagonal, false otherwise.</returns>
        public static bool IsDiagonal(this Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.South:
                case Direction.North:
                case Direction.East:
                case Direction.West:
                    return false;

                case Direction.NorthEast:
                case Direction.SouthEast:
                case Direction.NorthWest:
                case Direction.SouthWest:
                    return true;
            }
        }
    }
}
