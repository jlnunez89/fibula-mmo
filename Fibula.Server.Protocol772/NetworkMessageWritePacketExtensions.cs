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
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Static class that defines extension methods which allow to write the packets in
    /// to an <see cref="INetworkMessage"/> in the way they are structured in this version.
    /// </summary>
    public static class NetworkMessageWritePacketExtensions
    {
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
    }
}
