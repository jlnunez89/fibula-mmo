// -----------------------------------------------------------------
// <copyright file="IncomingPacketTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming.Tests
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for packets that implement the <see cref="IIncomingPacket"/> interface.
    /// </summary>
    [TestClass]
    public class IncomingPacketTests
    {
        /// <summary>
        /// Checks <see cref="AttackPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void AttackPacket_Initialization()
        {
            const uint AnyCreatureId = 1u;

            IAttackInfo attackInfo = new AttackPacket(AnyCreatureId);

            Assert.AreEqual(AnyCreatureId, attackInfo.TargetCreatureId, $"Expected {nameof(attackInfo.TargetCreatureId)} to match {AnyCreatureId}.");
        }

        /// <summary>
        /// Checks <see cref="AutoMoveCancelPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void AutoMoveCancelPacket_Initialization()
        {
            const IncomingGamePacketType ExpectedPacketType = IncomingGamePacketType.AutoMoveCancel;

            IActionWithoutContentInfo actionWithoutContentInfo = new AutoMoveCancelPacket();

            Assert.AreEqual(ExpectedPacketType, actionWithoutContentInfo.Action, $"Expected {nameof(actionWithoutContentInfo.Action)} to match {ExpectedPacketType}.");
        }

        /// <summary>
        /// Checks <see cref="AutoMovePacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void AutoMovePacket_Initialization()
        {
            Direction[] expectedDirections = new Direction[] { Direction.East, Direction.North, Direction.West, Direction.South, };

            IAutoMovementInfo autoMoveInfo = new AutoMovePacket(expectedDirections);

            CollectionAssert.AreEqual(expectedDirections, autoMoveInfo.Directions, $"Expected {nameof(autoMoveInfo.Directions)} to match {expectedDirections}.");
        }

        /// <summary>
        /// Checks <see cref="DefaultReadPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void DefaultReadPacket_Initialization()
        {
            byte[] expectedBytes = new byte[] { 0x01, 0x03, 0x04, 0x10 };

            IBytesInfo bytesInfo = new DefaultReadPacket(expectedBytes);

            CollectionAssert.AreEqual(expectedBytes, bytesInfo.Bytes, $"Expected {nameof(bytesInfo.Bytes)} to match {bytesInfo}.");
        }

        /// <summary>
        /// Checks <see cref="GameLogInPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void GameLogInPacket_Initialization()
        {
            uint[] expectedKey = new uint[] { 4, 3, 2, 1 };
            const ushort ExpectedOsValue = 100;
            const ushort ExpectedVersionValue = 772;
            const bool IsGamemasterValue = true;
            const uint ExpectedAccountNumber = 987654;
            const string ExpectedCharname = "Potato";
            const string ExpectedPassword = "superSecurePwd";

            IGameLogInInfo gameLoginInfo = new GameLogInPacket(
                expectedKey,
                ExpectedOsValue,
                ExpectedVersionValue,
                IsGamemasterValue,
                ExpectedAccountNumber,
                ExpectedCharname,
                ExpectedPassword);

            CollectionAssert.AreEqual(expectedKey, gameLoginInfo.XteaKey, $"Expected {nameof(gameLoginInfo.XteaKey)} to match {expectedKey}.");
            Assert.AreEqual(ExpectedOsValue, gameLoginInfo.ClientOs, $"Expected {nameof(gameLoginInfo.ClientOs)} to match {ExpectedOsValue}.");
            Assert.AreEqual(ExpectedVersionValue, gameLoginInfo.ClientVersion, $"Expected {nameof(gameLoginInfo.ClientVersion)} to match {ExpectedVersionValue}.");
            Assert.AreEqual(IsGamemasterValue, gameLoginInfo.IsGamemaster, $"Expected {nameof(gameLoginInfo.IsGamemaster)} to match {IsGamemasterValue}.");
            Assert.AreEqual(ExpectedAccountNumber, gameLoginInfo.AccountNumber, $"Expected {nameof(gameLoginInfo.AccountNumber)} to match {ExpectedAccountNumber}.");
            Assert.AreEqual(ExpectedCharname, gameLoginInfo.CharacterName, $"Expected {nameof(gameLoginInfo.CharacterName)} to match {ExpectedCharname}.");
            Assert.AreEqual(ExpectedPassword, gameLoginInfo.Password, $"Expected {nameof(gameLoginInfo.Password)} to match {ExpectedPassword}.");
        }

        /// <summary>
        /// Checks <see cref="GameLogOutPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void GameLogOutPacket_Initialization()
        {
            const IncomingGamePacketType ExpectedPacketType = IncomingGamePacketType.LogOut;

            IActionWithoutContentInfo actionWithoutContentInfo = new GameLogOutPacket();

            Assert.AreEqual(ExpectedPacketType, actionWithoutContentInfo.Action, $"Expected {nameof(actionWithoutContentInfo.Action)} to match {ExpectedPacketType}.");
        }

        /// <summary>
        /// Checks <see cref="GatewayLogInPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void GatewayLogInPacket_Initialization()
        {
            uint[] expectedKey = new uint[] { 4, 3, 2, 1 };
            const ushort ExpectedOsValue = 100;
            const ushort ExpectedVersionValue = 772;
            const uint ExpectedAccountNumber = 987654;
            const string ExpectedPassword = "superSecurePwd";

            IGatewayLoginInfo gatewayLoginInfo = new GatewayLogInPacket(
                ExpectedVersionValue,
                ExpectedOsValue,
                expectedKey,
                ExpectedAccountNumber,
                ExpectedPassword);

            CollectionAssert.AreEqual(expectedKey, gatewayLoginInfo.XteaKey, $"Expected {nameof(gatewayLoginInfo.XteaKey)} to match {expectedKey}.");
            Assert.AreEqual(ExpectedOsValue, gatewayLoginInfo.ClientOs, $"Expected {nameof(gatewayLoginInfo.ClientOs)} to match {ExpectedOsValue}.");
            Assert.AreEqual(ExpectedVersionValue, gatewayLoginInfo.ClientVersion, $"Expected {nameof(gatewayLoginInfo.ClientVersion)} to match {ExpectedVersionValue}.");
            Assert.AreEqual(ExpectedAccountNumber, gatewayLoginInfo.AccountNumber, $"Expected {nameof(gatewayLoginInfo.AccountNumber)} to match {ExpectedAccountNumber}.");
            Assert.AreEqual(ExpectedPassword, gatewayLoginInfo.Password, $"Expected {nameof(gatewayLoginInfo.Password)} to match {ExpectedPassword}.");
        }

        /// <summary>
        /// Checks <see cref="HeartbeatPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void HeartbeatPacket_Initialization()
        {
            const IncomingGamePacketType ExpectedPacketType = IncomingGamePacketType.Heartbeat;

            IActionWithoutContentInfo actionWithoutContentInfo = new HeartbeatPacket();

            Assert.AreEqual(ExpectedPacketType, actionWithoutContentInfo.Action, $"Expected {nameof(actionWithoutContentInfo.Action)} to match {ExpectedPacketType}.");
        }

        /// <summary>
        /// Checks <see cref="HeartbeatResponsePacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void HeartbeatResponsePacket_Initialization()
        {
            const IncomingGamePacketType ExpectedPacketType = IncomingGamePacketType.HeartbeatResponse;

            IActionWithoutContentInfo actionWithoutContentInfo = new HeartbeatResponsePacket();

            Assert.AreEqual(ExpectedPacketType, actionWithoutContentInfo.Action, $"Expected {nameof(actionWithoutContentInfo.Action)} to match {ExpectedPacketType}.");
        }

        /// <summary>
        /// Checks <see cref="LookAtPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void LookAtPacket_Initialization()
        {
            const ushort ExpectedThingId = 1027;
            const byte ExpectedStackPosition = 1;

            Location expectedLocation = new Location()
            {
                X = 100,
                Y = 150,
                Z = 7,
            };

            ILookAtInfo lookAtInfo = new LookAtPacket(expectedLocation, ExpectedThingId, ExpectedStackPosition);

            Assert.AreEqual(ExpectedThingId, lookAtInfo.ThingId, $"Expected {nameof(lookAtInfo.ThingId)} to match {ExpectedThingId}.");
            Assert.AreEqual(ExpectedStackPosition, lookAtInfo.StackPosition, $"Expected {nameof(lookAtInfo.StackPosition)} to match {ExpectedStackPosition}.");
            Assert.AreEqual(expectedLocation, lookAtInfo.Location, $"Expected {nameof(lookAtInfo.Location)} to match {expectedLocation}.");
        }

        /// <summary>
        /// Checks <see cref="ModesPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void ModesPacket_Initialization()
        {
            const FightMode ExpectedFightMode = FightMode.Balanced;
            const ChaseMode ExpectedChaseMode = ChaseMode.KeepDistance;
            const bool ExpectedSafetyModeValue = true;

            IModesInfo modesInfo = new ModesPacket(ExpectedFightMode, ExpectedChaseMode, ExpectedSafetyModeValue);

            Assert.AreEqual(ExpectedFightMode, modesInfo.FightMode, $"Expected {nameof(modesInfo.FightMode)} to match {ExpectedFightMode}.");
            Assert.AreEqual(ExpectedChaseMode, modesInfo.ChaseMode, $"Expected {nameof(modesInfo.ChaseMode)} to match {ExpectedChaseMode}.");
            Assert.AreEqual(ExpectedSafetyModeValue, modesInfo.SafeModeOn, $"Expected {nameof(modesInfo.SafeModeOn)} to match {ExpectedSafetyModeValue}.");
        }

        /// <summary>
        /// Checks <see cref="SpeechPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void SpeechPacket_Initialization()
        {
            const SpeechType ExpectedSpeechType = SpeechType.Say;
            const ChatChannelType ExpectedChannelType = ChatChannelType.Game;
            const string ExpectedContent = "this is a test!";
            const string ExpectedReceiver = "Player 2";

            ISpeechInfo speechInfo = new SpeechPacket(ExpectedSpeechType, ExpectedChannelType, ExpectedContent);

            Assert.AreEqual(ExpectedSpeechType, speechInfo.SpeechType, $"Expected {nameof(speechInfo.SpeechType)} to match {ExpectedSpeechType}.");
            Assert.AreEqual(ExpectedChannelType, speechInfo.ChannelType, $"Expected {nameof(speechInfo.ChannelType)} to match {ExpectedChannelType}.");
            Assert.AreEqual(ExpectedContent, speechInfo.Content, $"Expected {nameof(speechInfo.Content)} to match {ExpectedContent}.");
            Assert.AreEqual(string.Empty, speechInfo.Receiver, $"Expected {nameof(speechInfo.Receiver)} to be empty.");

            ISpeechInfo speechInfoWithReceiver = new SpeechPacket(ExpectedSpeechType, ExpectedChannelType, ExpectedContent, ExpectedReceiver);

            Assert.AreEqual(ExpectedSpeechType, speechInfoWithReceiver.SpeechType, $"Expected {nameof(speechInfoWithReceiver.SpeechType)} to match {ExpectedSpeechType}.");
            Assert.AreEqual(ExpectedChannelType, speechInfoWithReceiver.ChannelType, $"Expected {nameof(speechInfoWithReceiver.ChannelType)} to match {ExpectedChannelType}.");
            Assert.AreEqual(ExpectedContent, speechInfoWithReceiver.Content, $"Expected {nameof(speechInfoWithReceiver.Content)} to match {ExpectedContent}.");
            Assert.AreEqual(ExpectedReceiver, speechInfoWithReceiver.Receiver, $"Expected {nameof(speechInfoWithReceiver.Receiver)} to match {ExpectedReceiver}.");
        }

        /// <summary>
        /// Checks <see cref="StopAllActionsPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void StopAllActionsPacket_Initialization()
        {
            const IncomingGamePacketType ExpectedPacketType = IncomingGamePacketType.StopAllActions;

            IActionWithoutContentInfo actionWithoutContentInfo = new StopAllActionsPacket();

            Assert.AreEqual(ExpectedPacketType, actionWithoutContentInfo.Action, $"Expected {nameof(actionWithoutContentInfo.Action)} to match {ExpectedPacketType}.");
        }

        /// <summary>
        /// Checks <see cref="ThingMovePacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void ThingMovePacket_Initialization()
        {
            const ushort ExpectedThingId = 1027;
            const byte ExpectedStackPosition = 6;
            const byte ExpectedCount = 1;

            Location expectedFromLocation = new Location()
            {
                X = 100,
                Y = 150,
                Z = 7,
            };

            Location expectedToLocation = new Location()
            {
                X = 101,
                Y = 150,
                Z = 7,
            };

            IThingMoveInfo thingMoveInfo = new ThingMovePacket(expectedFromLocation, ExpectedThingId, ExpectedStackPosition, expectedToLocation, ExpectedCount);

            Assert.AreEqual(ExpectedThingId, thingMoveInfo.ThingClientId, $"Expected {nameof(thingMoveInfo.ThingClientId)} to match {ExpectedThingId}.");
            Assert.AreEqual(expectedFromLocation, thingMoveInfo.FromLocation, $"Expected {nameof(thingMoveInfo.FromLocation)} to match {expectedFromLocation}.");
            Assert.AreEqual(ExpectedStackPosition, thingMoveInfo.FromStackPos, $"Expected {nameof(thingMoveInfo.FromStackPos)} to match {ExpectedStackPosition}.");
            Assert.AreEqual(expectedToLocation, thingMoveInfo.ToLocation, $"Expected {nameof(thingMoveInfo.ToLocation)} to match {expectedToLocation}.");
            Assert.AreEqual(ExpectedCount, thingMoveInfo.Amount, $"Expected {nameof(thingMoveInfo.Amount)} to match {ExpectedCount}.");
        }

        /// <summary>
        /// Checks <see cref="TurnOnDemandPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void TurnOnDemandPacket_Initialization()
        {
            const Direction ExpectedDirection = Direction.South;

            ITurnOnDemandInfo turnInfo = new TurnOnDemandPacket(ExpectedDirection);

            Assert.AreEqual(ExpectedDirection, turnInfo.Direction, $"Expected {nameof(turnInfo.Direction)} to match {ExpectedDirection}.");
        }

        /// <summary>
        /// Checks <see cref="WalkOnDemandPacket"/> initialization.
        /// </summary>
        [TestMethod]
        public void WalkOnDemandPacket_Initialization()
        {
            const Direction ExpectedDirection = Direction.West;

            IWalkOnDemandInfo walkInfo = new WalkOnDemandPacket(ExpectedDirection);

            Assert.AreEqual(ExpectedDirection, walkInfo.Direction, $"Expected {nameof(walkInfo.Direction)} to match {ExpectedDirection}.");
        }
    }
}