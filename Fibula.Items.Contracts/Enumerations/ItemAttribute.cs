// -----------------------------------------------------------------
// <copyright file="ItemAttribute.cs" company="2Dudes">
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
    /// Enumerates the different attributes of an <see cref="IItemType"/>.
    /// </summary>
    public enum ItemAttribute : byte
    {
        Capacity,
        Weight,
        Waypoints,
        AvoidDamageTypes,
        ExpireTarget,
        TotalExpireTime,
        SourceLiquidType,
        DisguiseTarget,
        Brightness,
        LightColor,
        Elevation,
        KeydoorTarget,
        ChangeTarget,
        NamedoorTarget,
        FontSize,
        QuestdoorTarget,
        LeveldoorTarget,
        ThrowRange,
        ThrowAttackValue,
        ThrowDefendValue,
        ThrowMissile,
        ThrowSpecialEffect,
        ThrowEffectStrength,
        ThrowFragility,
        RotateTarget,
        DestroyTarget,
        Meaning,
        InformationType,
        MaxLength,
        MaxLengthOnce,
        BodyPosition,
        ShieldDefendValue,
        ProtectionDamageTypes,
        DamageReduction,
        WearoutTarget,
        TotalUses,
        ArmorValue,
        MinimumLevel,
        Professions,
        WandRange,
        WandManaConsumption,
        WandAttackStrength,
        WandAttackVariation,
        WandDamageType,
        WandMissile,
        SkillNumber,
        SkillModification,
        WeaponType,
        WeaponAttackValue,
        WeaponDefendValue,
        Nutrition,
        BowRange,
        BowAmmoType,
        AmmoType,
        AmmoAttackValue,
        AmmoMissile,
        AmmoSpecialEffect,
        AmmoEffectStrength,
        CorpseType,
        AbsTeleportEffect,
        RelTeleportEffect,
        RelTeleportDisplacement,
        Amount,
        ContainerLiquidType,
        PoolLiquidType,
        String,
        SavedExpireTime,
    }
}
