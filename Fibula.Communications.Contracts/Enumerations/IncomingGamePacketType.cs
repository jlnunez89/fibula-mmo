// -----------------------------------------------------------------
// <copyright file="IncomingGamePacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different incoming game server packet types.
    /// </summary>
    public enum IncomingGamePacketType : byte
    {
        /// <summary>
        /// Login packet.
        /// </summary>
        LogIn = 0x0A,

        /// <summary>
        /// Logout packet.
        /// </summary>
        LogOut = 0x14,

        /// <summary>
        /// A heartbeat response.
        /// </summary>
        HeartbeatResponse = 0x1D,

        /// <summary>
        /// A heartbeat request.
        /// </summary>
        Heartbeat = 0x1E,

        /// <summary>
        /// Move with autowalk.
        /// </summary>
        AutoMove = 0x64,

        /// <summary>
        /// Walking north.
        /// </summary>
        WalkNorth = 0x65,

        /// <summary>
        /// Walking east.
        /// </summary>
        WalkEast = 0x66,

        /// <summary>
        /// Walking south.
        /// </summary>
        WalkSouth = 0x67,

        /// <summary>
        /// Walking west.
        /// </summary>
        WalkWest = 0x68,

        /// <summary>
        /// Stop auto-walking.
        /// </summary>
        AutoMoveCancel = 0x69,

        /// <summary>
        /// Walking north east.
        /// </summary>
        WalkNortheast = 0x6A,

        /// <summary>
        /// Walking south east.
        /// </summary>
        WalkSoutheast = 0x6B,

        /// <summary>
        /// Walking south west.
        /// </summary>
        WalkSouthwest = 0x6C,

        /// <summary>
        /// Walking north west.
        /// </summary>
        WalkNorthwest = 0x6D,

        /// <summary>
        /// Turning north.
        /// </summary>
        TurnNorth = 0x6F,

        /// <summary>
        /// Turning east.
        /// </summary>
        TurnEast = 0x70,

        /// <summary>
        /// Turning south.
        /// </summary>
        TurnSouth = 0x71,

        /// <summary>
        /// Turning west.
        /// </summary>
        TurnWest = 0x72,

        /// <summary>
        /// Moving a thing.
        /// </summary>
        MoveThing = 0x78,

        /// <summary>
        /// Trade request.
        /// </summary>
        TradeRequest = 0x7D,

        /// <summary>
        /// Look within trade window.
        /// </summary>
        TradeLook = 0x7E,

        /// <summary>
        /// Accepting a trade.
        /// </summary>
        TradeAccept = 0x7F,

        /// <summary>
        /// Cancel a trade.
        /// </summary>
        TradeCancel = 0x80,

        /// <summary>
        /// Use an item.
        /// </summary>
        ItemUse = 0x82,

        /// <summary>
        /// Use an item on something else.
        /// </summary>
        ItemUseOn = 0x83,

        /// <summary>
        /// Use an item on a creature in the battle window.
        /// </summary>
        ItemUseThroughBattleWindow = 0x84,

        /// <summary>
        /// Rotate an item.
        /// </summary>
        ItemRotate = 0x85,

        /// <summary>
        /// Close a container.
        /// </summary>
        ContainerClose = 0x87,

        /// <summary>
        /// Navigate up in a container.
        /// </summary>
        ContainerUp = 0x88,

        /// <summary>
        /// Window with text.
        /// </summary>
        WindowText = 0x89,

        /// <summary>
        /// House window.
        /// </summary>
        WindowHouse = 0x8A,

        /// <summary>
        /// Look at something.
        /// </summary>
        LookAt = 0x8C,

        /// <summary>
        /// Look at a creature in the battle window.
        /// </summary>
        LookThroughBattleWindow = 0x8D,

        /// <summary>
        /// Speech.
        /// </summary>
        Speech = 0x96,

        /// <summary>
        /// Request channel list.
        /// </summary>
        ChannelListRequest = 0x97,

        /// <summary>
        /// Request openning a channel.
        /// </summary>
        ChannelOpen = 0x98,

        /// <summary>
        /// Request closing a channel.
        /// </summary>
        ChannelClose = 0x99,

        /// <summary>
        /// Request openning a direct channel.
        /// </summary>
        ChannelOpenDirect = 0x9A,

        /// <summary>
        /// Submit a report.
        /// </summary>
        ReportStart = 0x9B,

        /// <summary>
        /// Report closed by a gamemaster.
        /// </summary>
        ReportClose = 0x9C,

        /// <summary>
        /// Report cancelled by the player.
        /// </summary>
        ReportCancel = 0x9D,

        /// <summary>
        /// Changing fight or follow modes.
        /// </summary>
        ChangeModes = 0xA0,

        /// <summary>
        /// Attacking something.
        /// </summary>
        Attack = 0xA1,

        /// <summary>
        /// Following something.
        /// </summary>
        Follow = 0xA2,

        /// <summary>
        /// Invite to party.
        /// </summary>
        PartyInvite = 0xA3,

        /// <summary>
        /// Accept a party invite.
        /// </summary>
        PartyAcceptInvitation = 0xA4,

        /// <summary>
        /// Kick from party.
        /// </summary>
        PartyKick = 0xA5,

        /// <summary>
        /// Pass party leadership.
        /// </summary>
        PartyPassLeadership = 0xA6,

        /// <summary>
        /// Leave a party.
        /// </summary>
        PartyLeave = 0xA7,

        /// <summary>
        /// Create a private channel.
        /// </summary>
        ChannelCreatePrivate = 0xAA,

        /// <summary>
        /// Invite to private channel.
        /// </summary>
        ChannelInvite = 0xAB,

        /// <summary>
        /// Exclude from private channel.
        /// </summary>
        ChannelExclude = 0xAC,

        /// <summary>
        /// Stop all actions.
        /// </summary>
        StopAllActions = 0xBE,

        /// <summary>
        /// Request to re-send tile information.
        /// </summary>
        ResendTile = 0xC9,

        /// <summary>
        /// Request to re-send container information.
        /// </summary>
        /// <remarks>Happens when you store more than container max size.</remarks>
        ResendContainer = 0xCA,

        /// <summary>
        /// Request to find something in a container.
        /// </summary>
        FindInContainer = 0xCC,

        /// <summary>
        /// Request outfit change.
        /// </summary>
        StartOutfitChange = 0xD2,

        /// <summary>
        /// Submit outfit change.
        /// </summary>
        SubmitOutfitChange = 0xD3,

        /// <summary>
        /// Add a VIP.
        /// </summary>
        AddVip = 0xDC,

        /// <summary>
        /// Remove a VIP.
        /// </summary>
        RemoveVip = 0xDD,

        /// <summary>
        /// Bug report.
        /// </summary>
        ReportBug = 0xE6,

        /// <summary>
        /// Violation report.
        /// </summary>
        ReportViolation = 0xE7,

        /// <summary>
        /// Debug assertion report.
        /// </summary>
        ReportDebugAssertion = 0xE8,

        /// <summary>
        /// Wildcard.
        /// </summary>
        /// <remarks>Do not send.</remarks>
        Any = byte.MaxValue,
    }
}
