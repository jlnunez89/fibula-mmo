// -----------------------------------------------------------------
// <copyright file="Creature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fibula.Common;
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Structs;
    using Fibula.Items.Contracts.Abstractions;

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

            this.Outfit = new Outfit
            {
                Id = 0,
                ItemIdLookAlike = 0,
            };

            this.BaseSpeed = 70;

            // this.Skills = new Dictionary<SkillType, ISkill>();

            // Subscribe any attack-impacting conditions here
            // this.OnThingChanged += this.CheckAutoAttack;             // Are we in range with our target now/still?
            // this.OnThingChanged += this.CheckPendingActions;         // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;                      // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?
        }

        /// <summary>
        /// Gets the id of this creature.
        /// </summary>
        public override ushort ThingId => CreatureConstants.CreatureThingId;

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

        /// <summary>
        /// Gets or sets the creature's strength to carry stuff.
        /// </summary>
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
        public abstract ushort Speed { get; protected set; }

        /// <summary>
        /// Gets or sets this creature's variable speed.
        /// </summary>
        public int VariableSpeed { get; protected set; }

        /// <summary>
        /// Gets or sets this creature's base speed.
        /// </summary>
        public int BaseSpeed { get; protected set; }

        /// <summary>
        /// Gets this creature's flags.
        /// </summary>
        public uint Flags { get; private set; }

        /// <summary>
        /// Gets or sets this creature's blood type.
        /// </summary>
        public BloodType Blood { get; protected set; }

        ///// <summary>
        ///// Gets this creature's skills.
        ///// </summary>
        // public IDictionary<SkillType, ISkill> Skills { get; }

        /// <summary>
        /// Gets or sets the inventory for the creature.
        /// </summary>
        public abstract IInventory Inventory { get; protected set; }

        /// <summary>
        /// Gets or sets this creature's walk plan.
        /// </summary>
        public WalkPlan? WalkPlan { get; set; }

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

            // (!otherCreature.IsInvisible || this.CanSeeInvisible) &&
            return this.CanSee(otherCreature.Location);
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
        /// Attempts to add a <see cref="IThing"/> to this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index at which to add the <see cref="IThing"/>. Defaults to 0xFF, which instructs to add the <see cref="IThing"/> at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) AddContent(IThingFactory thingFactory, IThing thing, byte index = 0xFF)
        {
            // TODO: try to add at that specific body container.
            return (false, thing);
        }

        /// <summary>
        /// Attempts to remove a thing from this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="index">Optional. The index from which to remove the <see cref="IThing"/>. Defaults to 0xFF, which instructs to remove the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainder) RemoveContent(IThingFactory thingFactory, ref IThing thing, byte index = 0xFF, byte amount = 1)
        {
            // TODO: try to delete from that specific body container.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to replace a <see cref="IThing"/> from this container with another.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="fromThing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="toThing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index from which to replace the <see cref="IThing"/>. Defaults to 0xFF, which instructs to replace the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="fromThing"/> to replace.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        public (bool result, IThing remainderToChange) ReplaceContent(IThingFactory thingFactory, IThing fromThing, IThing toThing, byte index = 0xFF, byte amount = 1)
        {
            if (this.Inventory[index] is IContainerItem bodyContainer)
            {
                return bodyContainer.ReplaceContent(thingFactory, fromThing, toThing, DefaultBodyContainerIndex, amount);
            }

            return (false, fromThing);
        }

        /// <summary>
        /// Attempts to find an <see cref="IThing"/> whitin this container.
        /// </summary>
        /// <param name="index">The index at which to look for the <see cref="IThing"/>.</param>
        /// <returns>The <see cref="IThing"/> found at the index, if any was found.</returns>
        public IThing FindThingAtIndex(byte index)
        {
            if (this.Inventory[index] is IContainerItem bodyContainer)
            {
                return bodyContainer[DefaultBodyContainerIndex];
            }

            return null;
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

        /// <summary>
        /// Calculates the base movement speed of the creature.
        /// </summary>
        /// <returns>The base movement speed of the creature.</returns>
        protected virtual ushort CalculateMovementBaseSpeed()
        {
            return (ushort)((2 * (this.VariableSpeed + this.BaseSpeed)) + 80);
        }
    }
}
