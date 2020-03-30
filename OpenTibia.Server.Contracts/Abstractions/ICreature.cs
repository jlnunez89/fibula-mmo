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
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for all creatures in the game.
    /// </summary>
    public interface ICreature : IThing, ISuffersExhaustion, IHasSkills, ICylinder, IHasInventory, IEquatable<ICreature>
    {
        /// <summary>
        /// The id for things that are creatures.
        /// </summary>
        public const ushort CreatureThingId = 0x63;

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

        ///// <summary>
        ///// Evaluates the location-based retry actions pending of a given creature, and invokes them if any is met.
        ///// </summary>
        ///// <param name="operationContext">The context to pass down to operations to fire.</param>
        ///// <returns>True if there is at least one action that was executed, false otherwise.</returns>
        //bool ExecuteLocationBasedOperations(IOperationContext operationContext);

        ///// <summary>
        ///// Adds an operation that should be fired when the creature steps at a given location.
        ///// </summary>
        ///// <param name="atLocation">The location.</param>
        ///// <param name="operation">The operation.</param>
        //void SetOperationAtLocation(Location atLocation, IOperation operation);

        ///// <summary>
        ///// Removes all the operations from the queue at a given location.
        ///// </summary>
        ///// <param name="atLocation">The location.</param>
        //void ClearAllOperationsAtLocation(Location atLocation);

        ///// <summary>
        ///// Removes all operations from the location-based actions queue.
        ///// </summary>
        //void ClearAllLocationBasedOperations();

        ///// <summary>
        ///// Gets the collection of operations to fire based on location.
        ///// </summary>
        ///// <param name="atLocation">The location to evaluate for.</param>
        ///// <returns>The collection of operations set to be fired at the location, if any.</returns>
        //IEnumerable<IOperation> GetLocationBasedOperations(Location atLocation);

        ///// <summary>
        ///// Adds an operation that should be fired when the creature steps within a given range of another.
        ///// </summary>
        ///// <param name="range">The range withing which the retry happens.</param>
        ///// <param name="creatureId">The id of the creature which to calculate the range to.</param>
        ///// <param name="operation">The operation.</param>
        //void EnqueueOperationWithinRangeToCreature(byte range, uint creatureId, IOperation operation);

        ///// <summary>
        ///// Removes a single action from the queue given its particular location.
        ///// </summary>
        ///// <param name="withinRange">The range within which to identify the action to remove from the queue.</param>
        ///// <param name="creatureId">The id of the creature which to calculate the range to.</param>
        //void DequeueOperationsWithinRangeToCreature(byte withinRange, uint creatureId);

        ///// <summary>
        ///// Removes all actions from the location-based actions queue.
        ///// </summary>
        //void ClearAllRangeBasedOperations();

        ///// <summary>
        ///// Evaluates the range-based operations of this creature, and invokes them if any is met.
        ///// </summary>
        ///// <param name="operationContext">The context to pass down to operations to fire.</param>
        ///// <returns>True if there is at least one operation that was executed, false otherwise.</returns>
        //bool ExecuteRangeBasedOperations(IOperationContext operationContext);
    }
}
