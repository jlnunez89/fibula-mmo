// -----------------------------------------------------------------
// <copyright file="StandardScriptApi.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Factories
{
    using System;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Enumerations;
    using Serilog;

    public class StandardScriptApi : IScriptApi
    {
        public StandardScriptApi(
            ILogger logger,
            ITileAccessor tileAccessor)
        {
            logger.ThrowIfNull(nameof(logger));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.Logger = logger.ForContext<StandardScriptApi>();
            this.TileAccessor = tileAccessor;
        }

        public IGame Game { get; set; }

        public ILogger Logger { get; }

        public ITileAccessor TileAccessor { get; }

        public bool CompareCountOf(IThing thing, FunctionComparisonType comparisonType, ushort value)
        {
            if (thing == null || !this.TileAccessor.GetTileAt(thing.Location, out ITile tile))
            {
                return false;
            }

            var count = tile.Ground != null && tile.Ground.ThingId == thing.ThingId ? 1 : 0;

            count += tile.Items.Count(i => i.ThingId == thing.ThingId);

            return comparisonType switch
            {
                FunctionComparisonType.Equal => count == value,
                FunctionComparisonType.GreaterThanOrEqual => count >= value,
                FunctionComparisonType.LessThanOrEqual => count <= value,
                FunctionComparisonType.GreaterThan => count > value,
                FunctionComparisonType.LessThan => count < value,

                _ => false,
            };
        }

        public bool IsCreature(IThing thing)
        {
            return thing is ICreature;
        }

        public bool IsType(IThing thing, ushort typeId)
        {
            return thing is IItem item && item.Type.TypeId == typeId;
        }

        public bool IsPosition(IThing thing, Location location)
        {
            return thing != null && thing.Location == location;
        }

        public bool IsPlayer(IThing thing)
        {
            return thing is IPlayer;
        }

        public bool IsObjectThere(Location location, ushort typeId)
        {
            return this.TileAccessor.GetTileAt(location, out ITile targetTile) && targetTile.HasItemWithId(typeId);
        }

        public bool HasRight(IPlayer user, string rightStr)
        {
            return true; // TODO: implement.
        }

        public bool MayLogout(IPlayer player)
        {
            return player.IsLogoutAllowed;
        }

        public bool HasFlag(IThing itemThing, string flagStr)
        {
            if (!(itemThing is IItem))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(flagStr))
            {
                return true;
            }

            return Enum.TryParse(flagStr, out ItemFlag parsedFlag) && ((IItem)itemThing).Type.Flags.Contains(parsedFlag);
        }

        public bool HasProfession(IThing thing, byte profesionId)
        {
            // TODO: implement professions.
            return thing != null && thing is IPlayer && false;
        }

        public bool HasInstanceAttribute(IThing thing, ItemAttribute attribute, FunctionComparisonType comparisonType, ushort value)
        {
            if (thing == null || !(thing is IItem thingAsItem))
            {
                return false;
            }

            if (!thingAsItem.Attributes.ContainsKey(attribute))
            {
                return false;
            }

            return comparisonType switch
            {
                FunctionComparisonType.Equal => Convert.ToUInt16(thingAsItem.Attributes[attribute]) == value,
                FunctionComparisonType.GreaterThanOrEqual => Convert.ToUInt16(thingAsItem.Attributes[attribute]) >= value,
                FunctionComparisonType.LessThanOrEqual => Convert.ToUInt16(thingAsItem.Attributes[attribute]) <= value,
                FunctionComparisonType.GreaterThan => Convert.ToUInt16(thingAsItem.Attributes[attribute]) > value,
                FunctionComparisonType.LessThan => Convert.ToUInt16(thingAsItem.Attributes[attribute]) < value,

                _ => false,
            };
        }

        public bool IsHouse(IThing thing)
        {
            // TODO: implement houses.
            return false; // thing?.Tile != null && thing.Tile.IsHouse;
        }

        public bool IsHouseOwner(IThing thing, IPlayer user)
        {
            // TODO: implement house ownership.
            return this.IsHouse(thing); // && thing.Tile.House.Owner == user.Name;
        }

        public bool Random(byte value)
        {
            // TODO: is this really bound to 100? or do we need more precision.
            return new Random().Next(100) <= value;
        }

        public void CreateItemAtLocation(Location location, ushort itemId, byte effect)
        {
            this.Game.ScriptRequest_CreateItemAt(location, itemId, effect);
        }

        public void ChangeItem(ref IThing thing, ushort toItemId, byte effect)
        {
            if (thing == null)
            {
                return;
            }

            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect > 0 && effect <= (byte)AnimatedEffect.SoundWhite)
            {
                animatedEffect = (AnimatedEffect)effect;
            }

            this.Game.ScriptRequest_ChangeItem(ref thing, toItemId, animatedEffect);
        }

        public void ChangeItemAtLocation(Location location, ushort fromItemId, ushort toItemId, byte effect)
        {
            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect > 0 && effect <= (byte)AnimatedEffect.SoundWhite)
            {
                animatedEffect = (AnimatedEffect)effect;
            }

            this.Game.ScriptRequest_ChangeItemAt(location, fromItemId, toItemId, animatedEffect);
        }

        public void CreateAnimatedEffectAt(Location location, byte effect)
        {
            AnimatedEffect animatedEffect = AnimatedEffect.None;

            if (effect > 0 && effect <= (byte)AnimatedEffect.SoundWhite)
            {
                animatedEffect = (AnimatedEffect)effect;
            }

            if (animatedEffect != AnimatedEffect.None)
            {
                this.Game.ScriptRequest_AddAnimatedEffectAt(location, animatedEffect);
            }
        }

        public void Delete(IThing thing)
        {
            if (thing == null || !(thing is IItem item))
            {
                return;
            }

            this.Game.ScriptRequest_DeleteItem(item);
        }

        public void DeleteOnMap(Location location, ushort itemId)
        {
            this.Game.ScriptRequest_DeleteItemAt(location, itemId);
        }

        public void PlaceMonsterAt(Location location, ushort monsterType)
        {
            this.Game.ScriptRequest_PlaceMonsterAt(location, monsterType);
        }

        public void ApplyDamage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
        {
            // TODO: implement correctly when combat is...
            //if (!(damagedThing is ICreature damagedCreature))
            //{
            //    return;
            //}

            //switch (damageSourceType)
            //{
            //    default: // physical
            //        break;
            //    case 2: // magic? or mana?
            //        break;
            //    case 4: // fire instant
            //        this.CreateAnimatedEffectAt(damagedThing, (byte)AnimatedEffect.Flame);
            //        break;
            //    case 8: // energy instant
            //        this.CreateAnimatedEffectAt(damagedThing, (byte)AnimatedEffect.DamageEnergy);
            //        break;
            //    case 16: // poison instant?
            //        this.CreateAnimatedEffectAt(damagedThing, (byte)AnimatedEffect.RingsGreen);
            //        break;
            //    case 32: // poison over time (poisoned condition)
            //        break;
            //    case 64: // fire over time (burned condition)
            //        break;
            //    case 128: // energy over time (electrified condition)
            //        break;
            //}
        }

        public void LogPlayerOut(IPlayer player)
        {
            this.Game.ScriptRequest_Logout(player); // TODO: force?
        }

        public void MoveThingTo(IThing thingToMove, Location targetLocation)
        {
            if (thingToMove == null)
            {
                return;
            }

            if (thingToMove is ICreature thingAsCreature)
            {
                this.Game.ScriptRequest_MoveCreature(thingAsCreature, targetLocation);
            }
            else if (thingToMove is IItem thingAsItem)
            {
                this.Game.ScriptRequest_MoveItemTo(thingAsItem, targetLocation);
            }
        }

        public void MoveEverythingToLocation(Location fromLocation, Location targetLocation, params ushort[] exceptTypeIds)
        {
            this.Game.ScriptRequest_MoveEverythingTo(fromLocation, targetLocation, exceptTypeIds);
        }

        public void MoveByIdToLocation(ushort itemId, Location fromLocation, Location toLocation)
        {
            this.Game.ScriptRequest_MoveItemOfTypeTo(itemId, fromLocation, toLocation);
        }

        public void DisplayAnimatedText(Location location, string text, byte textType)
        {
        }

        public void WritePlayerNameOnThing(IPlayer player, string format, IThing targetThing)
        {
            // TODO: implement.
        }

        public void ChangePlayerStartLocation(IPlayer player, Location newLocation)
        {
            // TODO: implement.
        }
    }
}
