// -----------------------------------------------------------------
// <copyright file="Location.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Structs
{
    using System;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Structure for all locations in the game.
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// Gets or sets the value of this location in the X coordinate.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the value of this location in the Y coordinate.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the value of this location in the Z coordinate.
        /// </summary>
        public sbyte Z { get; set; }

        /// <summary>
        /// Gets a value indicating whether this location is underground.
        /// </summary>
        public bool IsUnderground => this.Z > 7;

        /// <summary>
        /// Gets this location's type.
        /// </summary>
        public LocationType Type
        {
            get
            {
                if (this.X != 0xFFFF)
                {
                    return LocationType.Ground;
                }

                if ((this.Y & 0x40) != 0)
                {
                    return LocationType.Container;
                }

                return LocationType.Slot;
            }
        }

        /// <summary>
        /// Gets the <see cref="Slot"/> value of this location.
        /// </summary>
        public Slot Slot => (Slot)Convert.ToByte(this.Y);

        /// <summary>
        /// Gets the container value of this location.
        /// </summary>
        public byte Container => Convert.ToByte(this.Y - 0x40);

        /// <summary>
        /// Gets or sets the container index of this location.
        /// </summary>
        public sbyte ContainerPosition
        {
            get
            {
                return Convert.ToSByte(this.Z);
            }

            set
            {
                this.Z = value;
            }
        }

        /// <summary>
        /// Gets the maximum value from X or Y in this location's coordinates.
        /// </summary>
        public int MaxValueIn2D => Math.Max(Math.Abs(this.X), Math.Abs(this.Y));

        /// <summary>
        /// Gets the maximum value from X, Y, or Z in this location's coordinates.
        /// </summary>
        public int MaxValueIn3D => Math.Max(this.MaxValueIn2D, Math.Abs(this.Z));

        /// <summary>
        /// Adds two <see cref="Location"/> values.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>The result of the operation.</returns>
        public static Location operator +(Location location1, Location location2)
        {
            return new Location
            {
                X = location1.X + location2.X,
                Y = location1.Y + location2.Y,
                Z = (sbyte)(location1.Z + location2.Z),
            };
        }

        /// <summary>
        /// Subtracts two <see cref="Location"/> values.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>The result of the operation.</returns>
        public static Location operator -(Location location1, Location location2)
        {
            return new Location
            {
                X = location2.X - location1.X,
                Y = location2.Y - location1.Y,
                Z = (sbyte)(location2.Z - location1.Z),
            };
        }

        /// <summary>
        /// Checks if two <see cref="Location"/> values are considered equal.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public static bool operator ==(Location location1, Location location2) => location1.X == location2.X && location1.Y == location2.Y && location1.Z == location2.Z;

        /// <summary>
        /// Checks if two <see cref="Location"/> values are considered equal.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>True if they are not equal, false otherwise.</returns>
        public static bool operator !=(Location location1, Location location2) => location1.X != location2.X || location1.Y != location2.Y || location1.Z != location2.Z;

        /// <summary>
        /// Checks if a <see cref="Location"/> is considered greater than <see cref="Location"/>.
        /// A <see cref="Location"/> is considered 'greater' than another when any of it's coordinates is greater than those of the other <see cref="Location"/>.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>True if <paramref name="location1"/> is considered greater than <paramref name="location2"/>.</returns>
        public static bool operator >(Location location1, Location location2) => location1.X > location2.X || location1.Y > location2.Y || location1.Z > location2.Z;

        /// <summary>
        /// Checks if a <see cref="Location"/> is considered lesser than <see cref="Location"/>.
        /// A <see cref="Location"/> is considered 'lesser' than another when any of it's coordinates is less than those of the other <see cref="Location"/>.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>True if <paramref name="location1"/> is considered lesser than <paramref name="location2"/>.</returns>
        public static bool operator <(Location location1, Location location2) => location1.X < location2.X || location1.Y < location2.Y || location1.Z < location2.Z;

        /// <summary>
        /// Computes the offset from the <paramref name="origin"/> to the <paramref name="target"/>.
        /// </summary>
        /// <param name="origin">The origin location.</param>
        /// <param name="target">The target location.</param>
        /// <returns>An ordered set, containing the offset bewteen locations.</returns>
        public static (long XOffset, long YOffset, int ZOffset) GetOffsetBetween(Location origin, Location target)
        {
            return ((long)origin.X - target.X, (long)origin.Y - target.Y, (int)origin.Z - target.Z);
        }

        /// <summary>
        /// Gets this location's string representation: it's coordinates.
        /// </summary>
        /// <returns>This location's string representation.</returns>
        public override string ToString()
        {
            return $"[{this.X}, {this.Y}, {this.Z}]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Location && this == (Location)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (this.X, this.Y, this.Z).GetHashCode();
        }

        /// <summary>
        /// Computes the direction in which the <paramref name="target"/> is, as seen from this location.
        /// </summary>
        /// <param name="target">The target location.</param>
        /// <param name="returnDiagonals">Optional. A value indicating whether to include diagonals as the potential answer to this query. Defaults to false.</param>
        /// <returns>The direction in which <paramref name="target"/> is with respect to this location.</returns>
        public Direction DirectionTo(Location target, bool returnDiagonals = false)
        {
            var locationDiff = this - target;

            if (!returnDiagonals)
            {
                if (locationDiff.X < 0)
                {
                    return Direction.West;
                }

                if (locationDiff.X > 0)
                {
                    return Direction.East;
                }

                return locationDiff.Y < 0 ? Direction.North : Direction.South;
            }

            if (locationDiff.X < 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthWest;
                }

                return locationDiff.Y > 0 ? Direction.SouthWest : Direction.West;
            }

            if (locationDiff.X > 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthEast;
                }

                return locationDiff.Y > 0 ? Direction.SouthEast : Direction.East;
            }

            return locationDiff.Y < 0 ? Direction.North : Direction.South;
        }
    }
}