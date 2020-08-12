// -----------------------------------------------------------------
// <copyright file="MonsterConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Constants
{
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Static class that contains contants for <see cref="IMonster"/> derived clases.
    /// </summary>
    public static class MonsterConstants
    {
        /// <summary>
        /// The default attack range for melee figthing in monsters.
        /// </summary>
        public const int DefaultMeleeFightingAttackRange = 1;

        /// <summary>
        /// The default attack range for distance figthing in monsters.
        /// </summary>
        public const int DefaultDistanceFightingAttackRange = 4;

        /// <summary>
        /// The factor to use to calculate drop chance for monster loot.
        /// </summary>
        public const int DropChanceFactor = 1000;

        /// <summary>
        /// The default maximum capacity for the monster inventory.
        /// </summary>
        public const int DefaultMaximumCapacity = 20;

        /// <summary>
        /// The default drop probability for an item in the monster inventory.
        /// </summary>
        public const int DefaultLossProbability = 1000;
    }
}
