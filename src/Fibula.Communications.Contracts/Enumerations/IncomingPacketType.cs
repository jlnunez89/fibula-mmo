// -----------------------------------------------------------------
// <copyright file="IncomingPacketType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different incoming game server packet types.
    /// </summary>
    public enum IncomingPacketType : byte
    {
        /// <summary>
        /// Unsupported request type.
        /// </summary>
        Unsupported,

        /// <summary>
        /// Login packet.
        /// </summary>
        LogIn,

        /// <summary>
        /// Logout packet.
        /// </summary>
        LogOut,

        /// <summary>
        /// A heartbeat response.
        /// </summary>
        HeartbeatResponse,

        /// <summary>
        /// A heartbeat request.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Move with autowalk.
        /// </summary>
        AutoMove,

        /// <summary>
        /// Walking north.
        /// </summary>
        WalkNorth,

        /// <summary>
        /// Walking east.
        /// </summary>
        WalkEast,

        /// <summary>
        /// Walking south.
        /// </summary>
        WalkSouth,

        /// <summary>
        /// Walking west.
        /// </summary>
        WalkWest,

        /// <summary>
        /// Stop auto-walking.
        /// </summary>
        AutoMoveCancel,

        /// <summary>
        /// Walking north east.
        /// </summary>
        WalkNortheast,

        /// <summary>
        /// Walking south east.
        /// </summary>
        WalkSoutheast,

        /// <summary>
        /// Walking south west.
        /// </summary>
        WalkSouthwest,

        /// <summary>
        /// Walking north west.
        /// </summary>
        WalkNorthwest,

        /// <summary>
        /// Turning north.
        /// </summary>
        TurnNorth,

        /// <summary>
        /// Turning east.
        /// </summary>
        TurnEast,

        /// <summary>
        /// Turning south.
        /// </summary>
        TurnSouth,

        /// <summary>
        /// Turning west.
        /// </summary>
        TurnWest,

        /// <summary>
        /// Moving a thing.
        /// </summary>
        MoveThing,

        /// <summary>
        /// Trade request.
        /// </summary>
        TradeRequest,

        /// <summary>
        /// Look within trade window.
        /// </summary>
        TradeLook,

        /// <summary>
        /// Accepting a trade.
        /// </summary>
        TradeAccept,

        /// <summary>
        /// Cancel a trade.
        /// </summary>
        TradeCancel,

        /// <summary>
        /// Use an item.
        /// </summary>
        ItemUse,

        /// <summary>
        /// Use an item on something else.
        /// </summary>
        ItemUseOn,

        /// <summary>
        /// Use an item on a creature in the battle window.
        /// </summary>
        ItemUseThroughBattleWindow,

        /// <summary>
        /// Rotate an item.
        /// </summary>
        ItemRotate,

        /// <summary>
        /// Close a container.
        /// </summary>
        ContainerClose,

        /// <summary>
        /// Navigate up in a container.
        /// </summary>
        ContainerUp,

        /// <summary>
        /// Window with text.
        /// </summary>
        WindowText,

        /// <summary>
        /// House window.
        /// </summary>
        WindowHouse,

        /// <summary>
        /// Look at something.
        /// </summary>
        LookAt,

        /// <summary>
        /// Look at a creature in the battle window.
        /// </summary>
        LookThroughBattleWindow,

        /// <summary>
        /// Speech.
        /// </summary>
        Speech,

        /// <summary>
        /// Request channel list.
        /// </summary>
        ChannelListRequest,

        /// <summary>
        /// Request openning a channel.
        /// </summary>
        ChannelOpen,

        /// <summary>
        /// Request closing a channel.
        /// </summary>
        ChannelClose,

        /// <summary>
        /// Request openning a direct channel.
        /// </summary>
        ChannelOpenDirect,

        /// <summary>
        /// Submit a report.
        /// </summary>
        ReportStart,

        /// <summary>
        /// Report closed by a gamemaster.
        /// </summary>
        ReportClose,

        /// <summary>
        /// Report cancelled by the player.
        /// </summary>
        ReportCancel,

        /// <summary>
        /// Changing fight or follow modes.
        /// </summary>
        ChangeModes,

        /// <summary>
        /// Attacking something.
        /// </summary>
        Attack,

        /// <summary>
        /// Following something.
        /// </summary>
        Follow,

        /// <summary>
        /// Invite to party.
        /// </summary>
        PartyInvite,

        /// <summary>
        /// Accept a party invite.
        /// </summary>
        PartyAcceptInvitation,

        /// <summary>
        /// Kick from party.
        /// </summary>
        PartyKick,

        /// <summary>
        /// Pass party leadership.
        /// </summary>
        PartyPassLeadership,

        /// <summary>
        /// Leave a party.
        /// </summary>
        PartyLeave,

        /// <summary>
        /// Create a private channel.
        /// </summary>
        ChannelCreatePrivate,

        /// <summary>
        /// Invite to private channel.
        /// </summary>
        ChannelInvite,

        /// <summary>
        /// Exclude from private channel.
        /// </summary>
        ChannelExclude,

        /// <summary>
        /// Stop all actions.
        /// </summary>
        StopAllActions,

        /// <summary>
        /// Request to re-send tile information.
        /// </summary>
        ResendTile,

        /// <summary>
        /// Request to re-send container information.
        /// </summary>
        /// <remarks>Happens when you store more than container max size.</remarks>
        ResendContainer,

        /// <summary>
        /// Request to find something in a container.
        /// </summary>
        FindInContainer,

        /// <summary>
        /// Request outfit change.
        /// </summary>
        StartOutfitChange,

        /// <summary>
        /// Submit outfit change.
        /// </summary>
        SubmitOutfitChange,

        /// <summary>
        /// Add a VIP.
        /// </summary>
        AddVip,

        /// <summary>
        /// Remove a VIP.
        /// </summary>
        RemoveVip,

        /// <summary>
        /// Bug report.
        /// </summary>
        ReportBug,

        /// <summary>
        /// Violation report.
        /// </summary>
        ReportViolation,

        /// <summary>
        /// Debug assertion report.
        /// </summary>
        ReportDebugAssertion,

        /// <summary>
        /// A request for information about the game server.
        /// </summary>
        ServerStatus,
    }
}
