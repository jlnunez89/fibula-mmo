// -----------------------------------------------------------------
// <copyright file="ICreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for all creatures in the game.
    /// </summary>
    public interface ICreature : IThing, ISuffersExhaustion, IHasSkills, ICylinder, IHasInventory
    {
        // event OnCreatureStateChange OnZeroHealth;
        // event OnCreatureStateChange OnInventoryChanged;

        /// <summary>
        /// Gets the creature's in-game id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the creature's blood type.
        /// </summary>
        BloodType Blood { get; }

        /// <summary>
        /// Gets the article in the name of the creature.
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Gets the name of the creature.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the creature's corpse.
        /// </summary>
        ushort Corpse { get; }

        /// <summary>
        /// Gets the creature's current hitpoints.
        /// </summary>
        ushort Hitpoints { get; }

        /// <summary>
        /// Gets the creature's maximum hitpoints.
        /// </summary>
        ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the creature's current manapoints.
        /// </summary>
        ushort Manapoints { get; }

        /// <summary>
        /// Gets the creature's maximum manapoints.
        /// </summary>
        ushort MaxManapoints { get; }

        /// <summary>
        /// Gets the creature's strength value for carrying stuff.
        /// </summary>
        decimal CarryStrength { get; }

        /// <summary>
        /// Gets the creature's outfit.
        /// </summary>
        Outfit Outfit { get; }

        /// <summary>
        /// Gets the creatures facing direction.
        /// </summary>
        Direction Direction { get; }

        /// <summary>
        /// Gets this creature's emitted light level.
        /// </summary>
        byte EmittedLightLevel { get; }

        /// <summary>
        /// Gets this creature's emitted light color.
        /// </summary>
        byte EmittedLightColor { get; }

        /// <summary>
        /// Gets this creature's movement speed.
        /// </summary>
        ushort Speed { get; }

        /// <summary>
        /// Gets this creature's flags.
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// Gets a value indicating whether this creature is currently invisible from any source.
        /// </summary>
        // TODO: rethink this one? might want to make creatures invisible depending on who's querying.
        bool IsInvisible { get; }

        /// <summary>
        /// Gets a value indicating whether this creature can perceive all invisible creatures.
        /// </summary>
        // TODO: rethink this one?
        bool CanSeeInvisible { get; }

        /// <summary>
        /// Gets this creature's skull status.
        /// </summary>
        // TODO: implement or decide what to do with this.
        byte Skull { get; }

        /// <summary>
        /// Gets this creature's shield status.
        /// </summary>
        // TODO: implement or decide what to do with this.
        byte Shield { get; } // TODO: implement.

        /// <summary>
        /// Gets the collection of open containers tracked by this player.
        /// </summary>
        IEnumerable<IContainerItem> OpenContainers { get; }

        // ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

        // byte NextStepId { get; set; }

        // void StopWalking();

        // void AutoWalk(params Direction[] directions);

        // void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);

        /// <summary>
        /// Checks if this creature can see a given creature.
        /// </summary>
        /// <param name="creature">The creature to check against.</param>
        /// <returns>True if this creature can see the given creature, false otherwise.</returns>
        bool CanSee(ICreature creature);

        /// <summary>
        /// Checks if this creature can see a given location.
        /// </summary>
        /// <param name="location">The location to check against.</param>
        /// <returns>True if this creature can see the given location, false otherwise.</returns>
        bool CanSee(Location location);

        /// <summary>
        /// Turns this creature to a given direction.
        /// </summary>
        /// <param name="direction">The direction to turn the creature to.</param>
        void TurnToDirection(Direction direction);

        /// <summary>
        /// Attempts to set this creature's <see cref="Outfit"/>.
        /// </summary>
        /// <param name="outfit">The new outfit to change to.</param>
        void SetOutfit(Outfit outfit);

        /// <summary>
        /// Makes this creature "think" and make decisions for the next game step.
        /// </summary>
        /// <returns>A collection of events with delays, representing decisions made after thinking.</returns>
        IEnumerable<(IEvent Event, TimeSpan Delay)> Think();

        /// <summary>
        /// Opens a container for this player, which tracks it.
        /// </summary>
        /// <param name="container">The container being opened.</param>
        /// <returns>The id of the container as seen by this player.</returns>
        byte OpenContainer(IContainerItem container);

        /// <summary>
        /// Opens a container by placing it at the given index id.
        /// If there is a container already open at this index, it is first closed.
        /// </summary>
        /// <param name="container">The container to open.</param>
        /// <param name="containerId">Optional. The index at which to open the container. Defaults to 0xFF which means open at any free index.</param>
        void OpenContainerAt(IContainerItem container, byte containerId = 0xFF);

        /// <summary>
        /// Gets the id of the given container as known by this player, if it is.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>The id of the container if known by this player.</returns>
        sbyte GetContainerId(IContainerItem container);

        /// <summary>
        /// Closes a container for this player, which stops tracking it.
        /// </summary>
        /// <param name="containerId">The id of the container being closed.</param>
        void CloseContainerWithId(byte containerId);

        /// <summary>
        /// Gets a container by the id known to this player.
        /// </summary>
        /// <param name="containerId">The id of the container.</param>
        /// <returns>The container, if found.</returns>
        IContainerItem GetContainerById(byte containerId);
    }
}
