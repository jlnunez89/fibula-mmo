// <copyright file="ICombatActor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    public delegate void OnAttackTargetChange(uint oldTargetId, uint newTargetId);
    // public delegate void OnAttackPerformed(uint fromCreatureId, uint toCreatureId);
    public interface ICombatActor
    {
        event OnAttackTargetChange OnTargetChanged;

        uint ActorId { get; }

        Dictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }

        BloodType Blood { get; }

        uint FollowId { get; }

        uint AutoAttackTargetId { get; }

        byte AutoAttackRange { get; }

        byte AutoAttackCredits { get; }

        byte AutoDefenseCredits { get; }

        DateTime LastAttackTime { get; }

        TimeSpan LastAttackCost { get; }

        TimeSpan CombatCooldownTimeRemaining { get; }

        /// <summary>
        /// How fast an Actor can earn a new AutoAttack credit.
        /// </summary>
        decimal BaseAttackSpeed { get; }

        /// <summary>
        /// How fast an Actor can earn a new AutoDefense credit.
        /// </summary>
        decimal BaseDefenseSpeed { get; }

        ushort AttackPower { get; }

        ushort DefensePower { get; }

        ushort ArmorRating { get; }

        Location Location { get; }

        void SetAttackTarget(uint targetId);

        void UpdateLastAttack(TimeSpan cost);

        void CheckAutoAttack();
    }
}
