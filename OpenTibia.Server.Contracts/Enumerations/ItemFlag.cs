// -----------------------------------------------------------------
// <copyright file="ItemFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Enumerates all known <see cref="IItemType"/> flags.
    /// </summary>
    public enum ItemFlag : byte
    {
        Container,
        Take,
        Unpass,
        Bank,
        Unmove,
        Unthrow,
        Unlay,
        CollisionEvent,
        Avoid,
        Expire,
        LiquidSource,
        SeparationEvent,
        Disguise,
        UseEvent,
        ForceUse,
        RopeSpot,
        Bottom,
        Light,
        Height,
        Clip,
        Top,
        HookEast,
        HookSouth,
        KeyDoor,
        ChangeUse,
        NameDoor,
        Text,
        QuestDoor,
        LevelDoor,
        Cumulative,
        Throw,
        Hang,
        Rotate,
        Destroy,
        MagicField,
        Special,
        Information,
        Chest,
        Bed,
        MultiUse,
        LiquidContainer,
        Write,
        WriteOnce,
        Clothes,
        LiquidPool,
        ExpireStop,
        MovementEvent,
        Key,
        Shield,
        Protection,
        WearOut,
        ShowDetail,
        Armor,
        RestrictLevel,
        RestrictProfession,
        Wand,
        SkillBoost,
        DistUse,
        Rune,
        Weapon,
        Food,
        Bow,
        Ammo,
        Corpse,
        TeleportAbsolute,
        TeleportRelative,
    }
}
