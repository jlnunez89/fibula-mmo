using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Data.Interfaces
{
    public delegate void OnCreatureStateChange();

    public interface ICreature : IThing
    {
        //event OnCreatureStateChange OnZeroHealth;
        //event OnCreatureStateChange OnInventoryChanged;

        uint CreatureId { get; }
        string Article { get; }
        string Name { get; }
        ushort Corpse { get; }
        uint Hitpoints { get; }
        uint MaxHitpoints { get; }
        uint Manapoints { get; }
        uint MaxManapoints { get; }
        decimal CarryStrength { get; }
        Outfit Outfit { get; }
        Direction Direction { get; }
        Direction ClientSafeDirection { get; }
        byte LightBrightness { get; }
        byte LightColor { get; }
        ushort Speed { get; }
        uint Flags { get; }

        ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }
        byte NextStepId { get; set; }

        Dictionary<SkillType, ISkill> Skills { get; }

        /// <summary>
        /// Stores information about cooldowns for a creature where the key is a <see cref="CooldownType"/>, and the value is a <see cref="Tuple{T1, T2}"/>.
        /// The tuple elements are a <see cref="DateTime"/>, to store the time when the cooldown started, and a <see cref="TimeSpan"/> to denote how long it should last.
        /// </summary>
        Dictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }

        bool IsInvisible { get; } // TODO: implement.
        bool CanSeeInvisible { get; } // TODO: implement.

        byte Skull { get; } // TODO: implement.
        byte Shield { get; } // TODO: implement.
        
        IInventory Inventory { get; }
        
        byte GetStackPosition();
        bool CanSee(ICreature creature);
        bool CanSee(Location location);
        void TurnToDirection(Direction direction);
        void StopWalking();
        void WalkTo(Direction direction);
        void AutoWalk(params Direction[] directions);
        TimeSpan CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime);
        void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);
    }
}
