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
        /// Lock object to semaphore interaction with the exhaustion dictionary.
        /// </summary>
        private readonly object exhaustionLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creature"/> class.
        /// </summary>
        /// <param name="name">The name of this creature.</param>
        /// <param name="article">An article for the name of this creature.</param>
        /// <param name="maxHitpoints">The maximum hitpoints of the creature.</param>
        /// <param name="maxManapoints">The maximum manapoints of the creature.</param>
        /// <param name="corpse">The corpse of the creature.</param>
        /// <param name="hitpoints">The current hitpoints of the creature.</param>
        /// <param name="manapoints">The current manapoints of the creature.</param>
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

            this.exhaustionLock = new object();
            this.ExhaustionInformation = new Dictionary<ExhaustionType, DateTimeOffset>();

            this.Outfit = new Outfit
            {
                Id = 0,
                ItemIdLookAlike = 0,
            };

            this.Speed = 220;

            this.Skills = new Dictionary<SkillType, ISkill>();

            // Subscribe any attack-impacting conditions here
            // this.OnThingChanged += this.CheckAutoAttack;             // Are we in range with our target now/still?
            // this.OnThingChanged += this.CheckPendingActions;         // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;                      // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?

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
        public override string InspectionText => this.Description;

        /// <summary>
        /// Gets the creature's in-game id.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Gets the article in the name of the creature.
        /// </summary>
        public string Article { get; }

        /// <summary>
        /// Gets the name of the creature.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the corpse of the creature.
        /// </summary>
        public ushort Corpse { get; }

        /// <summary>
        /// Gets thre current hitpoints.
        /// </summary>
        public ushort Hitpoints { get; }

        /// <summary>
        /// Gets the maximum hitpoints.
        /// </summary>
        public ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the current manapoints.
        /// </summary>
        public ushort Manapoints { get; }

        /// <summary>
        /// Gets the maximum manapoints.
        /// </summary>
        public ushort MaxManapoints { get; }

        public decimal CarryStrength { get; protected set; }

        /// <summary>
        /// Gets or sets the outfit of this creature.
        /// </summary>
        public Outfit Outfit { get; protected set; }

        /// <summary>
        /// Gets or sets the direction that this creature is facing.
        /// </summary>
        public Direction Direction { get; protected set; }

        /// <summary>
        /// Gets the location in front of this creature.
        /// </summary>
        public Location LocationInFront => this.CalculateLocationInFront();

        /// <summary>
        /// Gets or sets this creature's light level.
        /// </summary>
        public byte EmittedLightLevel { get; protected set; }

        /// <summary>
        /// Gets or sets this creature's light color.
        /// </summary>
        public byte EmittedLightColor { get; protected set; }

        /// <summary>
        /// Gets or sets this creature's speed.
        /// </summary>
        public ushort Speed { get; protected set; }

        public uint Flags { get; private set; }

        /// <summary>
        /// Gets or sets this creature's blood type.
        /// </summary>
        public BloodType Blood { get; protected set; }

        /// <summary>
        /// Gets this creature's skills.
        /// </summary>
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

        public abstract IInventory Inventory { get; protected set; }

        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the entity's exhaustion is recovered from.
        /// </summary>
        /// <param name="type">The type of exhaustion.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public TimeSpan CalculateRemainingCooldownTime(ExhaustionType type, DateTimeOffset currentTime)
        {
            lock (this.exhaustionLock)
            {
                if (!this.ExhaustionInformation.TryGetValue(type, out DateTimeOffset readyAtTime))
                {
                    return TimeSpan.Zero;
                }

                var timeLeft = readyAtTime - currentTime;

                if (timeLeft < TimeSpan.Zero)
                {
                    this.ExhaustionInformation.Remove(type);
                    return TimeSpan.Zero;
                }

                return timeLeft;
            }
        }

        /// <summary>
        /// Adds exhaustion of the given type.
        /// </summary>
        /// <param name="type">The type of exhaustion to add.</param>
        /// <param name="fromTime">The reference time from which to add.</param>
        /// <param name="timeSpan">The amount of time to add exhaustion for.</param>
        public void AddExhaustion(ExhaustionType type, DateTimeOffset fromTime, TimeSpan timeSpan)
        {
            lock (this.exhaustionLock)
            {
                this.ExhaustionInformation[type] = fromTime + timeSpan;
            }
        }

        /// <summary>
        /// Adds exhaustion of the given type.
        /// </summary>
        /// <param name="type">The type of exhaustion to add.</param>
        /// <param name="fromTime">The reference time from which to add.</param>
        /// <param name="milliseconds">The amount of time in milliseconds to add exhaustion for.</param>
        public void AddExhaustion(ExhaustionType type, DateTimeOffset fromTime, uint milliseconds)
        {
            this.AddExhaustion(type, fromTime, TimeSpan.FromMilliseconds(milliseconds));
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
        /// Checks if this creature can see a given creature.
        /// </summary>
        /// <param name="otherCreature">The creature to check against.</param>
        /// <returns>True if this creature can see the given creature, false otherwise.</returns>
        public bool CanSee(ICreature otherCreature)
        {
            otherCreature.ThrowIfNull(nameof(otherCreature));

            return (!otherCreature.IsInvisible || this.CanSeeInvisible) && this.CanSee(otherCreature.Location);
        }

        /// <summary>
        /// Checks if this creature can see a given location.
        /// </summary>
        /// <param name="location">The location to check against.</param>
        /// <returns>True if this creature can see the given location, false otherwise.</returns>
        public bool CanSee(Location location)
        {
            if (this.Location.Z <= 7)
            {
                // we are on ground level or above (7 -> 0)
                // view is from 7 -> 0
                if (location.Z > 7)
                {
                    return false;
                }
            }
            else if (this.Location.Z >= 8)
            {
                // we are underground (8 -> 15)
                // view is +/- 2 from the floor we stand on
                if (Math.Abs(this.Location.Z - location.Z) > 2)
                {
                    return false;
                }
            }

            var offsetZ = this.Location.Z - location.Z;

            if (location.X >= this.Location.X - 8 + offsetZ && location.X <= this.Location.X + 9 + offsetZ &&
                location.Y >= this.Location.Y - 6 + offsetZ && location.Y <= this.Location.Y + 7 + offsetZ)
            {
                return true;
            }

            return false;
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
