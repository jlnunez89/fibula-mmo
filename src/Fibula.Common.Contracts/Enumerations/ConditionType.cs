// -----------------------------------------------------------------
// <copyright file="ConditionType.cs" company="2Dudes">
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
    /// Enumerates the condition flags that a thing can suffer from.
    /// </summary>
    public enum ConditionType : uint
    {
        /// <summary>
        /// In combat, voluntarily or not.
        /// </summary>
        InFight,

        /// <summary>
        /// Locked out of protecion zones.
        /// </summary>
        PzLocked,

        /// <summary>
        /// A decaying thing.
        /// </summary>
        Decaying,

        /// <summary>
        /// Loosing hitpoints over time.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Loosing manapoints over time.
        /// </summary>
        ManaDraining,

        /// <summary>
        /// Poisoned.
        /// </summary>
        Posioned,

        /// <summary>
        /// On fire (the unpleasant way).
        /// </summary>
        Burning,

        /// <summary>
        /// Shocked (the bad way).
        /// </summary>
        Electrified,

        /// <summary>
        /// Silenced from chatting in all channels and casting instant spells, but able to cast commands.
        /// </summary>
        Silenced,

        /// <summary>
        /// Movement restricted.
        /// </summary>
        Paralyzed,

        /// <summary>
        /// The outfit is temporarily changed.
        /// </summary>
        Outfit,

        /// <summary>
        /// Invisibility.
        /// </summary>
        Invisible,

        /// <summary>
        /// Irradiating light.
        /// </summary>
        Light,

        /// <summary>
        /// Redirecting hitpoint damage to mana pool.
        /// </summary>
        ManaShield,

        /// <summary>
        /// Movement speed increased/decreased.
        /// </summary>
        ChangedSpeed,

        /// <summary>
        /// Skills are altered.
        /// </summary>
        ChangedSkill,

        /// <summary>
        /// Had too much wine.
        /// </summary>
        Drunk,

        /// <summary>
        /// Muted from public chat channels, except for casting spells or commands.
        /// </summary>
        Muted,

        /// <summary>
        /// Regenerating one or more stats.
        /// </summary>
        Regenerating,

        /// <summary>
        /// Exhausted to use items or otherwise perform actions.
        /// </summary>
        ExhaustedAction,

        /// <summary>
        /// Exhausted to fight.
        /// </summary>
        ExhaustedCombat,

        /// <summary>
        /// Exhausted to cast spells or using runes.
        /// </summary>
        ExhaustedMagic,

        /// <summary>
        /// Exhausted to move.
        /// </summary>
        ExhaustedMovement,
    }
}
