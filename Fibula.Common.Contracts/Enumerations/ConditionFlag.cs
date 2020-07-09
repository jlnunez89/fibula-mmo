// -----------------------------------------------------------------
// <copyright file="ConditionFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the condition flags that a creature can suffer from.
    /// </summary>
    public enum ConditionFlag : uint
    {
        /// <summary>
        /// No condition set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Poisoned
        /// </summary>
        Posion = 1 << 0,

        /// <summary>
        /// Burning.
        /// </summary>
        Burning = 1 << 1,

        /// <summary>
        /// Electrified.
        /// </summary>
        Electrified = 1 << 2,

        /// <summary>
        /// Hitpoints being drained.
        /// </summary>
        HitpointDraining = 1 << 3,

        /// <summary>
        /// Movement speed increased.
        /// </summary>
        Hasted = 1 << 4,

        /// <summary>
        /// Movement speed decreased.
        /// </summary>
        Paralyzed = 1 << 5,

        /// <summary>
        /// The outfit is temporarily changed.
        /// </summary>
        Outfit = 1 << 6,

        /// <summary>
        /// Invisibility.
        /// </summary>
        Invisible = 1 << 7,

        /// <summary>
        /// Irradiating light.
        /// </summary>
        Light = 1 << 8,

        /// <summary>
        /// Redirecting hitpoint damage to mana pool.
        /// </summary>
        ManaShield = 1 << 9,

        /// <summary>
        /// In combat, voluntarily or not.
        /// </summary>
        InFight = 1 << 10,

        /// <summary>
        /// Locked out of protecion zones.
        /// </summary>
        PzLocked = 1 << 11,

        /// <summary>
        /// Drunk.
        /// </summary>
        Drunk = 1 << 12,

        /// <summary>
        /// Muted from public chat channels, except for casting spells or commands.
        /// </summary>
        Muted = 1 << 13,

        /// <summary>
        /// Silenced from chatting in all channels and casting instant spells, but able to cast commands.
        /// </summary>
        Silenced = 1 << 14,
    }
}
