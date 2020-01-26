// -----------------------------------------------------------------
// <copyright file="IScriptApi.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Enumerations;

    public interface IScriptApi
    {
        IGame Game { get; set; }

        void ChangeItem(IThing thing, ushort toItemId, byte effect);

        void ChangeItemAtLocation(Location location, ushort fromItemId, ushort toItemId, byte effect);

        bool CompareCountItemsAt(Location location, FunctionComparisonType comparisonType, ushort value);

        void CreateItemAtLocation(Location location, ushort itemId, byte effect);

        void ApplyDamage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue);

        void Delete(IThing thing);

        void DeleteOnMap(Location location, ushort itemId);

        void CreateAnimatedEffectAt(Location location, byte effectByte);

        bool HasFlag(IThing itemThing, string flagStr);

        bool HasInstanceAttribute(IThing thing, ItemAttribute attribute, FunctionComparisonType comparisonType, ushort value);

        bool HasProfession(IThing thing, byte profesionId);

        bool HasRight(IPlayer user, string rightStr);

        bool IsCreature(IThing thing);

        bool IsDressed(IThing thing);

        bool IsHouse(IThing thing);

        bool IsHouseOwner(IThing thing, IPlayer user);

        bool IsObjectThere(Location location, ushort typeId);

        bool IsPlayer(IThing thing);

        bool IsPosition(IThing thing, Location location);

        bool IsType(IThing thing, ushort typeId);

        void LogPlayerOut(IPlayer user);

        bool MayLogout(IPlayer user);

        void PlaceMonsterAt(Location location, ushort monsterId);

        void MoveThingTo(IThing thingToMove, Location targetLocation);

        void MoveEverythingToLocation(Location fromLocation, Location targetLocation, params ushort[] exceptTypeIds);

        void MoveByIdToLocation(ushort itemId, Location fromLocation, Location toLocation);

        bool Random(byte value);

        void ChangePlayerStartLocation(IPlayer player, Location newLocation);

        void DisplayAnimatedText(Location location, string text, byte textType);

        void WritePlayerNameOnThing(IPlayer player, string format, IThing targetThing);

        void DescribeThingFor(IThing thingToDescribe, ICreature user);
    }
}