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
    using System.Diagnostics.CodeAnalysis;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class Creature : Thing, ICreature
    {
        /// <summary>
        /// The default index for body containers.
        /// </summary>
        private const byte DefaultBodyContainerIndex = 0;

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

            this.Speed = 200;

            this.Skills = new Dictionary<SkillType, ISkill>();

            // Subscribe any attack-impacting conditions here
            // this.OnThingChanged += this.CheckAutoAttack;             // Are we in range with our target now/still?
            // this.OnThingChanged += this.CheckPendingActions;         // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;                      // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?
        }

        /// <summary>
        /// Gets the id of this creature.
        /// </summary>
        public override ushort ThingId => ICreature.CreatureThingId;

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

        /// <summary>
        /// Gets the location where this thing is being carried at, which is null for creatures.
        /// </summary>
        public override Location? CarryLocation
        {
            get
            {
                return null;
            }
        }

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

        /// <summary>
        /// Gets this creature's flags.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the inventory for the creature.
        /// </summary>
        public abstract IInventory Inventory { get; protected set; }

        public abstract bool IsThinking { get; }

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
                if (this.ExhaustionInformation.ContainsKey(type) && this.ExhaustionInformation[type] > fromTime)
                {
                    fromTime = this.ExhaustionInformation[type];
                }

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

        public void AddContent(ILogger logger, IItemFactory itemFactory, IEnumerable<IParsedElement> contentElements)
        {
            // TODO: iterate all body containers and try to add there.
            throw new NotImplementedException();
        }

        public (bool result, IThing remainder) AddContent(IItemFactory itemFactory, IThing thing, byte index = 255)
        {
            // TODO: try to add at that specific body container.
            return (false, thing);
        }

        public (bool result, IThing remainder) RemoveContent(IItemFactory itemFactory, ref IThing thing, byte index = 255, byte amount = 1)
        {
            // TODO: try to delete from that specific body container.
            throw new NotImplementedException();
        }

        public (bool result, IThing remainderToChange) ReplaceContent(IItemFactory itemFactory, IThing fromThing, IThing toThing, byte index = 255, byte amount = 1)
        {
            if (this.Inventory[index] is IContainerItem bodyContainer)
            {
                return bodyContainer.ReplaceContent(itemFactory, fromThing, toThing, DefaultBodyContainerIndex, amount);
            }

            return (false, fromThing);
        }

        /// <summary>
        /// Gets an item within this cylinder.
        /// </summary>
        /// <param name="index">The index at which to get the item.</param>
        /// <returns>The item found at the index, if there is any.</returns>
        public IItem FindItemAt(byte index)
        {
            if (this.Inventory[index] is IContainerItem bodyContainer)
            {
                return bodyContainer[DefaultBodyContainerIndex];
            }

            return null;
        }

        /// <summary>
        /// Gets the description of this creature as seen by the given player.
        /// </summary>
        /// <param name="forPlayer">The player as which to get the description.</param>
        /// <returns>The description string.</returns>
        public override string GetDescription(IPlayer forPlayer)
        {
            return $"{this.Article} {this.Name}.";
        }

        /// <summary>
        /// Provides a string describing the current creature for logging purposes.
        /// </summary>
        /// <returns>The string to log.</returns>
        public override string DescribeForLogger()
        {
            return $"{(string.IsNullOrWhiteSpace(this.Article) ? string.Empty : $"{this.Article} ")}{this.Name}";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">The other object to compare against.</param>
        /// <returns>True if the current object is equal to the other parameter, false otherwise.</returns>
        public bool Equals([AllowNull] ICreature other)
        {
            return this.Id == other?.Id;
        }
    }
}
