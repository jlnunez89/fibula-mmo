// -----------------------------------------------------------------
// <copyright file="CipItemFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Enumerations
{
    /// <summary>
    /// Enumerates all known CIP item flags.
    /// </summary>
    public enum CipItemFlag : byte
    {
        /// <summary>
        /// The item has armor traits.
        /// </summary>
        Armor,

        /// <summary>
        /// The item is ammunition.
        /// </summary>
        Ammo,

        /// <summary>
        /// The item hints at avoiding pathfind.
        /// </summary>
        Avoid,

        /// <summary>
        /// The item has pathfinding.
        /// </summary>
        Bank,

        /// <summary>
        /// The item is a bed.
        /// </summary>
        Bed,

        /// <summary>
        /// The item stays on bottom.
        /// </summary>
        Bottom,

        /// <summary>
        /// The item shoots ammunition.
        /// </summary>
        Bow,

        /// <summary>
        /// The item can be changed on use.
        /// </summary>
        ChangeUse,

        /// <summary>
        /// The item is a chest.
        /// </summary>
        Chest,

        /// <summary>
        /// The item is clipped.
        /// </summary>
        Clip,

        /// <summary>
        /// The item can be dressed.
        /// </summary>
        Clothes,

        /// <summary>
        /// The item can trigger collision event rules.
        /// </summary>
        CollisionEvent,

        /// <summary>
        /// The item is a container.
        /// </summary>
        Container,

        /// <summary>
        /// The item is a corpse.
        /// </summary>
        Corpse,

        /// <summary>
        /// The item is cummulative.
        /// </summary>
        Cumulative,

        /// <summary>
        /// The item can be destroyed.
        /// </summary>
        Destroy,

        /// <summary>
        /// The item looks like another item.
        /// </summary>
        Disguise,

        /// <summary>
        /// The item can be used at a distance.
        /// </summary>
        DistUse,

        /// <summary>
        /// The item has time based expiration.
        /// </summary>
        Expire,

        /// <summary>
        /// The item's expiration stops on use or undressing.
        /// </summary>
        ExpireStop,

        /// <summary>
        /// The item has nutrition.
        /// </summary>
        Food,

        /// <summary>
        /// The item can be used even if there are other items blocking it.
        /// </summary>
        ForceUse,

        /// <summary>
        /// The item can be hanged on a wall with a hook.
        /// </summary>
        Hang,

        /// <summary>
        /// The item has height.
        /// </summary>
        Height,

        /// <summary>
        /// The item can hang/unhang as long as it is interacted with from the east.
        /// </summary>
        HookEast,

        /// <summary>
        /// The item can hang/unhang as long as it is interacted with from the south.
        /// </summary>
        HookSouth,

        /// <summary>
        /// The item provides extra information.
        /// </summary>
        Information,

        /// <summary>
        /// The item is a key.
        /// </summary>
        Key,

        /// <summary>
        /// The item is a door that can be locked.
        /// </summary>
        KeyDoor,

        /// <summary>
        /// The item is a door that has level requirements.
        /// </summary>
        LevelDoor,

        /// <summary>
        /// The item irradiates light.
        /// </summary>
        Light,

        /// <summary>
        /// The item is a liquid container.
        /// </summary>
        LiquidContainer,

        /// <summary>
        /// The item is a liquid pool.
        /// </summary>
        LiquidPool,

        /// <summary>
        /// The item is a liquid source.
        /// </summary>
        LiquidSource,

        /// <summary>
        /// The item is a magic field.
        /// </summary>
        MagicField,

        /// <summary>
        /// The item can trigger movement events.
        /// </summary>
        MovementEvent,

        /// <summary>
        /// The item can be used on other things.
        /// </summary>
        MultiUse,

        /// <summary>
        /// The item is a door which has a list of allowed users.
        /// </summary>
        NameDoor,

        /// <summary>
        /// The item provides protection against some type of damage.
        /// </summary>
        Protection,

        /// <summary>
        /// The item is a door that is part of a quest.
        /// </summary>
        QuestDoor,

        /// <summary>
        /// The item has a level restriction.
        /// </summary>
        RestrictLevel,

        /// <summary>
        /// The item has a profession restriction.
        /// </summary>
        RestrictProfession,

        /// <summary>
        /// The item is a spot where a rope can be used.
        /// </summary>
        RopeSpot,

        /// <summary>
        /// The item can be rotated.
        /// </summary>
        Rotate,

        /// <summary>
        /// The item is a rune.
        /// </summary>
        Rune,

        /// <summary>
        /// The item can trigger separation events.
        /// </summary>
        SeparationEvent,

        /// <summary>
        /// The item has shield traits.
        /// </summary>
        Shield,

        /// <summary>
        /// The item has extra detail, like expire time.
        /// </summary>
        ShowDetail,

        /// <summary>
        /// The item boosts skills.
        /// </summary>
        SkillBoost,

        /// <summary>
        /// The item has special traits.
        /// </summary>
        Special,

        /// <summary>
        /// The item can be taken.
        /// </summary>
        Take,

        /// <summary>
        /// The item is a teleportation device to an absolute location.
        /// </summary>
        TeleportAbsolute,

        /// <summary>
        /// The item is a teleportation device to a relative location.
        /// </summary>
        TeleportRelative,

        /// <summary>
        /// The item holds text information.
        /// </summary>
        Text,

        /// <summary>
        /// The item can be thrown (as a weapon).
        /// </summary>
        Throw,

        /// <summary>
        /// The item stays on top of others.
        /// </summary>
        Top,

        /// <summary>
        /// The item blocks laying other items on top.
        /// </summary>
        Unlay,

        /// <summary>
        /// The item cannot be moved.
        /// </summary>
        Unmove,

        /// <summary>
        /// The item blocks movement on top of it.
        /// </summary>
        Unpass,

        /// <summary>
        /// The item blocks throws through it.
        /// </summary>
        Unthrow,

        /// <summary>
        /// The item can trigger use events.
        /// </summary>
        UseEvent,

        /// <summary>
        /// The item is a wand.
        /// </summary>
        Wand,

        /// <summary>
        /// The item is a weapon.
        /// </summary>
        Weapon,

        /// <summary>
        /// The item has a limited number of uses.
        /// </summary>
        WearOut,

        /// <summary>
        /// The item can be writen on, multiple times.
        /// </summary>
        Write,

        /// <summary>
        /// The item can be writen on only once.
        /// </summary>
        WriteOnce,
    }
}
