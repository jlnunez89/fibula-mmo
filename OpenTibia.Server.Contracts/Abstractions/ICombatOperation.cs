// -----------------------------------------------------------------
// <copyright file="ICombatOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for a combat operation.
    /// </summary>
    public interface ICombatOperation : IOperation
    {
        /// <summary>
        /// The default combat rount time in milliseconds.
        /// </summary>
        const int DefaultCombatRoundTimeInMs = 2000;

        /// <summary>
        /// Gets the combat operation's attack type.
        /// </summary>
        AttackType AttackType { get; }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        ICombatant Attacker { get; }

        /// <summary>
        /// Gets the combatant that is the target on this operation.
        /// </summary>
        ICombatant Target { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        int MinimumDamage { get; }

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        int MaximumDamage { get; }
    }
}
