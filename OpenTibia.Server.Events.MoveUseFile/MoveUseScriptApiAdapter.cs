// -----------------------------------------------------------------
// <copyright file="MoveUseScriptApiAdapter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an adapter between the Move/Use scripts and the current <see cref="IScriptApi"/> implementation.
    /// </summary>
    /// <remarks>
    /// Names of functions in this class must match those found within the Move/Use file.
    /// </remarks>
    public class MoveUseScriptApiAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseScriptApiAdapter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptApi">A reference to the script api in use.</param>
        public MoveUseScriptApiAdapter(
            ILogger logger,
            IScriptApi scriptApi)
        {
            logger.ThrowIfNull(nameof(logger));
            scriptApi.ThrowIfNull(nameof(scriptApi));

            this.Logger = logger.ForContext<MoveUseScriptApiAdapter>();
            this.ScriptApi = scriptApi;
        }

        /// <summary>
        /// Gets the reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the reference to the script api in use.
        /// </summary>
        public IScriptApi ScriptApi { get; }

        /// <summary>
        /// Counts the occurrences of the <paramref name="thingToCount"/>'s type in it's own location.
        /// </summary>
        /// <param name="thingToCount">The thing to count.</param>
        /// <param name="comparer">The comparison to make.</param>
        /// <param name="value">The value to compare against.</param>
        /// <returns>True if the count passes the comparison and value provided, false otherwise.</returns>
        public bool CountObjects(IThing thingToCount, string comparer, ushort value)
        {
            if (thingToCount == null || string.IsNullOrWhiteSpace(comparer))
            {
                return false;
            }

            FunctionComparisonType comparison = FunctionComparisonType.Undefined;

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    comparison = FunctionComparisonType.Equal;
                    break;
                case ">=":
                    comparison = FunctionComparisonType.GreaterThanOrEqual;
                    break;
                case "<=":
                    comparison = FunctionComparisonType.LessThanOrEqual;
                    break;
                case ">":
                    comparison = FunctionComparisonType.GreaterThan;
                    break;
                case "<":
                    comparison = FunctionComparisonType.LessThan;
                    break;
            }

            if (comparison == FunctionComparisonType.Undefined)
            {
                throw new ArgumentException($"{nameof(this.CountObjects)}: Invalid {nameof(comparer)} value '{comparer}'.", nameof(comparer));
            }

            return this.ScriptApi.CompareCountOf(thingToCount, comparison, value);
        }

        /// <summary>
        /// Checks if the thing passed is considered a creature.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <returns>True if the thing is an <see cref="ICreature"/>, false otherwise.</returns>
        public bool IsCreature(IThing thing)
        {
            if (thing == null)
            {
                return false;
            }

            return this.ScriptApi.IsCreature(thing);
        }

        /// <summary>
        /// Checks if the given thing is of the type passed.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <param name="typeId">The type to check against.</param>
        /// <returns>True if the thing is of the given type, false otherwise.</returns>
        public bool IsType(IThing thing, ushort typeId)
        {
            return this.ScriptApi.IsType(thing, typeId);
        }

        /// <summary>
        /// Checks if the given thing is at the given location.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <param name="location">The location to check for.</param>
        /// <returns>True if the thing is at the given location, false otherwise.</returns>
        public bool IsPosition(IThing thing, Location location)
        {
            return this.ScriptApi.IsPosition(thing, location);
        }

        /// <summary>
        /// Checks if the thing passed is a player.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <returns>True if the thing is an <see cref="IPlayer"/>, false otherwise.</returns>
        public bool IsPlayer(IThing thing)
        {
            return this.ScriptApi.IsPlayer(thing);
        }

        /// <summary>
        /// Checks if there is an item of the given type at the given location.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <param name="typeId">The type for which to check.</param>
        /// <returns>True if there is an object of that type at the location, false otherwise.</returns>
        public bool IsObjectThere(Location location, ushort typeId)
        {
            return this.ScriptApi.IsObjectThere(location, typeId);
        }

        /// <summary>
        /// Checks if the player has a given right.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="rightStr">A string representing the right to check for.</param>
        /// <returns>True if the player has the right, false otherwise.</returns>
        public bool HasRight(IPlayer player, string rightStr)
        {
            return this.ScriptApi.HasRight(player, rightStr);
        }

        /// <summary>
        /// Checks if the player is allowed to log out.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if the player is allowed to log out, false otherwise.</returns>
        public bool MayLogout(IPlayer player)
        {
            return this.ScriptApi.MayLogout(player);
        }

        /// <summary>
        /// Checks if a thing has a flag value.
        /// </summary>
        /// <param name="itemThing">The thing to check.</param>
        /// <param name="flagStr">The string representation of the flag.</param>
        /// <returns>True if the thing has the flag, false otherwise.</returns>
        public bool HasFlag(IThing itemThing, string flagStr)
        {
            return this.ScriptApi.HasFlag(itemThing, flagStr);
        }

        /// <summary>
        /// Checks if the thing has a given profession.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <param name="professionId">The id of the profession to check for.</param>
        /// <returns>True if the thing is a player and has the given profession, false otherwise.</returns>
        public bool HasProfession(IThing thing, byte professionId)
        {
            return this.ScriptApi.HasProfession(thing, professionId);
        }

        /// <summary>
        /// Checks of the thing is an item, if it has the given attribute, and if that attribute has a value matching the comparison.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <param name="attributeStr">The attribute to check.</param>
        /// <param name="comparer">The type of comparison to make.</param>
        /// <param name="value">The value to comapre for.</param>
        /// <returns>True if the thing is an item, has the attribute, and the value of the attribute satisfies the comparison. False otherwise.</returns>
        public bool HasInstanceAttribute(IThing thing, string attributeStr, string comparer, ushort value)
        {
            if (!Enum.TryParse(attributeStr, out ItemAttribute actualAttribute))
            {
                return false;
            }

            FunctionComparisonType comparison = FunctionComparisonType.Undefined;

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    comparison = FunctionComparisonType.Equal;
                    break;
                case ">=":
                    comparison = FunctionComparisonType.GreaterThanOrEqual;
                    break;
                case "<=":
                    comparison = FunctionComparisonType.LessThanOrEqual;
                    break;
                case ">":
                    comparison = FunctionComparisonType.GreaterThan;
                    break;
                case "<":
                    comparison = FunctionComparisonType.LessThan;
                    break;
            }

            if (comparison == FunctionComparisonType.Undefined)
            {
                throw new ArgumentException($"{nameof(this.HasInstanceAttribute)}: Invalid {nameof(comparer)} value '{comparer}'.", nameof(comparer));
            }

            return this.ScriptApi.HasInstanceAttribute(thing, actualAttribute, comparison, value);
        }

        /// <summary>
        /// Checks if the thing is inside a house.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <returns>True if the thing is inside a house.</returns>
        public bool IsHouse(IThing thing)
        {
            return this.ScriptApi.IsHouse(thing);
        }

        /// <summary>
        /// Checks if the thing is in a house, and if given player is an owner of such house.
        /// </summary>
        /// <param name="thing">The thing to check.</param>
        /// <param name="user">The player.</param>
        /// <returns>True if the item is in a house and the player owns the house. False otherwise.</returns>
        public bool IsHouseOwner(IThing thing, IPlayer user)
        {
            return this.ScriptApi.IsHouseOwner(thing, user);
        }

        /// <summary>
        /// Generates a pseudo-random number between 1 and 100, and checks if the number is less than or equal to the supplied value.
        /// </summary>
        /// <param name="value">The value to compare against.</param>
        /// <returns>True if the random number generated is less than or equal to the value, false otherwise.</returns>
        public bool Random(byte value)
        {
            return this.ScriptApi.Random(value);
        }

        /// <summary>
        /// Creates an item of the given type at the given thing's location.
        /// </summary>
        /// <param name="atThing">The thing to create at.</param>
        /// <param name="typeId">The type of item to create.</param>
        /// <param name="effect">An optional effect to include with the creation.</param>
        public void Create(IThing atThing, ushort typeId, byte effect)
        {
            if (atThing == null)
            {
                return;
            }

            this.ScriptApi.CreateItemAtLocation(atThing.Location, typeId, effect);
        }

        /// <summary>
        /// Creates an item of the given type at the given location.
        /// </summary>
        /// <param name="location">The thing to create at.</param>
        /// <param name="typeId">The type of item to create.</param>
        /// <param name="effect">An optional effect to include with the creation.</param>
        public void CreateOnMap(Location location, ushort typeId, byte effect)
        {
            this.ScriptApi.CreateItemAtLocation(location, typeId, effect);
        }

        /// <summary>
        /// Changes an item from one type to another at a given location.
        /// </summary>
        /// <param name="location">The location in play.</param>
        /// <param name="fromItemId">The type from which to change.</param>
        /// <param name="toItemId">The type to change to.</param>
        /// <param name="limit">Limit the amount of items to change in this operation. 0 means no limit.</param>
        public void ChangeOnMap(Location location, ushort fromItemId, ushort toItemId, byte limit)
        {
            this.ScriptApi.ChangeItemAtLocation(location, fromItemId, toItemId, limit);
        }

        /// <summary>
        /// Displays an effect at the given thing's location.
        /// </summary>
        /// <param name="thing">The thing at play.</param>
        /// <param name="effect">The effect to display.</param>
        public void Effect(IThing thing, byte effect)
        {
            if (thing == null)
            {
                return;
            }

            this.ScriptApi.CreateAnimatedEffectAt(thing.Location, effect);
        }

        /// <summary>
        /// Displays an effect at the given location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="effect">The effect to display.</param>
        public void EffectOnMap(Location location, byte effect)
        {
            this.ScriptApi.CreateAnimatedEffectAt(location, effect);
        }

        /// <summary>
        /// Deletes a thing.
        /// </summary>
        /// <param name="thing">The thing to delete.</param>
        public void Delete(IThing thing)
        {
            this.ScriptApi.Delete(thing);
        }

        /// <summary>
        /// Deletes any amount of a given type at the given location.
        /// </summary>
        /// <param name="location">The location at which to delete.</param>
        /// <param name="typeId">The type to delete.</param>
        public void DeleteOnMap(Location location, ushort typeId)
        {
            this.ScriptApi.DeleteOnMap(location, typeId);
        }

        /// <summary>
        /// Places a monster at a given location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="monsterType">The type of monster to place.</param>
        public void MonsterOnMap(Location location, ushort monsterType)
        {
            this.ScriptApi.PlaceMonsterAt(location, monsterType);
        }

        /// <summary>
        /// Changes a given thing to an item of a given type.
        /// </summary>
        /// <param name="thing">The thing to change.</param>
        /// <param name="toTypeId">The type to change to.</param>
        /// <param name="effect">The effect to display as part of the change.</param>
        public void Change(ref IThing thing, ushort toTypeId, byte effect)
        {
            this.ScriptApi.ChangeItem(ref thing, toTypeId, effect);
        }

        /// <summary>
        /// Changes any amount of items of a given type to another.
        /// </summary>
        /// <param name="fromThing">A reference thing to get the location to change.</param>
        /// <param name="locationOffset">A location offset to apply to the thing's location and get the correct location.</param>
        /// <param name="fromTypeId">The type of item to change from.</param>
        /// <param name="toTypeId">The type of item to change to.</param>
        /// <param name="effect">An effect to display as part of the change.</param>
        public void ChangeRel(IThing fromThing, Location locationOffset, ushort fromTypeId, ushort toTypeId, byte effect)
        {
            if (fromThing == null)
            {
                return;
            }

            var targetLocation = fromThing.Location + locationOffset;

            this.ScriptApi.ChangeItemAtLocation(targetLocation, fromTypeId, toTypeId, effect);
        }

        /// <summary>
        /// Inflicts damage on behalf of an <paramref name="attacker"/>, to a <paramref name="victim"/>.
        /// </summary>
        /// <param name="attacker">The thing that is inflincting damage, if any.</param>
        /// <param name="victim">The thing receiving the damage.</param>
        /// <param name="damageType">The type of damage.</param>
        /// <param name="value">The initial value of the damage.</param>
        public void Damage(IThing attacker, IThing victim, byte damageType, ushort value)
        {
            this.ScriptApi.ApplyDamage(attacker, victim, damageType, value);
        }

        /// <summary>
        /// Logs a player out.
        /// </summary>
        /// <param name="player">The player.</param>
        public void Logout(IPlayer player)
        {
            this.ScriptApi.LogPlayerOut(player);
        }

        /// <summary>
        /// Moves a thing to a target location.
        /// </summary>
        /// <param name="thingToMove">The thing to move.</param>
        /// <param name="targetLocation">The target location.</param>
        public void Move(IThing thingToMove, Location targetLocation)
        {
            this.ScriptApi.MoveThingTo(thingToMove, targetLocation);
        }

        /// <summary>
        /// Moves the user to a location relative to the <paramref name="objectUsed"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="objectUsed">The object being used.</param>
        /// <param name="locationOffset">A location offset.</param>
        public void MoveRel(ICreature user, IThing objectUsed, Location locationOffset)
        {
            if (user == null || objectUsed == null)
            {
                return;
            }

            var targetLocation = objectUsed.Location + locationOffset;

            this.ScriptApi.MoveThingTo(user, targetLocation);
        }

        /// <summary>
        /// Moves everything (that's movable) at the location of a thing to a given target location.
        /// </summary>
        /// <param name="fromThing">The reference thing.</param>
        /// <param name="targetLocation">The location to move things to.</param>
        public void MoveTop(IThing fromThing, Location targetLocation)
        {
            if (fromThing == null)
            {
                return;
            }

            this.ScriptApi.MoveEverythingToLocation(fromThing.Location, targetLocation);
        }

        /// <summary>
        /// Moves the top thing in the stack of the <paramref name="fromThing"/>'s tile to the relative location off of it.
        /// </summary>
        /// <param name="fromThing">The reference <see cref="IThing"/> to move from.</param>
        /// <param name="locationOffset">The <see cref="Location"/> offset to move to.</param>
        public void MoveTopRel(IThing fromThing, Location locationOffset)
        {
            if (fromThing == null)
            {
                return;
            }

            var targetLocation = fromThing.Location + locationOffset;

            this.ScriptApi.MoveEverythingToLocation(fromThing.Location, targetLocation);
        }

        /// <summary>
        /// Moves any amount of items of a given type from one location to another.
        /// </summary>
        /// <param name="fromLocation">The location from which to move.</param>
        /// <param name="typeId">The item type to move.</param>
        /// <param name="toLocation">The location to move to.</param>
        public void MoveTopOnMap(Location fromLocation, ushort typeId, Location toLocation)
        {
            this.ScriptApi.MoveByIdToLocation(typeId, fromLocation, toLocation);
        }

        /// <summary>
        /// Displays an animated text at the given thing's location.
        /// </summary>
        /// <param name="fromThing">The thing.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="textType">The type of text to display.</param>
        public void Text(IThing fromThing, string text, byte textType)
        {
            if (fromThing == null)
            {
                return;
            }

            this.ScriptApi.DisplayAnimatedText(fromThing.Location, text, textType);
        }

        /// <summary>
        /// Writes text the player's name in the target thing's description's parameters.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="format">The format of the name to write.</param>
        /// <param name="targetThing">The target thing.</param>
        public void WriteName(IPlayer user, string format, IThing targetThing)
        {
            this.ScriptApi.WritePlayerNameOnThing(user, format, targetThing);
        }

        /// <summary>
        /// Sets a thing's starting location.
        /// </summary>
        /// <param name="thing">The thing to change starting location of.</param>
        /// <param name="newStartingLocation">The new starting location.</param>
        public void SetStart(IThing thing, Location newStartingLocation)
        {
            if (!(thing is IPlayer player))
            {
                return;
            }

            this.ScriptApi.ChangePlayerStartLocation(player, newStartingLocation);
        }
    }
}
