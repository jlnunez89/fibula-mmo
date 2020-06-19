// -----------------------------------------------------------------
// <copyright file="NetworkMessageWritePacketExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol772
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;

    /// <summary>
    /// Static class that defines extension methods which allow to write the packets in
    /// to an <see cref="INetworkMessage"/> in the way they are structured in this version.
    /// </summary>
    public static class NetworkMessageWritePacketExtensions
    {
        /// <summary>
        /// Writes the contents of the <see cref="AddCreaturePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteAddCreaturePacket(this INetworkMessage message, AddCreaturePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddLocation(packet.Creature.Location);
            message.AddCreature(packet.Creature, packet.AsKnown, packet.RemoveThisCreatureId);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="AddItemPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteAddItemPacket(this INetworkMessage message, AddItemPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddLocation(packet.Location);
        //    message.AddItem(packet.Item);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="AnimatedTextPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteAnimatedTextPacket(this INetworkMessage message, AnimatedTextPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddLocation(packet.Location);
        //    message.AddByte((byte)packet.Color);
        //    message.AddString(packet.Text);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="AuthenticationResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteAuthenticationResultPacket(this INetworkMessage message, AuthenticationResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)(packet.HadError ? 0x01 : 0x00));
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="BanismentResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteBanismentResultPacket(this INetworkMessage message, BanismentResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.BanDays);
        //    message.AddUInt32(packet.BanishedUntil);
        //    message.AddByte(0x00);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="CharacterListPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCharacterListPacket(this INetworkMessage message, CharacterListPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)packet.Characters.Count());

        //    foreach (CharacterInfo character in packet.Characters)
        //    {
        //        message.AddString(character.Name);
        //        message.AddString(character.World);
        //        message.AddBytes(character.Ip);
        //        message.AddUInt16(character.Port);
        //    }

        //    message.AddUInt16(packet.PremiumDaysLeft);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ClearOnlinePlayersResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteClearOnlinePlayersResultPacket(this INetworkMessage message, ClearOnlinePlayersResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt16(packet.ClearedCount);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ContainerAddItemPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteContainerAddItemPacket(this INetworkMessage message, ContainerAddItemPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.ContainerId);
        //    message.AddItem(packet.Item);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ContainerClosePacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteContainerClosePacket(this INetworkMessage message, ContainerClosePacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.ContainerId);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ContainerOpenPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteContainerOpenPacket(this INetworkMessage message, ContainerOpenPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.ContainerId);
        //    message.AddUInt16(packet.ClientItemId);
        //    message.AddString(packet.Name);
        //    message.AddByte(packet.Volume);
        //    message.AddByte(Convert.ToByte(packet.HasParent ? 0x01 : 0x00));
        //    message.AddByte(Convert.ToByte(packet.Contents.Count));

        //    foreach (var item in packet.Contents)
        //    {
        //        message.AddItem(item);
        //    }
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ContainerRemoveItemPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteContainerRemoveItemPacket(this INetworkMessage message, ContainerRemoveItemPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.ContainerId);
        //    message.AddByte(packet.Index);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ContainerUpdateItemPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteContainerUpdateItemPacket(this INetworkMessage message, ContainerUpdateItemPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.ContainerId);
        //    message.AddByte(packet.Index);
        //    message.AddItem(packet.Item);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatePlayerListResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatePlayerListResultPacket(this INetworkMessage message, CreatePlayerListResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

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

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatureLightPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatureLightPacket(this INetworkMessage message, CreatureLightPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32(packet.Creature.Id);
        //    message.AddByte(packet.Creature.EmittedLightLevel);
        //    message.AddByte(packet.Creature.EmittedLightColor);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatureMovedPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatureMovedPacket(this INetworkMessage message, CreatureMovedPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddLocation(packet.FromLocation);
        //    message.AddByte(packet.FromStackpos);
        //    message.AddLocation(packet.ToLocation);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="CreatureSpeechPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatureSpeechPacket(this INetworkMessage message, CreatureSpeechPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32(0);
        //    message.AddString(packet.SenderName);
        //    message.AddByte((byte)packet.SpeechType);

        //    switch (packet.SpeechType)
        //    {
        //        case SpeechType.Say:
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

        //    message.AddString(packet.Text);
        //}

        /// <summary>
        /// Writes the contents of the <see cref="CreatureTurnedPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteCreatureTurnedPacket(this INetworkMessage message, CreatureTurnedPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddLocation(packet.Creature.Location);
            message.AddByte(packet.StackPosition);
            message.AddUInt16(packet.Creature.ThingId);
            message.AddUInt32(packet.Creature.Id);
            message.AddByte((byte)packet.Creature.Direction);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="AuctionsResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteAuctionsResultPacket(this INetworkMessage message, AuctionsResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt16((ushort)packet.AuctionResults.Count);

        //    foreach (var house in packet.AuctionResults)
        //    {
        //        message.AddUInt16(house.HouseId);
        //        message.AddUInt32(house.WinnerId);
        //        message.AddString(house.WinnerName);
        //        message.AddUInt32(house.Bid);
        //    }
        //}

        /// <summary>
        /// Writes the contents of the <see cref="GameServerDisconnectPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteGameServerDisconnectPacket(this INetworkMessage message, GameServerDisconnectPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddString(packet.Reason);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerInventoryClearSlotPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteInventoryClearSlotPacket(this INetworkMessage message, PlayerInventoryClearSlotPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)packet.Slot);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerInventorySetSlotPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteInventorySetSlotPacket(this INetworkMessage message, PlayerInventorySetSlotPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)packet.Slot);
        //    message.AddItem(packet.Item);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="LoadPlayersResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteLoadPlayersResultPacket(this INetworkMessage message, LoadPlayersResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32((uint)packet.LoadedPlayers.Count);

        //    foreach (var player in packet.LoadedPlayers)
        //    {
        //        message.AddString(player.CharacterName);
        //        message.AddUInt32(player.AccountId);
        //    }
        //}

        /// <summary>
        /// Writes the contents of the <see cref="GatewayServerDisconnectPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteLoginServerDisconnectPacket(this INetworkMessage message, GatewayServerDisconnectPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

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

            message.AddLocation(packet.Location);
            message.AddByte((byte)packet.Effect);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="SquarePacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteCreatureSquarePacket(this INetworkMessage message, SquarePacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32(packet.OnCreatureId);
        //    message.AddByte((byte)packet.Color);
        //}

        /// <summary>
        /// Writes the contents of the <see cref="MapDescriptionPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteMapDescriptionPacket(this INetworkMessage message, MapDescriptionPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

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

            message.AddString("1\n" + packet.MessageOfTheDay);
        }

        ///// <summary>
        ///// Writes the contents of the <see cref="NotationResultPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteNotationResultPacket(this INetworkMessage message, NotationResultPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32(packet.GamemasterId);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerChooseOutfitPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerChooseOutfitPacket(this INetworkMessage message, PlayerChooseOutfitPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt16(packet.CurrentOutfit.Id);

        //    if (packet.CurrentOutfit.Id != 0)
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

        //    message.AddUInt16(packet.ChooseFromId);
        //    message.AddUInt16(packet.ChooseToId);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerConditionsPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerConditionsPacket(this INetworkMessage message, PlayerConditionsPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    // TODO: implement contidions.
        //    message.AddByte(0x00);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerInventoryPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerInventoryPacket(this INetworkMessage message, PlayerInventoryPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    var addInventoryItem = new Action<Slot>(slot =>
        //    {
        //        var slotContainer = packet.Player.Inventory[(byte)slot] as IContainerItem;

        //        var itemInContainer = slotContainer?.Content.FirstOrDefault();

        //        if (itemInContainer == null)
        //        {
        //            message.AddByte((byte)GameResponseType.InventoryEmpty);
        //            message.AddByte((byte)slot);
        //        }
        //        else
        //        {
        //            message.AddByte((byte)GameResponseType.InventoryItem);
        //            message.AddByte((byte)slot);
        //            message.AddItem(itemInContainer);
        //        }
        //    });

        //    addInventoryItem(Slot.Head);
        //    addInventoryItem(Slot.Neck);
        //    addInventoryItem(Slot.Back);
        //    addInventoryItem(Slot.Body);
        //    addInventoryItem(Slot.RightHand);
        //    addInventoryItem(Slot.LeftHand);
        //    addInventoryItem(Slot.Legs);
        //    addInventoryItem(Slot.Feet);
        //    addInventoryItem(Slot.Ring);
        //    addInventoryItem(Slot.Ammo);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerLoginRejectionPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerLoginRejectionPacket(this INetworkMessage message, PlayerLoginRejectionPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.Reason);
        //    message.AddByte(0xFF); // EOM ?
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerLoginSucessPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerLoginSucessPacket(this INetworkMessage message, PlayerLoginSucessPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt32((uint)packet.AccountId);
        //    message.AddString(packet.CharacterName);
        //    message.AddByte(packet.Gender);

        //    message.AddString(packet.Guild ?? string.Empty);
        //    message.AddString(string.IsNullOrWhiteSpace(packet.Guild) || string.IsNullOrWhiteSpace(packet.GuildTitle) ? "0" : packet.GuildTitle);
        //    message.AddString(string.IsNullOrWhiteSpace(packet.Guild) || string.IsNullOrWhiteSpace(packet.PlayerTitle) ? "0" : packet.PlayerTitle);

        //    message.AddByte((byte)(packet.VipContacts == null ? 0 : Math.Min(packet.VipContacts.Count, 100)));

        //    foreach (var contact in packet.VipContacts.Take(100))
        //    {
        //        message.AddUInt32(contact.ContactId);
        //        message.AddString(contact.ContactName);
        //    }

        //    message.AddByte((byte)(packet.Privileges == null ? 0 : Math.Min(packet.Privileges.Count, 255)));

        //    foreach (var privString in packet.Privileges.Take(255))
        //    {
        //        message.AddString(privString);
        //    }

        //    message.AddByte((byte)(packet.RecentlyActivatedPremmium ? 0x01 : 0x00));
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerSkillsPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerSkillsPacket(this INetworkMessage message, PlayerSkillsPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.NoWeapon].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.NoWeapon));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Club].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Club));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Sword].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Sword));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Axe].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Axe));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Ranged].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Ranged));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Shield].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Shield));

        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Fishing].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Fishing));
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerStatsPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerStatsPacket(this INetworkMessage message, PlayerStatsPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.Hitpoints));
        //    message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.MaxHitpoints));
        //    message.AddUInt16(Convert.ToUInt16(packet.Player.CarryStrength));

        //    // Experience: 7.7x Client debugs after 0x7FFFFFFF (2,147,483,647) exp
        //    message.AddUInt32(Math.Min(0x7FFFFFFF, Convert.ToUInt32(packet.Player.Skills[SkillType.Experience].Count)));

        //    message.AddUInt16((ushort)Math.Min(1, Math.Min(ushort.MaxValue, packet.Player.Skills[SkillType.Experience].Level)));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Experience));
        //    message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.Manapoints));
        //    message.AddUInt16(Math.Min(ushort.MaxValue, packet.Player.MaxManapoints));
        //    message.AddByte((byte)Math.Min(byte.MaxValue, packet.Player.Skills[SkillType.Magic].Level));
        //    message.AddByte(packet.Player.CalculateSkillPercent(SkillType.Magic));

        //    message.AddByte(packet.Player.SoulPoints);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="PlayerWalkCancelPacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WritePlayerWalkCancelPacket(this INetworkMessage message, PlayerWalkCancelPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte((byte)packet.ResultingDirection);
        //}

        ///// <summary>
        ///// Writes the contents of the <see cref="ProjectilePacket"/> into the message.
        ///// </summary>
        ///// <param name="message">The message to write to.</param>
        ///// <param name="packet">The packet to write in the message.</param>
        //public static void WriteProjectilePacket(this INetworkMessage message, ProjectilePacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddLocation(packet.FromLocation);
        //    message.AddLocation(packet.ToLocation);
        //    message.AddByte((byte)packet.ShootType);
        //}

        /// <summary>
        /// Writes the contents of the <see cref="RemoveAtPositionPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteRemoveAtStackposPacket(this INetworkMessage message, RemoveAtPositionPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

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
        //public static void WriteServerStatusPacket(this INetworkMessage message, ServerStatusPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddBytes(packet.Data.ToByteArray());
        //}

        /// <summary>
        /// Writes the contents of the <see cref="TextMessagePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteTextMessagePacket(this INetworkMessage message, TextMessagePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddByte((byte)packet.Type);
            message.AddString(packet.Message);
        }

        /// <summary>
        /// Writes the contents of the <see cref="TileUpdatePacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteTileUpdatePacket(this INetworkMessage message, TileUpdatePacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

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
        //public static void WriteWorldConfigPacket(this INetworkMessage message, WorldConfigPacket packet)
        //{
        //    packet.ThrowIfNull(nameof(packet));

        //    message.AddByte(packet.WorldType);
        //    message.AddByte(packet.DailyResetHour);

        //    message.AddBytes(packet.IpAddressBytes);
        //    message.AddUInt16(packet.Port);

        //    message.AddUInt16(packet.MaximumTotalPlayers);
        //    message.AddUInt16(packet.PremiumMainlandBuffer);
        //    message.AddUInt16(packet.MaximumRookgardians);
        //    message.AddUInt16(packet.PremiumRookgardiansBuffer);
        //}

        /// <summary>
        /// Writes the contents of the <see cref="WorldLightPacket"/> into the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="packet">The packet to write in the message.</param>
        public static void WriteWorldLightPacket(this INetworkMessage message, WorldLightPacket packet)
        {
            packet.ThrowIfNull(nameof(packet));

            message.AddByte(packet.Level);
            message.AddByte(packet.Color);
        }
    }
}
