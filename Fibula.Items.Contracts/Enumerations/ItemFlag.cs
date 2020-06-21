// -----------------------------------------------------------------
// <copyright file="ItemFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Enumerations
{
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Enumerates all known <see cref="IItemType"/> flags.
    /// </summary>
    public enum ItemFlag : byte
    {
        Ammo,
        Armor,
        Avoid,
        Bank,
        Bed,
        Bottom,
        Bow,
        ChangeUse,
        Chest,
        Clip,
        Clothes,
        CollisionEvent,
        Container,
        Corpse,
        Cumulative,
        Destroy,
        Disguise,
        DistUse,
        Expire,
        ExpireStop,
        Food,
        ForceUse,
        Hang,
        Height,
        HookEast,
        HookSouth,
        Information,
        Key,
        KeyDoor,
        LevelDoor,
        Light,
        LiquidContainer,
        LiquidPool,
        LiquidSource,
        MagicField,
        MovementEvent,
        MultiUse,
        NameDoor,
        Protection,
        QuestDoor,
        RestrictLevel,
        RestrictProfession,
        RopeSpot,
        Rotate,
        Rune,
        SeparationEvent,
        Shield,
        ShowDetail,
        SkillBoost,
        Special,
        Take,
        TeleportAbsolute,
        TeleportRelative,
        Text,
        Throw,
        Top,
        Unlay,
        Unmove,
        Unpass,
        Unthrow,
        UseEvent,
        Wand,
        Weapon,
        WearOut,
        Write,
        WriteOnce,
    }
}
