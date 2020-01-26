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
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class Creature : Thing, ICreature
    {
        /// <summary>
        /// The maximum number of containers to maintain.
        /// </summary>
        private const int MaxContainers = 16;

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
        /// Stores the open containers of this creature.
        /// </summary>
        private readonly IContainerItem[] openContainers;

        /// <summary>
        /// Lock object to semaphore interaction with the location-based actions queue.
        /// </summary>
        private readonly object actionsAtLocationLock;

        /// <summary>
        /// The queue of current location-based actions to retry.
        /// </summary>
        private readonly Queue<(Location atLoc, Action action)> locationBasedRetryActions;

        /// <summary>
        /// Lock object to semaphore interaction with the range-based actions queue.
        /// </summary>
        private readonly object actionsWithinRangeLock;

        /// <summary>
        /// The queue of current range-based actions to retry.
        /// </summary>
        private readonly Queue<(byte range, uint creatureId, Action action)> rangeToCreatureBasedRetryActions;

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

            this.openContainers = new IContainerItem[MaxContainers];

            this.Name = name;
            this.Article = article;
            this.MaxHitpoints = maxHitpoints;
            this.Hitpoints = Math.Min(this.MaxHitpoints, hitpoints == 0 ? this.MaxHitpoints : hitpoints);
            this.MaxManapoints = maxManapoints;
            this.Manapoints = Math.Min(this.MaxManapoints, manapoints);
            this.Corpse = corpse;

            this.exhaustionLock = new object();
            this.ExhaustionInformation = new Dictionary<ExhaustionType, DateTimeOffset>();

            this.actionsAtLocationLock = new object();
            this.locationBasedRetryActions = new Queue<(Location atLoc, Action action)>();

            this.actionsWithinRangeLock = new object();
            this.rangeToCreatureBasedRetryActions = new Queue<(byte range, uint creatureId, Action action)>();

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

        /// <summary>
        /// Gets the collection of open containers tracked by this player.
        /// </summary>
        public IEnumerable<IContainerItem> OpenContainers => this.openContainers;

        /// <summary>
        /// Gets the collection of current location-based actions to retry.
        /// </summary>
        public IEnumerable<(Location atLocation, Action action)> LocationBasedActions
        {
            get
            {
                lock (this.actionsAtLocationLock)
                {
                    return this.locationBasedRetryActions.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the collection of current range-based actions to retry.
        /// </summary>
        public IEnumerable<(byte range, uint creatureId, Action action)> RangeBasedActions
        {
            get
            {
                lock (this.actionsWithinRangeLock)
                {
                    return this.rangeToCreatureBasedRetryActions.ToArray();
                }
            }
        }

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
        /// Gets the id of the given container as known by this player, if it is.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>The id of the container if known by this player.</returns>
        public sbyte GetContainerId(IContainerItem container)
        {
            for (sbyte i = 0; i < this.openContainers.Length; i++)
            {
                if (this.openContainers[i] == container)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Closes a container for this player, which stops tracking it.
        /// </summary>
        /// <param name="containerId">The id of the container being closed.</param>
        public void CloseContainerWithId(byte containerId)
        {
            try
            {
                this.openContainers[containerId].EndTracking(this.Id);
                this.openContainers[containerId] = null;
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Opens a container for this player, which tracks it.
        /// </summary>
        /// <param name="container">The container being opened.</param>
        /// <returns>The id of the container as seen by this player.</returns>
        public byte OpenContainer(IContainerItem container)
        {
            container.ThrowIfNull(nameof(container));

            for (byte i = 0; i < this.openContainers.Length; i++)
            {
                if (this.openContainers[i] != null)
                {
                    continue;
                }

                this.openContainers[i] = container;
                this.openContainers[i].BeginTracking(this.Id, i);

                return i;
            }

            var lastIdx = (byte)(this.openContainers.Length - 1);

            this.openContainers[lastIdx] = container;

            return lastIdx;
        }

        /// <summary>
        /// Opens a container by placing it at the given index id.
        /// If there is a container already open at this index, it is first closed.
        /// </summary>
        /// <param name="container">The container to open.</param>
        /// <param name="containerId">Optional. The index at which to open the container. Defaults to 0xFF which means open at any free index.</param>
        public void OpenContainerAt(IContainerItem container, byte containerId = 0xFF)
        {
            if (containerId == 0xFF)
            {
                this.OpenContainer(container);

                return;
            }

            this.openContainers[containerId]?.EndTracking(this.Id);
            this.openContainers[containerId] = container;
            this.openContainers[containerId].BeginTracking(this.Id, containerId);
        }

        /// <summary>
        /// Gets a container by the id known to this player.
        /// </summary>
        /// <param name="containerId">The id of the container.</param>
        /// <returns>The container, if found.</returns>
        public IContainerItem GetContainerById(byte containerId)
        {
            try
            {
                var container = this.openContainers[containerId];

                container.BeginTracking(this.Id, containerId);

                return container;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public void AddContent(ILogger logger, IItemFactory itemFactory, IEnumerable<IParsedElement> contentElements)
        {
            throw new NotImplementedException();
        }

        public (bool result, IThing remainder) AddContent(IItemFactory itemFactory, IThing thing, byte index = 255)
        {
            return (false, thing);
        }

        public (bool result, IThing remainder) RemoveContent(IItemFactory itemFactory, ref IThing thing, byte index = 255, byte amount = 1)
        {
            throw new NotImplementedException();
        }

        public (bool result, IThing remainderToChange) ReplaceContent(IItemFactory itemFactory, IThing fromThing, IThing toThing, byte index = 255, byte amount = 1)
        {
            throw new NotImplementedException();
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
        /// Adds an action that should be retried when the creature steps at this particular location.
        /// </summary>
        /// <param name="retryLoc">The location at which the retry happens.</param>
        /// <param name="action">The delegate action to invoke when the location is reached.</param>
        public void EnqueueRetryActionAtLocation(Location retryLoc, Action action)
        {
            if (action == null || retryLoc == default)
            {
                return;
            }

            lock (this.actionsAtLocationLock)
            {
                this.locationBasedRetryActions.Enqueue((retryLoc, action));
            }
        }

        /// <summary>
        /// Removes a single action from the queue given its particular location.
        /// </summary>
        /// <param name="loc">The location by which to identify the action to remove from the queue.</param>
        public void DequeueActionAtLocation(Location loc)
        {
            lock (this.actionsAtLocationLock)
            {
                int count = this.locationBasedRetryActions.Count;

                for (int i = 0; i < count; i++)
                {
                    var tuple = this.locationBasedRetryActions.Dequeue();

                    if (tuple.atLoc != loc)
                    {
                        this.locationBasedRetryActions.Enqueue(tuple);
                        continue;
                    }

                    // we already removed it, just exit.
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all actions from the location-based actions queue.
        /// </summary>
        public void ClearAllLocationActions()
        {
            lock (this.actionsAtLocationLock)
            {
                this.locationBasedRetryActions.Clear();
            }
        }

        /// <summary>
        /// Adds an action that should be retried when the creature steps within a given range of another.
        /// </summary>
        /// <param name="range">The range withing which the retry happens.</param>
        /// <param name="creatureId">The id of the creature which to calculate the range to.</param>
        /// <param name="action">The delegate action to invoke when the location is reached.</param>
        public void EnqueueRetryActionWithinRangeToCreature(byte range, uint creatureId, Action action)
        {
            if (action == null || creatureId == 0)
            {
                return;
            }

            lock (this.actionsWithinRangeLock)
            {
                this.rangeToCreatureBasedRetryActions.Enqueue((range, creatureId, action));
            }
        }

        /// <summary>
        /// Removes a single action from the queue given its particular location.
        /// </summary>
        /// <param name="withinRange">The range within which to identify the action to remove from the queue.</param>
        /// <param name="creatureId">The location to which to calculate the range.</param>
        public void DequeueRetryActionWithinRangeToCreature(byte withinRange, uint creatureId)
        {
            lock (this.actionsWithinRangeLock)
            {
                int count = this.rangeToCreatureBasedRetryActions.Count;

                for (int i = 0; i < count; i++)
                {
                    var tuple = this.rangeToCreatureBasedRetryActions.Dequeue();

                    if (tuple.range > withinRange || tuple.creatureId != creatureId)
                    {
                        this.rangeToCreatureBasedRetryActions.Enqueue(tuple);
                        continue;
                    }

                    // we already removed it, just exit.
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all actions from the location-based actions queue.
        /// </summary>
        public void ClearAllRangeBasedActions()
        {
            lock (this.actionsWithinRangeLock)
            {
                this.rangeToCreatureBasedRetryActions.Clear();
            }
        }
    }
}
