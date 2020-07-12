// -----------------------------------------------------------------
// <copyright file="CipItemAttribute.cs" company="2Dudes">
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
    /// Enumerates the different attributes of an item in the CIP files.
    /// </summary>
    /// <remarks>These will all be revisited and possibly change a lot.</remarks>
    public enum CipItemAttribute : byte
    {
        /// <summary>
        /// Effect to display on absolute teleport.
        /// </summary>
        AbsTeleportEffect,

        /// <summary>
        /// The attack value for ammunition.
        /// </summary>
        AmmoAttackValue,

        /// <summary>
        /// The strength of any <see cref="AmmoSpecialEffect"/> specified.
        /// </summary>
        AmmoEffectStrength,

        /// <summary>
        /// The type of missile.
        /// </summary>
        AmmoMissile,

        /// <summary>
        /// Indicates that this ammunition produces a special effect.
        /// </summary>
        AmmoSpecialEffect,

        /// <summary>
        /// The type of ammunition.
        /// </summary>
        AmmoType,

        /// <summary>
        /// Indicates that this item is cumulative, and if so, it's current amount.
        /// </summary>
        Amount,

        /// <summary>
        /// The armor value of an item.
        /// </summary>
        ArmorValue,

        /// <summary>
        /// Indicates that stepping on this item should be avoided for this specific damage types.
        /// </summary>
        AvoidDamageTypes,

        /// <summary>
        /// Inidicates that this item is dessable and where it should be dressed.
        /// </summary>
        BodyPosition,

        /// <summary>
        /// Inidicates that this item can be used to shoot projectiles, and if so, what type of projectiles it supports.
        /// </summary>
        BowAmmoType,

        /// <summary>
        /// Inidicates that this item can be used to shoot projectiles, and if so, how far can it be used from.
        /// </summary>
        BowRange,

        /// <summary>
        /// Inidicates the level of irradiated light from this item.
        /// </summary>
        Brightness,

        /// <summary>
        /// Inidicates the capacity that this item can hold.
        /// </summary>
        Capacity,

        /// <summary>
        /// Indicates that this item can be changed when used, and if so, to what other item.
        /// </summary>
        ChangeTarget,

        /// <summary>
        /// The type of liquids that this container supports.
        /// </summary>
        ContainerLiquidType,

        /// <summary>
        /// The type of corpse that this item is.
        /// </summary>
        CorpseType,

        /// <summary>
        /// The percentual value of damage reduced by an item.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// The target id to transform after this item is destroyed.
        /// </summary>
        DestroyTarget,

        /// <summary>
        /// The id of the item that this item disguises as.
        /// </summary>
        DisguiseTarget,

        /// <summary>
        /// The elevation that this item adds.
        /// </summary>
        Elevation,

        /// <summary>
        /// The id of the item that this item expires to.
        /// </summary>
        ExpireTarget,

        /// <summary>
        /// The size of font that this item has, which directly correlates with how far someone can read it from.
        /// </summary>
        FontSize,

        /// <summary>
        /// An extra type of information that this item provides.
        /// </summary>
        InformationType,

        /// <summary>
        /// The id of the item to change into once the correct key is used on this door.
        /// </summary>
        KeydoorTarget,

        /// <summary>
        /// The id of the item to change into once this gate is used.
        /// </summary>
        LeveldoorTarget,

        /// <summary>
        /// The color of the light irradiated by this item.
        /// </summary>
        LightColor,

        /// <summary>
        /// The maximum length that this item support when accepting text.
        /// </summary>
        MaxLength,

        /// <summary>
        /// The maximum length that this item support when accepting text once.
        /// </summary>
        MaxLengthOnce,

        /// <summary>
        /// A special meaning value.
        /// </summary>
        Meaning,

        /// <summary>
        /// The minimum level that a player must be to wield this item.
        /// </summary>
        MinimumLevel,

        /// <summary>
        /// The id to change to in case the user uses and is named on this door.
        /// </summary>
        NamedoorTarget,

        /// <summary>
        /// The nutrition that this item provides.
        /// </summary>
        Nutrition,

        /// <summary>
        /// The type of liquid that this pool is of.
        /// </summary>
        PoolLiquidType,

        /// <summary>
        /// The professions that can wield this item.
        /// </summary>
        Professions,

        /// <summary>
        /// The type of damages that this item provides protection from.
        /// </summary>
        ProtectionDamageTypes,

        /// <summary>
        /// The id to change to in case the user successfully uses this quest door.
        /// </summary>
        QuestdoorTarget,

        /// <summary>
        /// The value to teleport by relatively.
        /// </summary>
        RelTeleportDisplacement,

        /// <summary>
        /// Effect to display on absolute teleport.
        /// </summary>
        RelTeleportEffect,

        /// <summary>
        /// The id of the item to rotate this item to.
        /// </summary>
        RotateTarget,

        /// <summary>
        /// The time left for item expiration.
        /// </summary>
        SavedExpireTime,

        /// <summary>
        /// The shield value that this item provides.
        /// </summary>
        ShieldDefendValue,

        /// <summary>
        /// The value by which this item modifies a certain skill by.
        /// </summary>
        SkillModification,

        /// <summary>
        /// The skill that this item modifies.
        /// </summary>
        SkillNumber,

        /// <summary>
        /// The type of liquid that this item is a source for.
        /// </summary>
        SourceLiquidType,

        /// <summary>
        /// The text that this item holds.
        /// </summary>
        String,

        /// <summary>
        /// The attack value of this item when thrown.
        /// </summary>
        ThrowAttackValue,

        /// <summary>
        /// The defense value of this item when thrown.
        /// </summary>
        ThrowDefendValue,

        /// <summary>
        /// The strength of the effect that this item produces when thrown.
        /// </summary>
        ThrowEffectStrength,

        /// <summary>
        /// The percentual chance that this item breaks when thrown.
        /// </summary>
        ThrowFragility,

        /// <summary>
        /// The missile effect that this item produces when thrown.
        /// </summary>
        ThrowMissile,

        /// <summary>
        /// The range within which this item can ben thrown.
        /// </summary>
        ThrowRange,

        /// <summary>
        /// The effect produced when (and where) this item impacts.
        /// </summary>
        ThrowSpecialEffect,

        /// <summary>
        /// The total (starting) expiration time of this item.
        /// </summary>
        TotalExpireTime,

        /// <summary>
        /// The maximum uses that this item has.
        /// </summary>
        TotalUses,

        /// <summary>
        /// The attack strength for wand items.
        /// </summary>
        WandAttackStrength,

        /// <summary>
        /// The attack damage variation for wand items.
        /// </summary>
        WandAttackVariation,

        /// <summary>
        /// The damage type for wand items.
        /// </summary>
        WandDamageType,

        /// <summary>
        /// The mana required to use a wand item.
        /// </summary>
        WandManaConsumption,

        /// <summary>
        /// The missile effect of a wand item.
        /// </summary>
        WandMissile,

        /// <summary>
        /// The range of the wand item.
        /// </summary>
        WandRange,

        /// <summary>
        /// The speed at which the ground item allows to be walked in.
        /// </summary>
        Waypoints,

        /// <summary>
        /// The attack value of a weapon.
        /// </summary>
        WeaponAttackValue,

        /// <summary>
        /// The defense value of a weapon.
        /// </summary>
        WeaponDefendValue,

        /// <summary>
        /// The type of a weapon.
        /// </summary>
        WeaponType,

        /// <summary>
        /// The id of the item into which this item wears out to, after it's maximum uses.
        /// </summary>
        WearoutTarget,

        /// <summary>
        /// The weight of an item.
        /// </summary>
        Weight,
    }
}
