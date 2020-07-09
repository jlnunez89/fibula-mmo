// -----------------------------------------------------------------
// <copyright file="ItemConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Constants
{
    /// <summary>
    /// Static class that contains constants regarding items.
    /// </summary>
    public static class ItemConstants
    {
        /// <summary>
        /// The maximum amount value of cummulative items.
        /// </summary>
        public const byte MaximumAmountOfCummulativeItems = 100;

        /// <summary>
        /// The position to use for unset or new containers.
        /// </summary>
        public const byte UnsetContainerPosition = byte.MaxValue;

        /// <summary>
        /// The maximum number of containers to maintain per creature.
        /// </summary>
        public const int MaxContainersPerCreature = 16;
    }
}
