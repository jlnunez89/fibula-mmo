﻿// -----------------------------------------------------------------
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
        /// Gets the collection of current location-based actions to retry.
        /// </summary>
        IEnumerable<(Location atLocation, Action action)> LocationBasedActions { get; }

        /// <summary>
        /// Gets the collection of current range-based actions to retry.
        /// </summary>
        IEnumerable<(byte range, uint creatureId, Action action)> RangeBasedActions { get; }

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
        /// Adds an action that should be retried when the creature steps at this particular location.
        /// </summary>
        /// <param name="retryLoc">The location at which the retry happens.</param>
        /// <param name="action">The delegate action to invoke when the location is reached.</param>
        void EnqueueRetryActionAtLocation(Location retryLoc, Action action);

        /// <summary>
        /// Removes a single action from the queue given its particular location.
        /// </summary>
        /// <param name="loc">The location by which to identify the action to remove from the queue.</param>
        void DequeueActionAtLocation(Location loc);

        /// <summary>
        /// Removes all actions from the location-based actions queue.
        /// </summary>
        void ClearAllLocationActions();

        /// <summary>
        /// Adds an action that should be retried when the creature steps within a given range of another.
        /// </summary>
        /// <param name="range">The range withing which the retry happens.</param>
        /// <param name="creatureId">The id of the creature which to calculate the range to.</param>
        /// <param name="action">The delegate action to invoke when the location is reached.</param>
        void EnqueueRetryActionWithinRangeToCreature(byte range, uint creatureId, Action action);

        /// <summary>
        /// Removes a single action from the queue given its particular location.
        /// </summary>
        /// <param name="withinRange">The range within which to identify the action to remove from the queue.</param>
        /// <param name="creatureId">The id of the creature which to calculate the range to.</param>
        void DequeueRetryActionWithinRangeToCreature(byte withinRange, uint creatureId);

        /// <summary>
        /// Removes all actions from the location-based actions queue.
        /// </summary>
        void ClearAllRangeBasedActions();

        /// <summary>
        /// Evaluates the location-based retry actions pending of a given creature, and invokes them if any is met.
        /// </summary>
        /// <returns>True if there is at least one action that was executed, false otherwise.</returns>
        bool EvaluateLocationBasedActions();

        /// <summary>
        /// Evaluates the location-based retry actions pending of a given creature, and invokes them if any is met.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <returns>True if there is at least one action that was executed, false otherwise.</returns>
        bool EvaluateCreatureRangeBasedActions(ICreatureFinder creatureFinder);
    }
}
