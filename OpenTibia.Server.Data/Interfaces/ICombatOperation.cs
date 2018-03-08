// <copyright file="ICombatOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System;
    using OpenTibia.Data.Contracts;

    public interface ICombatOperation
    {
        AttackType AttackType { get; }

        ICombatActor Attacker { get; }

        ICombatActor Target { get; }

        TimeSpan ExhaustionCost { get; }

        int MinimumDamage { get; }

        int MaximumDamage { get; }

        bool CanBeExecuted { get; }

        void Execute();
    }
}