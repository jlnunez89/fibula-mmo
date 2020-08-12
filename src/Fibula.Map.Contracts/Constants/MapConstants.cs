// -----------------------------------------------------------------
// <copyright file="MapConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Constants
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Static class that contains constants regarding the map.
    /// </summary>
    public static class MapConstants
    {
        /// <summary>
        /// The maximum number of <see cref="IThing"/>s to describe per tile.
        /// </summary>
        public const int MaximumNumberOfThingsToDescribePerTile = 9;

        /// <summary>
        /// The default window size in the X coordinate.
        /// </summary>
        public const byte DefaultWindowSizeX = 18;

        /// <summary>
        /// The default window size in the Y coordinate.
        /// </summary>
        public const byte DefaultWindowSizeY = 14;

        /// <summary>
        /// The mark for the temple in Rookgaard.
        /// </summary>
        public static readonly Location RookgaardTempleMark = new Location { X = 32097, Y = 32219, Z = 7 };

        /// <summary>
        /// The mark for the temple in Thais.
        /// </summary>
        public static readonly Location ThaisTempleMark = new Location { X = 32369, Y = 32241, Z = 7 };

        /// <summary>
        /// The mark for Fibula downdown.
        /// </summary>
        public static readonly Location FibulaMark = new Location { X = 32176, Y = 32437, Z = 7 };

        /// <summary>
        /// The mark for Elvenbane.
        /// </summary>
        public static readonly Location ElvenbaneMark = new Location { X = 32590, Y = 31657, Z = 7 };
    }
}
