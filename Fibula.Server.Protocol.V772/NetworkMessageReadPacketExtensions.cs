// -----------------------------------------------------------------
// <copyright file="NetworkMessageReadPacketExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772
{
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Static class that defines extension methods which allow to read the packets in
    /// from an <see cref="INetworkMessage"/> in the way they are structured in this version.
    /// </summary>
    public static class NetworkMessageReadPacketExtensions
    {
        ///// <summary>
        ///// Reads the attack information from the network message.
        ///// </summary>
        ///// <param name="message">The message to read from.</param>
        ///// <returns>The attack information.</returns>
        //public static IAttackInfo ReadAttackInfo(this INetworkMessage message)
        //{
        //    return new AttackPacket(targetCreatureId: message.GetUInt32());
        //}

        ///// <summary>
        ///// Reads the automove directions information sent in the message.
        ///// </summary>
        ///// <param name="message">The message to read from.</param>
        ///// <returns>The automovement directions information.</returns>
        //public static IAutoMovementInfo ReadAutomovementInfo(this INetworkMessage message)
        //{
        //    var numberOfMovements = message.GetByte();

        //    var directions = new Direction[numberOfMovements];

        //    for (var i = 0; i < numberOfMovements; i++)
        //    {
        //        var dir = message.GetByte();

        //        directions[i] = dir switch
        //        {
        //            1 => Direction.East,
        //            2 => Direction.NorthEast,
        //            3 => Direction.North,
        //            4 => Direction.NorthWest,
        //            5 => Direction.West,
        //            6 => Direction.SouthWest,
        //            7 => Direction.South,
        //            8 => Direction.SouthEast,
        //            _ => throw new InvalidDataException($"Invalid direction value '{dir}' on message."),
        //        };
        //    }

        //    return new AutoMovePacket(directions);
        //}

        ///// <summary>
        ///// Reads the death information sent in the message.
        ///// </summary>
        ///// <param name="message">The message to read the information from.</param>
        ///// <returns>The death information for a character.</returns>
        // public static ICharacterDeathInfo ReadCharacterDeathInfo(this INetworkMessage message)
        // {
        //    return new CharacterDeathPacket(
        //        victimId: message.GetUInt32(),
        //        victimLevel: message.GetUInt16(),
        //        killerId: message.GetUInt32(),
        //        killerName: message.GetString(),
        //        wasUnjustified: message.GetByte() > 0,
        //        timestamp: DateTimeOffset.FromUnixTimeSeconds(message.GetUInt32()));
        // }

        ///// <summary>
        ///// Reads the container close information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The container close information.</returns>
        //public static IContainerInfo ReadContainerCloseInfo(this INetworkMessage message)
        //{
        //    return new ContainerCloseRequestPacket(containerId: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the container move up information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The container move up information.</returns>
        //public static IContainerInfo ReadContainerMoveUpInfo(this INetworkMessage message)
        //{
        //    return new ContainerMoveUpPacket(containerId: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the player list information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The player list information.</returns>
        // public static IOnlinePlayerListInfo ReadPlayerListInfo(this INetworkMessage message)
        // {
        //    var count = message.GetUInt16();

        // var playerList = new List<IOnlinePlayer>();

        // for (int i = 0; i < count; i++)
        //    {
        //        playerList.Add(new OnlinePlayer(
        //            name: message.GetString(),
        //            message.GetUInt16(),
        //            message.GetString()));
        //    }

        // return new CreatePlayerListPacket(playerList);
        // }

        ///// <summary>
        ///// Reads the debug assertion information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The debug assertion information.</returns>
        // public static IDebugAssertionInfo ReadDebugAssertionInfo(this INetworkMessage message)
        // {
        //    return new DebugAssertionPacket(
        //        assertionLine: message.GetString(),
        //        date: message.GetString(),
        //        description: message.GetString(),
        //        comment: message.GetString());
        // }

        ///// <summary>
        ///// Reads thing movement information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The thing movement information.</returns>
        //public static IThingMoveInfo ReadMoveThingInfo(this INetworkMessage message)
        //{
        //    return new ThingMovePacket(
        //        fromLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        thingClientId: message.GetUInt16(),
        //        fromStackPos: message.GetByte(),
        //        toLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        count: message.GetByte());
        //}

        ///// <summary>
        ///// Reads fight and chase modes information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The fight and chase modes information.</returns>
        //public static IModesInfo ReadModesInfo(this INetworkMessage message)
        //{
        //    // 1 - offensive, 2 - balanced, 3 - defensive
        //    var rawFightMode = message.GetByte();

        //    // 0 - stand while fightning, 1 - chase opponent
        //    var rawChaseMode = message.GetByte();

        //    // 0 - safe mode, 1 - free mode
        //    var rawSafeMode = message.GetByte();

        //    FightMode fightMode = FightMode.Balanced;
        //    ChaseMode chaseMode = ChaseMode.Stand;

        //    if (Enum.IsDefined(typeof(FightMode), rawFightMode))
        //    {
        //        fightMode = (FightMode)rawFightMode;
        //    }

        //    if (Enum.IsDefined(typeof(ChaseMode), rawChaseMode))
        //    {
        //        chaseMode = (ChaseMode)rawChaseMode;
        //    }

        //    return new ModesPacket(fightMode, chaseMode, isSafetyEnabled: rawSafeMode > 0);
        //}

        ///// <summary>
        ///// Reads the item used on information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The item used on information.</returns>
        //public static IUseItemOnInfo ReadItemUseOnInfo(this INetworkMessage message)
        //{
        //    return new UseItemOnPacket(
        //        fromLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        fromItemId: message.GetUInt16(),
        //        fromStackPos: message.GetByte(),
        //        toLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        toItemId: message.GetUInt16(),
        //        toStackPos: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the item use information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The item use information.</returns>
        //public static IUseItemInfo ReadItemUseInfo(this INetworkMessage message)
        //{
        //    return new UseItemPacket(
        //        fromLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        clientId: message.GetUInt16(),
        //        fromStackPos: message.GetByte(),
        //        index: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the item rotation information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The item use information.</returns>
        //public static IRotateItemInfo ReadItemRotationInfo(this INetworkMessage message)
        //{
        //    return new RotateItemPacket(
        //        fromLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        clientId: message.GetUInt16(),
        //        index: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the look at information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The look at information.</returns>
        //public static ILookAtInfo ReadLookAtInfo(this INetworkMessage message)
        //{
        //    return new LookAtPacket(
        //        location: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        thingId: message.GetUInt16(),
        //        stackPos: message.GetByte());
        //}

        ///// <summary>
        ///// Reads the management service player log in information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The player lofin information.</returns>
        // public static IManagementPlayerLoginInfo ReadManagementPlayerLoginInfo(this INetworkMessage message)
        // {
        //    return new ManagementPlayerLoginPacket(
        //        accountNumber: message.GetUInt32(),
        //        characterName: message.GetString(),
        //        password: message.GetString(),
        //        ipAddress: message.GetString());
        // }

        ///// <summary>
        ///// Reads the management plauer logout information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The player log out information.</returns>
        // public static IManagementPlayerLogoutInfo ReadManagementPlayerLogoutInfo(this INetworkMessage message)
        // {
        //    return new ManagementPlayerLogoutPacket(
        //        message.GetUInt32(),
        //        level: message.GetUInt16(),
        //        vocation: message.GetString(),
        //        residence: message.GetString(),
        //        lastLogin: DateTimeOffset.FromUnixTimeSeconds(message.GetUInt32()));
        // }

        ///// <summary>
        ///// Reads outfit information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The outfit information.</returns>
        // public static IOutfitInfo ReadOutfitInfo(this INetworkMessage message)
        // {
        //    Outfit outfit;

        // ushort lookType = message.GetUInt16();

        // if (lookType != 0)
        //    {
        //        outfit = new Outfit
        //        {
        //            Id = lookType,
        //            Head = message.GetByte(),
        //            Body = message.GetByte(),
        //            Legs = message.GetByte(),
        //            Feet = message.GetByte(),
        //        };
        //    }
        //    else
        //    {
        //        outfit = new Outfit
        //        {
        //            Id = lookType,
        //            LikeType = message.GetUInt16(),
        //        };
        //    }

        // return new OutfitChangedPacket(outfit);
        // }

        ///// <summary>
        ///// Reads rule violation information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The rule violation information.</returns>
        // public static IRuleViolationInfo ReadRuleViolationInfo(this INetworkMessage message)
        // {
        //    return new RuleViolationPacket(
        //        gamemasterId: message.GetUInt32(),
        //        characterName: message.GetString(),
        //        ipAddress: message.GetString(),
        //        reason: message.GetString(),
        //        comment: message.GetString());
        // }

        ///// <summary>
        ///// Reads speech information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The speech information.</returns>
        //public static ISpeechInfo ReadSpeechInfo(this INetworkMessage message)
        //{
        //    SpeechType type = (SpeechType)message.GetByte();

        //    switch (type)
        //    {
        //        case SpeechType.Private:
        //        // case SpeechType.PrivateRed:
        //        case SpeechType.RuleViolationAnswer:
        //            return new SpeechPacket(type, ChatChannelType.Private, receiver: message.GetString(), content: message.GetString());
        //        case SpeechType.ChannelYellow:
        //            // case SpeechType.ChannelRed:
        //            // case SpeechType.ChannelRedAnonymous:
        //            // case SpeechType.ChannelWhite:
        //            return new SpeechPacket(type, channelId: (ChatChannelType)message.GetUInt16(), content: message.GetString());
        //        default:
        //            return new SpeechPacket(type, channelId: ChatChannelType.None, content: message.GetString());
        //    }
        //}

        ///// <summary>
        ///// Reads statements list information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The statements list information.</returns>
        // public static IStatementListInfo ReadStatementListInfo(this INetworkMessage message)
        // {
        //    // read this unknown parts.
        //    message.GetUInt32();
        //    message.GetUInt32();

        // var count = message.GetUInt16();

        // var statements = new List<IStatement>();

        // for (int i = 0; i < count; i++)
        //    {
        //        var statementId = message.GetUInt32();
        //        var timestamp = DateTimeOffset.FromUnixTimeSeconds(message.GetUInt32());
        //        var playerId = message.GetUInt32();
        //        var channel = message.GetString();
        //        var msg = message.GetString();

        // statements.Add(new Statement(statementId, timestamp, playerId, channel, msg));
        //    }

        // return new StatementPacket(statements);
        // }
    }
}
