// -----------------------------------------------------------------
// <copyright file="IGameOperationsApi.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Contracts.Abstractions
{
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for the operations available in the game API.
    /// </summary>
    public interface IGameOperationsApi
    {
        //void ChangeItem(IThing thing, ushort toItemId, byte effect);

        //void ChangeItemAtLocation(Location location, ushort fromItemId, ushort toItemId, byte effect);

        //void ChangePlayerStartLocation(IPlayer player, Location newLocation);

        //bool CompareItemCountAt(Location location, FunctionComparisonType comparisonType, ushort value);

        //bool CompareItemAttribute(IThing thing, ItemAttribute attribute, FunctionComparisonType comparisonType, ushort value);

        //void CreateItemAtLocation(Location location, ushort itemId, byte effect);

        //void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue);

        //void Delete(IThing thing);

        //void DeleteOnMap(Location location, ushort itemId);

        //void DescribeFor(IThing thingToDescribe, ICreature user);

        //void DisplayAnimatedEffectAt(Location location, byte effectByte);

        //void DisplayAnimatedText(Location location, string text, byte textType);

        //bool HasAccessFlag(IPlayer player, string rightStr);

        //bool HasFlag(IThing itemThing, string flagStr);

        //bool HasProfession(IThing thing, byte profesionId);

        //bool IsAllowedToLogOut(IPlayer player);

        //bool IsAtLocation(IThing thing, Location location);

        //bool IsCreature(IThing thing);

        //bool IsDressed(IThing thing);

        //bool IsHouse(IThing thing);

        //bool IsHouseOwner(IThing thing, IPlayer user);

        //bool IsObjectThere(Location location, ushort typeId);

        //bool IsPlayer(IThing thing);

        //bool IsRandomNumberUnder(byte value, int maxValue = 100);

        //bool IsSpecificItem(IThing thing, ushort typeId);

        void LogPlayerIn(IPlayerCreationMetadata playerCreationMetadata);

        //void LogPlayerOut(IPlayer player);

        //void MoveTo(IThing thingToMove, Location targetLocation);

        //void MoveTo(ushort itemId, Location fromLocation, Location toLocation);

        //void MoveTo(Location fromLocation, Location targetLocation, params ushort[] exceptTypeIds);

        //void PlaceMonsterAt(Location location, ushort monsterId);

        //void TagThing(IPlayer player, string format, IThing targetThing);
    }
}