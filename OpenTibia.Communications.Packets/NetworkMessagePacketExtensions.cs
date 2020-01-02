// -----------------------------------------------------------------
// <copyright file="NetworkMessagePacketExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets
{
    using System;
    using System.IO;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Static class that defines extension methods for <see cref="INetworkMessage"/>, which allow it to
    /// read and write the packets contained in this assembly.
    /// </summary>
    public static class NetworkMessagePacketExtensions
    {
        /// <summary>
        /// Reads the account login information off of the network message.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The account login information.</returns>
        public static IAccountLoginInfo ReadAccountLoginInfo(this INetworkMessage message)
        {
            return new AccountLoginPacket(
                xteaKey: new uint[]
                {
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                },
                accountNumber: message.GetUInt32(),
                password: message.GetString());
        }

        ///// <summary>
        ///// Reads the attack information from the network message.
        ///// </summary>
        ///// <param name="message">The message to read from.</param>
        ///// <returns>The attack information.</returns>
        // public static IAttackInfo ReadAttackInfo(this INetworkMessage message)
        // {
        //    return new AttackPacket(targetCreatureId: message.GetUInt32());
        // }

        /// <summary>
        /// Reads authentication info from the network message.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The authentication information.</returns>
        public static IAuthenticationInfo ReadAuthenticationInfo(this INetworkMessage message)
        {
            // one byte to read and ignore.
            message.GetByte();

            return new AuthenticationPacket(password: message.GetString(), worldName: message.GetString());
        }

        /// <summary>
        /// Reads the automove directions information sent in the message.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The automovement directions information.</returns>
        public static IAutoMovementInfo ReadAutomovementInfo(this INetworkMessage message)
        {
            var numberOfMovements = message.GetByte();

            var directions = new Direction[numberOfMovements];

            for (var i = 0; i < numberOfMovements; i++)
            {
                var dir = message.GetByte();

                directions[i] = dir switch
                {
                    1 => Direction.East,
                    2 => Direction.NorthEast,
                    3 => Direction.North,
                    4 => Direction.NorthWest,
                    5 => Direction.West,
                    6 => Direction.SouthWest,
                    7 => Direction.South,
                    8 => Direction.SouthEast,
                    _ => throw new InvalidDataException($"Invalid direction value '{dir}' on message."),
                };
            }

            return new AutoMovePacket(directions);
        }

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
        // public static IContainerInfo ReadContainerCloseInfo(this INetworkMessage message)
        // {
        //    return new ContainerCloseRequestPacket(containerId: message.GetByte());
        // }

        ///// <summary>
        ///// Reads the container move up information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The container move up information.</returns>
        // public static IContainerInfo ReadContainerMoveUpInfo(this INetworkMessage message)
        // {
        //    return new ContainerMoveUpPacket(containerId: message.GetByte());
        // }

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

        /// <summary>
        /// Reads default information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The default formatted information.</returns>
        public static IDefaultInfo ReadDefaultInfo(this INetworkMessage message)
        {
            return new DefaultReadPacket(message.GetBytes(message.Length - message.Cursor));
        }

        /// <summary>
        /// Reads thing movement information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The thing movement information.</returns>
        public static IThingMoveInfo ReadMoveThingInfo(this INetworkMessage message)
        {
            return new ThingMovePacket(
                fromLocation: new Location
                {
                    X = message.GetUInt16(),
                    Y = message.GetUInt16(),
                    Z = (sbyte)message.GetByte(),
                },
                thingClientId: message.GetUInt16(),
                fromStackPos: message.GetByte(),
                toLocation: new Location
                {
                    X = message.GetUInt16(),
                    Y = message.GetUInt16(),
                    Z = (sbyte)message.GetByte(),
                },
                count: message.GetByte());
        }

        ///// <summary>
        ///// Reads the item used on information sent in the message.
        ///// </summary>
        ///// <param name="message">The mesage to read the information from.</param>
        ///// <returns>The item used on information.</returns>
        // public static IItemUseOnInfo ReadItemUseOnInfo(this INetworkMessage message)
        // {
        //    return new ItemUseOnPacket(
        //        fromLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        fromSpriteId: message.GetUInt16(),
        //        fromStackPos: message.GetByte(),
        //        toLocation: new Location
        //        {
        //            X = message.GetUInt16(),
        //            Y = message.GetUInt16(),
        //            Z = (sbyte)message.GetByte(),
        //        },
        //        toSpriteId: message.GetUInt16(),
        //        toStackPos: message.GetByte());
        // }

        /// <summary>
        /// Reads the item use information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The item use information.</returns>
        public static IUseItemInfo ReadItemUseInfo(this INetworkMessage message)
        {
            return new UseItemPacket(
                fromLocation: new Location
                {
                    X = message.GetUInt16(),
                    Y = message.GetUInt16(),
                    Z = (sbyte)message.GetByte(),
                },
                clientId: message.GetUInt16(),
                fromStackPos: message.GetByte(),
                index: message.GetByte());
        }

        /// <summary>
        /// Reads the look at information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The look at information.</returns>
        public static ILookAtInfo ReadLookAtInfo(this INetworkMessage message)
        {
            return new LookAtPacket(
                location: new Location
                {
                    X = message.GetUInt16(),
                    Y = message.GetUInt16(),
                    Z = (sbyte)message.GetByte(),
                },
                thingId: message.GetUInt16(),
                stackPos: message.GetByte());
        }

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

        /// <summary>
        /// Reads new connection information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The new connection information.</returns>
        public static INewConnectionInfo ReadNewConnectionInfo(this INetworkMessage message)
        {
            var packet = new NewConnectionPacket(operatingSystem: message.GetUInt16(), version: message.GetUInt16());

            // unknown bytes.
            message.SkipBytes(12);

            return packet;
        }

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

        /// <summary>
        /// Reads login information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The login information.</returns>
        public static ICharacterLoginInfo ReadCharacterLoginInfo(this INetworkMessage message)
        {
            return new CharacterLoginPacket(
                xteaKey: new uint[]
                {
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                },
                operatingSystem: message.GetUInt16(),
                version: message.GetUInt16(),
                isGamemaster: message.GetByte() > 0,
                accountNumber: message.GetUInt32(),
                characterName: message.GetString(),
                password: message.GetString());
        }

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
        // public static ISpeechInfo ReadSpeechInfo(this INetworkMessage message)
        // {
        //    SpeechType type = (SpeechType)message.GetByte();
        //    Speech speech;

        // switch (type)
        //    {
        //        case SpeechType.Private:
        //        // case SpeechType.PrivateRed:
        //        case SpeechType.RuleViolationAnswer:
        //            speech = new Speech
        //            {
        //                Type = type,
        //                Receiver = message.GetString(),
        //                Message = message.GetString(),
        //            };
        //            break;
        //        case SpeechType.ChannelYellow:
        //            // case SpeechType.ChannelRed:
        //            // case SpeechType.ChannelRedAnonymous:
        //            // case SpeechType.ChannelWhite:
        //            speech = new Speech
        //            {
        //                Type = type,
        //                ChannelId = (ChatChannelType)message.GetUInt16(),
        //                Message = message.GetString(),
        //            };
        //            break;
        //        default:
        //            speech = new Speech
        //            {
        //                Type = type,
        //                Message = message.GetString(),
        //            };
        //            break;
        //    }

        // return new SpeechPacket(speech);
        // }

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

        /// <summary>
        /// Writes the contents of the <see cref="AddCreaturePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteAddCreaturePacket(this INetworkMessage message, AddCreaturePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Creature.Location);
            message.AddCreature(packet.Creature, packet.AsKnown, packet.RemoveThisCreatureId);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="AddItemPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteAddItemPacket(this INetworkMessage message, AddItemPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddLocation(packet.Location);
        //    message.AddItem(packet.Item);
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="AnimatedTextPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteAnimatedTextPacket(this INetworkMessage message, AnimatedTextPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddLocation(packet.Location);
        //    message.AddByte((byte)packet.Color);
        //    message.AddString(packet.Text);
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="AuthenticationResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteAuthenticationResultPacket(this INetworkMessage message, AuthenticationResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte((byte)(packet.HadError ? 0x01 : 0x00));
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="BanismentResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteBanismentResultPacket(this INetworkMessage message, BanismentResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte(packet.BanDays);
        //    message.AddUInt32(packet.BanishedUntil);
        //    message.AddByte(0x00);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="CharacterListPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteCharacterListPacket(this INetworkMessage message, CharacterListPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte((byte)packet.Characters.Count());

            foreach (ICharacterListItem character in packet.Characters)
            {
                message.AddString(character.Name);
                message.AddString(character.World);
                message.AddBytes(character.Ip);
                message.AddUInt16(character.Port);
            }

            message.AddUInt16(packet.PremiumDaysLeft);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="ClearOnlinePlayersResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteClearOnlinePlayersResultPacket(this INetworkMessage message, ClearOnlinePlayersResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt16(packet.ClearedCount);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="ContainerAddItemPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteContainerAddItemPacket(this INetworkMessage message, ContainerAddItemPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.ContainerId);
            message.AddItem(packet.Item);
        }

        /// <summary>
        /// Writes the contents of the <see cref="ContainerClosePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteContainerClosePacket(this INetworkMessage message, ContainerClosePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.ContainerId);
        }

        /// <summary>
        /// Writes the contents of the <see cref="ContainerOpenPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteContainerOpenPacket(this INetworkMessage message, ContainerOpenPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.ContainerId);
            message.AddUInt16(packet.ClientItemId);
            message.AddString(packet.Name);
            message.AddByte(packet.Volume);
            message.AddByte(Convert.ToByte(packet.HasParent ? 0x01 : 0x00));
            message.AddByte(Convert.ToByte(packet.Contents.Count));

            foreach (var item in packet.Contents)
            {
                message.AddItem(item);
            }
        }

        /// <summary>
        /// Writes the contents of the <see cref="ContainerRemoveItemPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteContainerRemoveItemPacket(this INetworkMessage message, ContainerRemoveItemPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.ContainerId);
            message.AddByte(packet.Index);
        }

        /// <summary>
        /// Writes the contents of the <see cref="ContainerUpdateItemPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteContainerUpdateItemPacket(this INetworkMessage message, ContainerUpdateItemPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.ContainerId);
            message.AddByte(packet.Index);
            message.AddItem(packet.Item);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatePlayerListResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatePlayerListResultPacket(this INetworkMessage message, CreatePlayerListResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.WritePacketType(packet);

        //    message.AddByte((byte)(packet.IsNewRecord ? 0xFF : 0x00));
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatureChangedOutfitPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatureChangedOutfitPacket(this INetworkMessage message, CreatureChangedOutfitPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.WritePacketType(packet);

        //    message.AddUInt32(packet.Creature.Id);

        //    message.AddUInt16(packet.Creature.Outfit.Id);

        //    if (packet.Creature.Outfit.Id != 0)
        //    {
        //        message.AddByte(packet.Creature.Outfit.Head);
        //        message.AddByte(packet.Creature.Outfit.Body);
        //        message.AddByte(packet.Creature.Outfit.Legs);
        //        message.AddByte(packet.Creature.Outfit.Feet);
        //    }
        //    else
        //    {
        //        message.AddUInt16(packet.Creature.Outfit.LikeType);
        //    }
        //}

        /// <summary>
        /// Writes the contents of the <see cref="CreatureLightPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteCreatureLightPacket(this INetworkMessage message, CreatureLightPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddUInt32(packet.Creature.Id);
            message.AddByte(packet.Creature.EmittedLightLevel); // light level
            message.AddByte(packet.Creature.EmittedLightColor); // color
        }

        /// <summary>
        /// Writes the contents of the <see cref="CreatureMovedPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteCreatureMovedPacket(this INetworkMessage message, CreatureMovedPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.FromLocation);
            message.AddByte(packet.FromStackpos);
            message.AddLocation(packet.ToLocation);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatureSpeechPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteCreatureSpeechPacket(this INetworkMessage message, CreatureSpeechPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt32(0);
        //    message.AddString(packet.SenderName);
        //    message.AddByte((byte)packet.SpeechType);

        // switch (packet.SpeechType)
        //    {
        //        case SpeechType.Normal:
        //        case SpeechType.Whisper:
        //        case SpeechType.Yell:
        //        case SpeechType.MonsterSay:
        //            // case SpeechType.MonsterYell:
        //            message.AddLocation(packet.Location);
        //            break;
        //        // case SpeechType.ChannelRed:
        //        // case SpeechType.ChannelRedAnonymous:
        //        // case SpeechType.ChannelOrange:
        //        case SpeechType.ChannelYellow:
        //            // case SpeechType.ChannelWhite:
        //            message.AddUInt16((ushort)packet.ChannelId);
        //            break;
        //        case SpeechType.RuleViolationReport:
        //            message.AddUInt32(packet.Time);
        //            break;
        //        default:
        //            break;
        //    }

        // message.AddString(packet.Text);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="CreatureTurnedPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteCreatureTurnedPacket(this INetworkMessage message, CreatureTurnedPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Creature.Location);
            message.AddByte(packet.StackPosition);
            message.AddUInt16(packet.Creature.ThingId);
            message.AddUInt32(packet.Creature.Id);
            message.AddByte((byte)packet.Creature.Direction);
        }

        /// <summary>
        /// Writes the contents of the <see cref="DefaultErrorPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteDefaultErrorPacket(this INetworkMessage message, DefaultErrorPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);
        }

        /// <summary>
        /// Writes the contents of the <see cref="DefaultNoErrorPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteDefaultNoErrorPacket(this INetworkMessage message, DefaultNoErrorPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="AuctionsResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteAuctionsResultPacket(this INetworkMessage message, AuctionsResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt16((ushort)packet.AuctionResults.Count);

        // foreach (var house in packet.AuctionResults)
        //    {
        //        message.AddUInt16(house.HouseId);
        //        message.AddUInt32(house.WinnerId);
        //        message.AddString(house.WinnerName);
        //        message.AddUInt32(house.Bid);
        //    }
        // }

        /// <summary>
        /// Writes the contents of the <see cref="GameServerDisconnectPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteGameServerDisconnectPacket(this INetworkMessage message, GameServerDisconnectPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddString(packet.Reason);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="InventoryClearSlotPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteInventoryClearSlotPacket(this INetworkMessage message, InventoryClearSlotPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte((byte)packet.Slot);
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="InventorySetSlotPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteInventorySetSlotPacket(this INetworkMessage message, InventorySetSlotPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte((byte)packet.Slot);
        //    message.AddItem(packet.Item);
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="LoadPlayersResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteLoadPlayersResultPacket(this INetworkMessage message, LoadPlayersResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt32((uint)packet.LoadedPlayers.Count);

        // foreach (var player in packet.LoadedPlayers)
        //    {
        //        message.AddString(player.CharacterName);
        //        message.AddUInt32(player.AccountId);
        //    }
        // }

        /// <summary>
        /// Writes the contents of the <see cref="LoginServerDisconnectPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteLoginServerDisconnectPacket(this INetworkMessage message, LoginServerDisconnectPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddString(packet.Reason);
        }

        /// <summary>
        /// Writes the contents of the <see cref="MagicEffectPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteMagicEffectPacket(this INetworkMessage message, MagicEffectPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Location);
            message.AddByte((byte)packet.Effect);
        }

        /// <summary>
        /// Writes the contents of the <see cref="MapDescriptionPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteMapDescriptionPacket(this INetworkMessage message, MapDescriptionPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Origin);

            message.AddBytes(packet.DescriptionBytes);
        }

        /// <summary>
        /// Writes the contents of the <see cref="MapPartialDescriptionPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteMapPartialDescriptionPacket(this INetworkMessage message, MapPartialDescriptionPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddBytes(packet.DescriptionBytes);
        }

        /// <summary>
        /// Writes the contents of the <see cref="MessageOfTheDayPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteMessageOfTheDayPacket(this INetworkMessage message, MessageOfTheDayPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddString("1\n" + packet.MessageOfTheDay);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="NotationResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteNotationResultPacket(this INetworkMessage message, NotationResultPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt32(packet.GamemasterId);
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerChooseOutfitPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WritePlayerChooseOutfitPacket(this INetworkMessage message, PlayerChooseOutfitPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt16(packet.CurrentOutfit.Id);

        // if (packet.CurrentOutfit.Id != 0)
        //    {
        //        message.AddByte(packet.CurrentOutfit.Head);
        //        message.AddByte(packet.CurrentOutfit.Body);
        //        message.AddByte(packet.CurrentOutfit.Legs);
        //        message.AddByte(packet.CurrentOutfit.Feet);
        //    }
        //    else
        //    {
        //        message.AddUInt16(packet.CurrentOutfit.LikeType);
        //    }

        // message.AddUInt16(packet.ChooseFromId);
        //    message.AddUInt16(packet.ChooseToId);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="PlayerConditionsPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WritePlayerConditionsPacket(this INetworkMessage message, PlayerConditionsPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            // TODO: implement contidions.
            message.AddByte(0x00);
        }

        /// <summary>
        /// Writes the contents of the <see cref="PlayerInventoryPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WritePlayerInventoryPacket(this INetworkMessage message, PlayerInventoryPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            // message.WritePacketType(packet);
            var addInventoryItem = new Action<Slot>(slot =>
            {
                if (packet.Player.Inventory[(byte)slot] == null)
                {
                    message.AddByte((byte)Communications.Contracts.Enumerations.OutgoingGamePacketType.InventoryEmpty);
                    message.AddByte((byte)slot);
                }
                else
                {
                    message.AddByte((byte)Communications.Contracts.Enumerations.OutgoingGamePacketType.InventoryItem);
                    message.AddByte((byte)slot);
                    message.AddItem(packet.Player.Inventory[(byte)slot]);
                }
            });

            addInventoryItem(Slot.Head);
            addInventoryItem(Slot.Neck);
            addInventoryItem(Slot.Back);
            addInventoryItem(Slot.Body);
            addInventoryItem(Slot.RightHand);
            addInventoryItem(Slot.LeftHand);
            addInventoryItem(Slot.Legs);
            addInventoryItem(Slot.Feet);
            addInventoryItem(Slot.Ring);
            addInventoryItem(Slot.Ammo);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerLoginRejectionPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WritePlayerLoginRejectionPacket(this INetworkMessage message, PlayerLoginRejectionPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte(packet.Reason);
        //    message.AddByte(0xFF); // EOM ?
        // }

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerLoginSucessPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WritePlayerLoginSucessPacket(this INetworkMessage message, PlayerLoginSucessPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddUInt32((uint)packet.AccountId);
        //    message.AddString(packet.CharacterName);
        //    message.AddByte(packet.Gender);

        // message.AddString(packet.Guild ?? string.Empty);
        //    message.AddString(string.IsNullOrWhiteSpace(packet.Guild) || string.IsNullOrWhiteSpace(packet.GuildTitle) ? "0" : packet.GuildTitle);
        //    message.AddString(string.IsNullOrWhiteSpace(packet.Guild) || string.IsNullOrWhiteSpace(packet.PlayerTitle) ? "0" : packet.PlayerTitle);

        // message.AddByte((byte)(packet.VipContacts == null ? 0 : Math.Min(packet.VipContacts.Count, 100)));

        // foreach (var contact in packet.VipContacts.Take(100))
        //    {
        //        message.AddUInt32(contact.ContactId);
        //        message.AddString(contact.ContactName);
        //    }

        // message.AddByte((byte)(packet.Privileges == null ? 0 : Math.Min(packet.Privileges.Count, 255)));

        // foreach (var privString in packet.Privileges.Take(255))
        //    {
        //        message.AddString(privString);
        //    }

        // message.AddByte((byte)(packet.RecentlyActivatedPremmium ? 0x01 : 0x00));
        // }

        /// <summary>
        /// Writes the contents of the <see cref="PlayerSkillsPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WritePlayerSkillsPacket(this INetworkMessage message, PlayerSkillsPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.NoWeapon].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.NoWeapon));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Club].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Club));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Sword].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Sword));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Axe].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Axe));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Ranged].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Ranged));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Shield].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Shield));

            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Fishing].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Fishing));
        }

        /// <summary>
        /// Writes the contents of the <see cref="PlayerStatsPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WritePlayerStatsPacket(this INetworkMessage message, PlayerStatsPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.Hitpoints));
            message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.MaxHitpoints));
            message.AddUInt16(Convert.ToUInt16(packet.Player.CarryStrength));

            message.AddUInt32(Math.Min(0x7FFFFFFF, Convert.ToUInt32(packet.Player.Skills[SkillType.Experience].Count))); // Experience: Client debugs after 2,147,483,647 exp

            message.AddUInt16(packet.Player.Skills[SkillType.Experience].Level);
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Experience));
            message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.Manapoints));
            message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.MaxManapoints));
            message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Magic].Level));
            message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Magic));

            message.AddByte(packet.Player.SoulPoints);
        }

        /// <summary>
        /// Writes the contents of the <see cref="PlayerWalkCancelPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WritePlayerWalkCancelPacket(this INetworkMessage message, PlayerWalkCancelPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte((byte)packet.ResultingDirection);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="ProjectilePacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteProjectilePacket(this INetworkMessage message, ProjectilePacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddLocation(packet.FromLocation);
        //    message.AddLocation(packet.ToLocation);
        //    message.AddByte((byte)packet.ShootType);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="RemoveAtStackposPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteRemoveAtStackposPacket(this INetworkMessage message, RemoveAtStackposPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Location);
            message.AddByte(packet.Stackpos);
        }

        /// <summary>
        /// Writes the contents of the <see cref="SelfAppearPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteSelfAppearPacket(this INetworkMessage message, SelfAppearPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddUInt32(packet.CreatureId);
            message.AddByte(packet.GraphicsSpeed);
            message.AddByte(packet.CanReportBugs);

            message.AddByte(Math.Min((byte)0x01, packet.Player.PermissionsLevel));

            if (packet.Player.PermissionsLevel > 0)
            {
                // TODO: WTF are these, permissions flags?
                message.AddByte(0x0B);

                for (var i = 0; i < 32; i++)
                {
                    message.AddByte(0xFF);
                }
            }
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="ServerStatusPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteServerStatusPacket(this INetworkMessage message, ServerStatusPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // // message.WritePacketType(packet);

        // message.AddBytes(packet.Data.ToByteArray());
        // }

        /// <summary>
        /// Writes the contents of the <see cref="TextMessagePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteTextMessagePacket(this INetworkMessage message, TextMessagePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte((byte)packet.Type);
            message.AddString(packet.Message);
        }

        /// <summary>
        /// Writes the contents of the <see cref="UpdateTilePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteUpdateTilePacket(this INetworkMessage message, UpdateTilePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddLocation(packet.Location);

            if (packet.DescriptionBytes.Length > 0)
            {
                message.AddBytes(packet.DescriptionBytes);
                message.AddByte(0x00); // skip count
            }
            else
            {
                message.AddByte(0x01); // skip count
            }

            message.AddByte(0xFF);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="WorldConfigPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        // public static void WriteWorldConfigPacket(this INetworkMessage message, WorldConfigPacket packet)
        // {
        //    packet.ThrowIfNull(nameof(packet));

        // message.WritePacketType(packet);

        // message.AddByte(packet.WorldType);
        //    message.AddByte(packet.DailyResetHour);

        // message.AddBytes(packet.IpAddressBytes);
        //    message.AddUInt16(packet.Port);

        // message.AddUInt16(packet.MaximumTotalPlayers);
        //    message.AddUInt16(packet.PremiumMainlandBuffer);
        //    message.AddUInt16(packet.MaximumRookgardians);
        //    message.AddUInt16(packet.PremiumRookgardiansBuffer);
        // }

        /// <summary>
        /// Writes the contents of the <see cref="WorldLightPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteWorldLightPacket(this INetworkMessage message, WorldLightPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.WritePacketType(packet);

            message.AddByte(packet.Level);
            message.AddByte(packet.Color);
        }

        /// <summary>
        /// Writes the supplied packet's type as a byte into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet which's type will be added.</param>
        private static void WritePacketType(this INetworkMessage message, IOutgoingPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddByte(packet.PacketType);
        }
    }
}
