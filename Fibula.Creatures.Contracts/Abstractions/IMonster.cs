// -----------------------------------------------------------------
// <copyright file="IMonster.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all monsters.
    /// </summary>
    public interface IMonster : ICreature
    {
        /// <summary>
        /// The default attack range for melee figthing in monsters.
        /// </summary>
        const int DefaultMeleeFightingAttackRange = 1;

        /// <summary>
        /// The default attack range for distance figthing in monsters.
        /// </summary>
        const int DefaultDistanceFightingAttackRange = 5;

        /// <summary>
        /// Gets the type of this monster.
        /// </summary>
        IMonsterType Type { get; }

        /// <summary>
        /// Gets the experience yielded when this monster dies.
        /// </summary>
        uint Experience { get; }
    }
}
