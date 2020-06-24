// -----------------------------------------------------------------
// <copyright file="Location.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Structs
{
    using System;
    using Fibula.Common.Contracts.Constants;
    using Fibula.Common.Contracts.Enumerations;

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
        /// Gets this location's parent cylinder type.
        /// </summary>
        public LocationType Type
        {
            get
            {
                if (this == default)
                {
                    return LocationType.NotSet;
                }

                if (this.X != LocationConstants.NonMapLocationX)
                {
                    return LocationType.Map;
                }

                if ((this.Y & LocationConstants.ContainerFlag) > 0)
                {
                    return LocationType.InsideContainer;
                }

                return LocationType.InventorySlot;
            }
        }

        /// <summary>
        /// Gets the slot id encoded in this location.
        /// </summary>
        public Slot Slot
        {
            get
            {
                var byteValue = (byte)this.Y;

                if (Enum.IsDefined(typeof(Slot), byteValue))
                {
                    return (Slot)byteValue;
                }

                return Slot.UnsetInvalid;
            }
        }

        /// <summary>
        /// Gets the id of the container that's encoded in this location.
        /// </summary>
        public byte ContainerId
        {
            get
            {
                if (this.Type == LocationType.InsideContainer)
                {
                    return (byte)(this.Y - LocationConstants.ContainerFlag);
                }

                return byte.MaxValue;
            }
        }

        /// <summary>
        /// Gets or sets the index of the container that's encoded in this location.
        /// </summary>
        public byte ContainerIndex
        {
            get
            {
                return (byte)this.Z;
            }

            set
            {
                this.Z = (sbyte)value;
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
            return ((long)origin.X - target.X, (long)origin.Y - target.Y, origin.Z - target.Z);
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
            if (obj is Location other)
            {
                return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z);
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
                if (Math.Abs(locationDiff.X) < Math.Abs(locationDiff.Y))
                {
                    return locationDiff.Y < 0 ? Direction.North : Direction.South;
                }

                return locationDiff.X < 0 ? Direction.West : Direction.East;
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

        /// <summary>
        /// Computes the <see cref="Location"/> that is in the <paramref name="targetDirection"/>, as seen from this location.
        /// </summary>
        /// <param name="targetDirection">The target direction.</param>
        /// <returns>The <see cref="Location"/> which is in the <paramref name="targetDirection"/> with respect to this location.</returns>
        public Location LocationAt(Direction targetDirection)
        {
            var toLoc = this;

            switch (targetDirection)
            {
                case Direction.North:
                    toLoc.Y -= 1;
                    break;
                case Direction.South:
                    toLoc.Y += 1;
                    break;
                case Direction.East:
                    toLoc.X += 1;
                    break;
                case Direction.West:
                    toLoc.X -= 1;
                    break;
                case Direction.NorthEast:
                    toLoc.X += 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLoc.X -= 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLoc.X += 1;
                    toLoc.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLoc.X -= 1;
                    toLoc.Y += 1;
                    break;
            }

            return toLoc;
        }
    }
}