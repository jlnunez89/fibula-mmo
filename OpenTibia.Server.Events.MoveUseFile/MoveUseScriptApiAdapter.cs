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
        /// Counts the occurrences of the <paramref name="thingToCount"/>'s id in it's own location.
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

            return this.ScriptApi.CountOfThingComparison(thingToCount, comparison, value);
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

        public bool IsType(IThing thing, ushort typeId)
        {
            return this.ScriptApi.IsType(thing, typeId);
        }

        public bool IsPosition(IThing thing, Location location)
        {
            return this.ScriptApi.IsPosition(thing, location);
        }

        public bool IsPlayer(IThing thing)
        {
            return this.ScriptApi.IsPlayer(thing);
        }

        public bool IsObjectThere(Location location, ushort typeId)
        {
            return this.ScriptApi.IsObjectThere(location, typeId);
        }

        public bool HasRight(IPlayer user, string rightStr)
        {
            return this.ScriptApi.HasRight(user, rightStr);
        }

        public bool MayLogout(IPlayer player)
        {
            return this.ScriptApi.MayLogout(player);
        }

        public bool HasFlag(IThing itemThing, string flagStr)
        {
            return this.ScriptApi.HasFlag(itemThing, flagStr);
        }

        public bool HasProfession(IThing thing, byte professionId)
        {
            return this.ScriptApi.HasProfession(thing, professionId);
        }

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

        public bool IsHouse(IThing thing)
        {
            return this.ScriptApi.IsHouse(thing);
        }

        public bool IsHouseOwner(IThing thing, IPlayer user)
        {
            return this.ScriptApi.IsHouseOwner(thing, user);
        }

        public bool Random(byte value)
        {
            return this.ScriptApi.Random(value);
        }

        public void Create(IThing atThing, ushort itemId, byte effect)
        {
            if (atThing == null)
            {
                return;
            }

            this.ScriptApi.CreateItemAtLocation(atThing.Location, itemId);
        }

        public void CreateOnMap(Location location, ushort itemId, byte effect)
        {
            this.ScriptApi.CreateItemAtLocation(location, itemId);
        }

        public void ChangeOnMap(Location location, ushort fromItemId, ushort toItemId, byte unknown)
        {
            this.ScriptApi.ChangeItemAtLocation(location, fromItemId, toItemId, unknown);
        }

        public void Effect(IThing thing, byte effectByte)
        {
            if (thing == null)
            {
                return;
            }

            this.ScriptApi.CreateAnimatedEffectAt(thing.Location, effectByte);
        }

        public void EffectOnMap(Location location, byte effectByte)
        {
            this.ScriptApi.CreateAnimatedEffectAt(location, effectByte);
        }

        public void Delete(IThing thing)
        {
            this.ScriptApi.Delete(thing);
        }

        public void DeleteOnMap(Location location, ushort itemId)
        {
            this.ScriptApi.DeleteOnMap(location, itemId);
        }

        public void MonsterOnMap(Location location, ushort monsterId)
        {
            this.ScriptApi.PlaceMonsterAt(location, monsterId);
        }

        public void Change(ref IThing thing, ushort toItemId, byte unknown)
        {
            this.ScriptApi.ChangeItem(ref thing, toItemId, unknown);
        }

        public void ChangeRel(IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte effect)
        {
            if (fromThing == null)
            {
                return;
            }

            var targetLocation = fromThing.Location + locationOffset;

            this.ScriptApi.ChangeItemAtLocation(targetLocation, fromItemId, toItemId, effect);
        }

        public void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
        {
            this.ScriptApi.ApplyDamage(damagingThing, damagedThing, damageSourceType, damageValue);
        }

        public void Logout(IPlayer user)
        {
            this.ScriptApi.LogPlayerOut(user);
        }

        public void Move(IThing thingToMove, Location targetLocation)
        {
            this.ScriptApi.MoveThingTo(thingToMove, targetLocation);
        }

        public void MoveRel(ICreature user, IThing objectUsed, Location locationOffset)
        {
            if (user == null || objectUsed == null)
            {
                return;
            }

            var targetLocation = objectUsed.Location + locationOffset;

            this.ScriptApi.MoveThingTo(user, targetLocation);
        }

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

        public void MoveTopOnMap(Location fromLocation, ushort itemId, Location toLocation)
        {
            this.ScriptApi.MoveByIdToLocation(itemId, fromLocation, toLocation);
        }

        public void Text(IThing fromThing, string text, byte textType)
        {
            if (fromThing == null)
            {
                return;
            }

            this.ScriptApi.DisplayAnimatedText(fromThing.Location, text, textType);
        }

        public void WriteName(IPlayer user, string format, IThing targetThing)
        {
            this.ScriptApi.WritePlayerNameOnThing(user, format, targetThing);
        }

        public void SetStart(IThing thing, Location location)
        {
            if (!(thing is IPlayer player))
            {
                return;
            }

            this.ScriptApi.ChangePlayerStartLocation(player, location);
        }
    }
}
