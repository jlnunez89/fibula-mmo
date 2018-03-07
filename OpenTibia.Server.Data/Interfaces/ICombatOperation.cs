using System;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Data.Interfaces
{
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