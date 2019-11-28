// -----------------------------------------------------------------
// <copyright file="Creature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class Creature : Thing, ICreature
    {
        /// <summary>
        /// Lock used when assigning creature ids.
        /// </summary>
        private static readonly object IdLock = new object();

        /// <summary>
        /// Counter to assign new ids to new creatures created.
        /// </summary>
        private static uint idCounter = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creature"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="article"></param>
        /// <param name="maxHitpoints"></param>
        /// <param name="maxManapoints"></param>
        /// <param name="corpse"></param>
        /// <param name="hitpoints"></param>
        /// <param name="manapoints"></param>
        protected Creature(
            string name,
            string article,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse = 0,
            ushort hitpoints = 0,
            ushort manapoints = 0)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            if (maxHitpoints == 0)
            {
                throw new ArgumentException($"{nameof(maxHitpoints)} must be positive.", nameof(maxHitpoints));
            }

            lock (Creature.IdLock)
            {
                this.Id = idCounter++;
            }

            this.Name = name;
            this.Article = article;
            this.MaxHitpoints = maxHitpoints;
            this.Hitpoints = Math.Min(this.MaxHitpoints, hitpoints == 0 ? this.MaxHitpoints : hitpoints);
            this.MaxManapoints = maxManapoints;
            this.Manapoints = Math.Min(this.MaxManapoints, manapoints);
            this.Corpse = corpse;

            this.ExhaustionInformation = new Dictionary<ExhaustionType, DateTimeOffset>();

            this.Outfit = new Outfit
            {
                Id = 0,
                ItemIdLookAlike = 0,
            };

            this.Speed = 220;

            this.Skills = new Dictionary<SkillType, ISkill>();

            // this.WalkingQueue = new ConcurrentQueue<Tuple<byte, Direction>>();

            // Subscribe any attack-impacting conditions here
            // this.OnThingChanged += this.CheckAutoAttack;         // Are we in range with our target now/still?
            // this.OnThingChanged += this.CheckPendingActions;                    // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;          // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;        // Equipped / DeEquiped something?

            // this.Hostiles = new HashSet<uint>();
            // this.Friendly = new HashSet<uint>();
        }

        // public event OnAttackTargetChange OnTargetChanged;

        /// <summary>
        /// Gets the id of this creature.
        /// </summary>
        public override ushort ThingId => CreatureThingId;

        /// <summary>
        /// Gets the description of the creature.
        /// </summary>
        public override string Description => $"{this.Article} {this.Name}";

        /// <summary>
        /// Gets the inspection text of the creature.
        /// </summary>
        public override string InspectionText => this.InspectionText;

        /// <summary>
        /// Gets the creature's in-game id.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Gets the article in the name of the creature.
        /// </summary>
        public string Article { get; }

        public string Name { get; }

        public ushort Corpse { get; }

        public ushort Hitpoints { get; }

        public ushort MaxHitpoints { get; }

        public ushort Manapoints { get; }

        public ushort MaxManapoints { get; }

        public decimal CarryStrength { get; protected set; }

        public Outfit Outfit { get; protected set; }

        public Direction Direction { get; protected set; }

        public Location LocationInFront => this.CalculateLocationInFront();

        public byte EmittedLightLevel { get; protected set; }

        public byte EmittedLightColor { get; protected set; }

        public ushort Speed { get; protected set; }

        public uint Flags { get; private set; }

        public BloodType Blood { get; protected set; }

        public IDictionary<SkillType, ISkill> Skills { get; }

        /// <summary>
        /// Gets the current exhaustion information for the entity.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="ExhaustionType"/>, and the value is a <see cref="DateTimeOffset"/>: the date and time
        /// at which exhaustion is completely recovered.
        /// </remarks>
        public IDictionary<ExhaustionType, DateTimeOffset> ExhaustionInformation { get; }

        // public IList<Condition> Conditions { get; protected set; } // TODO: implement.
        public bool IsInvisible { get; protected set; } // TODO: implement.

        public bool CanSeeInvisible { get; } // TODO: implement.

        public byte Skull { get; protected set; } // TODO: implement.

        public byte Shield { get; protected set; } // TODO: implement.

        public ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

        public byte NextStepId { get; set; }

        public HashSet<uint> Hostiles { get; }

        public HashSet<uint> Friendly { get; }

        public abstract IInventory Inventory { get; protected set; }

        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the entity's exhaustion is recovered from.
        /// </summary>
        /// <param name="type">The type of exhaustion.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public TimeSpan CalculateRemainingCooldownTime(ExhaustionType type, DateTimeOffset currentTime)
        {
            if (!this.ExhaustionInformation.TryGetValue(type, out DateTimeOffset readyAtTime))
            {
                return TimeSpan.Zero;
            }

            var timeLeft = readyAtTime - currentTime;

            if (timeLeft < TimeSpan.Zero)
            {
                this.ExhaustionInformation.Remove(type);
            }

            return timeLeft;
        }

        /// <summary>
        /// Calculates the current percentual value between current and target counts for the given skill.
        /// </summary>
        /// <param name="type">The type of skill to calculate for.</param>
        /// <returns>A value between [0, 99] representing the current percentual value.</returns>
        public byte CalculateSkillPercent(SkillType type)
        {
            return (byte)Math.Min(100, this.Skills[type].Count * 100 / (this.Skills[type].Target + 1));
        }

        /// <summary>
        /// Makes this creature "think" and make decisions for the next game step.
        /// </summary>
        /// <returns>A collection of events with delays, representing decisions made after thinking.</returns>
        public IEnumerable<(IEvent Event, TimeSpan Delay)> Think()
        {
            // TODO: return something else here.
            return null;
        }

        /// <summary>
        /// Turns this creature to a given direction.
        /// </summary>
        /// <param name="direction">The direction to turn the creature to.</param>
        public void TurnToDirection(Direction direction)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Attempts to set this creature's <see cref="Outfit"/>.
        /// </summary>
        /// <param name="outfit">The new outfit to change to.</param>
        public void SetOutfit(Outfit outfit)
        {
            this.Outfit = outfit;
        }

        /// <summary>
        /// Calculates the <see cref="Location"/> in front of this creature.
        /// </summary>
        /// <returns>The location in front of this creature.</returns>
        private Location CalculateLocationInFront()
        {
            switch (this.Direction)
            {
                default:
                case Direction.South:
                    return this.Location + new Location() { X = 0, Y = 1, Z = 0 };

                case Direction.North:
                    return this.Location + new Location() { X = 0, Y = -1, Z = 0 };

                case Direction.East:
                case Direction.NorthEast:
                case Direction.SouthEast:
                    return this.Location + new Location() { X = 1, Y = 0, Z = 0 };

                case Direction.West:
                case Direction.NorthWest:
                case Direction.SouthWest:
                    return this.Location + new Location() { X = -1, Y = 1, Z = 0 };
            }
        }




        // protected virtual void CheckPendingActions(IThing thingChanged, ThingStateChangedEventArgs eventAgrs) { }

        // ~Creature()
        // {
        //    OnLocationChanged -= CheckAutoAttack;         // Are we in range with our target now/still?
        //    OnLocationChanged -= CheckPendingActions;                  // Are we in range with any of our pending actions?
        //    //OnTargetChanged -= CheckAutoAttack;           // Are we attacking someone new / not attacking anymore?
        //    //OnInventoryChanged -= Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?
        // }

        // public bool HasFlag(CreatureFlag flag)
        // {
        //    var flagValue = (uint)flag;

        // return (this.Flags & flagValue) == flagValue;
        // }

        // public void SetFlag(CreatureFlag flag)
        // {
        //    this.Flags |= (uint)flag;
        // }

        // public void UnsetFlag(CreatureFlag flag)
        // {
        //    this.Flags &= ~(uint)flag;
        // }

        public bool CanSee(ICreature otherCreature)
        {
            otherCreature.ThrowIfNull(nameof(otherCreature));

            return (!otherCreature.IsInvisible || this.CanSeeInvisible) && this.CanSee(otherCreature.Location);
        }

        public bool CanSee(Location pos)
        {
            if (this.Location.Z <= 7)
            {
                // we are on ground level or above (7 -> 0)
                // view is from 7 -> 0
                if (pos.Z > 7)
                {
                    return false;
                }
            }
            else if (this.Location.Z >= 8)
            {
                // we are underground (8 -> 15)
                // view is +/- 2 from the floor we stand on
                if (Math.Abs(this.Location.Z - pos.Z) > 2)
                {
                    return false;
                }
            }

            var offsetZ = this.Location.Z - pos.Z;

            if (pos.X >= this.Location.X - 8 + offsetZ && pos.X <= this.Location.X + 9 + offsetZ &&
                pos.Y >= this.Location.Y - 6 + offsetZ && pos.Y <= this.Location.Y + 7 + offsetZ)
            {
                return true;
            }

            return false;
        }

        // public void SetAttackTarget(uint targetId)
        // {
        //    if (targetId == this.Id || this.AutoAttackTargetId == targetId)
        //    {
        //        // if we want to attack ourselves or if the current target is already the one we want... no change needed.
        //        return;
        //    }

        // // save the previus target to report
        //    var oldTargetId = this.AutoAttackTargetId;

        // if (targetId == 0)
        //    {
        //        // clearing our target.
        //        if (this.AutoAttackTargetId != 0)
        //        {
        //            var attackTarget = this.Game.GetCreatureWithId(this.AutoAttackTargetId);

        // if (attackTarget != null)
        //            {
        //                attackTarget.OnThingChanged -= this.CheckAutoAttack;
        //            }

        // this.AutoAttackTargetId = 0;
        //        }
        //    }
        //    else
        //    {
        //        // TODO: verify against this.Hostiles.Union(this.Friendly).
        //        // if (creature != null)
        //        // {
        //        this.AutoAttackTargetId = targetId;

        // var attackTarget = this.Game.GetCreatureWithId(this.AutoAttackTargetId);

        // if (attackTarget != null)
        //        {
        //            attackTarget.OnThingChanged += this.CheckAutoAttack;
        //        }

        // // }
        //        // else
        //        // {
        //        //    Console.WriteLine("Taget creature not found in attacker\'s view.");
        //        // }
        //    }

        // // report the change to our subscribers.
        //    //this.OnTargetChanged?.Invoke(oldTargetId, targetId);
        //    //this.CheckAutoAttack(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(this.location) });
        // }

        // public void CheckAutoAttack(IThing thingChanged, ThingStateChangedEventArgs eventAgrs)
        // {
        //    if (this.AutoAttackTargetId == 0)
        //    {
        //        return;
        //    }

        // var attackTarget = this.Game.GetCreatureWithId(this.AutoAttackTargetId);

        // if (attackTarget == null || (thingChanged != this && thingChanged != attackTarget) || eventAgrs.PropertyChanged != nameof(this.Location))
        //    {
        //        return;
        //    }

        // var locationDiff = this.Location - attackTarget.Location;
        //    var inRange = this.CanSee(attackTarget) && locationDiff.Z == 0 && locationDiff.MaxValueIn2D <= this.AutoAttackRange;

        // if (inRange)
        //    {
        //        this.Game.SignalAttackReady();
        //    }
        // }

        // public void StopWalking()
        // {
        //    lock (this.enqueueWalkLock)
        //    {
        //        this.WalkingQueue.Clear(); // reset the actual queue
        //        this.UpdateLastStepInfo(0);
        //    }
        // }

        // public void AutoWalk(params Direction[] directions)
        // {
        //    lock (this.enqueueWalkLock)
        //    {
        //        if (this.WalkingQueue.Count > 0)
        //        {
        //            this.StopWalking();
        //        }

        // var nextStepId = this.NextStepId;

        // foreach (var direction in directions)
        //        {
        //            this.WalkingQueue.Enqueue(new Tuple<byte, Direction>((byte)(nextStepId++ % byte.MaxValue), direction));
        //        }

        // this.Game.SignalWalkAvailable();
        //    }
        // }

        // public void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true)
        // {
        //    var tilePenalty = this.Tile?.Ground?.MovementPenalty;
        //    var totalPenalty = (tilePenalty ?? 200) * (wasDiagonal ? 2 : 1);

        // this.Cooldowns[ExhaustionType.Movement] = new Tuple<DateTimeOffset, TimeSpan>(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000 * totalPenalty / (double)Math.Max(1, (int)this.Speed)));

        // this.NextStepId = (byte)(lastStepId + 1);
        // }
    }
}
