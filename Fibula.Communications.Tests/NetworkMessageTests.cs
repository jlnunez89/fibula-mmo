// -----------------------------------------------------------------
// <copyright file="NetworkMessageTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Tests
{
    using System;
    using System.Buffers;
    using System.Linq;
    using System.Text;
    using Fibula.Common.TestingUtilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the <see cref="NetworkMessage"/> class.
    /// </summary>
    [TestClass]
    public class NetworkMessageTests
    {
        private const int DefaultTestBufferIncomingMessageCursor = 23;
        private const int DefaultTestBufferIncomingMessageLength = 23;
        private const int DefaultTestBufferOutgoingMessageCursor = 4;
        private const int DefaultTestBufferOutgoingMessageLength = 4;

        private const string DefaultTestBuffer = "This is a test message.";
        private static readonly byte[] DefaultTestBufferAsBytes = Encoding.ASCII.GetBytes(DefaultTestBuffer);

        /// <summary>
        /// Checks the <see cref="NetworkMessage"/> initialization.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_Initialization()
        {
            const int ExpectedStartingInboundLength = 0;
            const int ExpectedStartingInboundPos = 0;
            const int ExpectedStartingOutbounLength = 4;
            const int ExpectedStartingOutboundPos = 4;

            var inboundMessage = new NetworkMessage(isOutbound: false);
            var outboundMessage = new NetworkMessage();

            Assert.IsNotNull(inboundMessage);
            Assert.AreEqual(ExpectedStartingInboundLength, inboundMessage.Length);
            Assert.AreEqual(ExpectedStartingInboundPos, inboundMessage.Cursor);

            Assert.IsNotNull(outboundMessage);
            Assert.AreEqual(ExpectedStartingOutbounLength, outboundMessage.Length);
            Assert.AreEqual(ExpectedStartingOutboundPos, outboundMessage.Cursor);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.ReadAsBytesInfo"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_ReadAsBytesInfoTest()
        {
            INetworkMessage inboundMessage = this.SetupInboundMessage();
            IBytesInfo resultInfo = inboundMessage.ReadAsBytesInfo();

            Assert.IsNotNull(resultInfo?.Bytes);
            CollectionAssert.AreEqual(DefaultTestBufferAsBytes, resultInfo.Bytes, "The bytes don't match the expected value.");
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.Resize"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_ResizeTest()
        {
            const int ExpectedSize = 1;
            const int ExpectedCursorBeforeResize = DefaultTestBufferIncomingMessageCursor;
            const int ExpectedLengthBeforeResize = DefaultTestBufferIncomingMessageLength;
            const int ExpectedCursorAfterResize = 0;
            const int ExpectedLengthAfterResize = ExpectedSize;
            const int ExpectedCursorAfterResize2 = ExpectedLengthBeforeResize;

            INetworkMessage testMessage1 = this.SetupInboundMessage(resetCursor: false);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(ExpectedCursorBeforeResize, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthBeforeResize, testMessage1.Length);

            // Test resizing and resetting the cursor.
            testMessage1.Resize(ExpectedSize);

            Assert.AreEqual(ExpectedCursorAfterResize, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthAfterResize, testMessage1.Length);

            INetworkMessage testMessage2 = this.SetupInboundMessage(resetCursor: false);

            Assert.IsNotNull(testMessage2);
            Assert.AreEqual(ExpectedCursorBeforeResize, testMessage2.Cursor);
            Assert.AreEqual(ExpectedLengthBeforeResize, testMessage2.Length);

            // Test resizing without resetting the cursor.
            testMessage2.Resize(ExpectedSize, resetCursor: false);

            Assert.AreEqual(ExpectedCursorAfterResize2, testMessage2.Cursor);
            Assert.AreEqual(ExpectedLengthAfterResize, testMessage2.Length);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.Reset"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_ResetTest()
        {
            const int ExpectedCursorBeforeReset = DefaultTestBufferIncomingMessageCursor;
            const int ExpectedLengthBeforeReset = DefaultTestBufferIncomingMessageLength;
            const int ExpectedCursorAfterReset = NetworkMessage.DefaultStartingIndex;
            const int ExpectedLengthAfterReset = NetworkMessage.DefaultStartingIndex;

            INetworkMessage testMessage1 = this.SetupInboundMessage(resetCursor: false);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(ExpectedCursorBeforeReset, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthBeforeReset, testMessage1.Length);

            // Test resetting the message.
            testMessage1.Reset();

            Assert.AreEqual(ExpectedCursorAfterReset, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthAfterReset, testMessage1.Length);

            Assert.IsNotNull(testMessage1.Buffer.ToArray());

            CollectionAssert.AreNotEqual(DefaultTestBufferAsBytes, testMessage1.Buffer.ToArray());
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.SkipBytes(int)"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_SkipBytesTest()
        {
            const int SkipCount = 10;
            const int NegativeSkipCount = -1;
            const int OutOfRangeSkipCountSize = NetworkMessage.BufferSize + 1;
            const int ExpectedCursorOnTestSetup = 0;
            const int ExpectedLengthOnTestSetup = DefaultTestBufferIncomingMessageLength;
            const int ExpectedCursorAfterSkip = SkipCount;
            const int ExpectedLengthAfterSkip = ExpectedLengthOnTestSetup;

            byte[] expectedBytesRead = DefaultTestBufferAsBytes.Skip(SkipCount).ToArray();

            // Do reset the cursor, since we're in "reading" mode.
            INetworkMessage testMessage1 = this.SetupInboundMessage(resetCursor: true);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(ExpectedCursorOnTestSetup, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthOnTestSetup, testMessage1.Length);

            ExceptionAssert.Throws<ArgumentException>(() => testMessage1.SkipBytes(NegativeSkipCount), "The count must not be negative.");
            ExceptionAssert.Throws<IndexOutOfRangeException>(() => testMessage1.SkipBytes(OutOfRangeSkipCountSize), "SkipBytes out of range.");

            testMessage1.SkipBytes(0);

            // These should not change on skip 0.
            Assert.AreEqual(ExpectedCursorOnTestSetup, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthOnTestSetup, testMessage1.Length);

            testMessage1.SkipBytes(SkipCount);

            // Now they should have changed.
            Assert.AreEqual(ExpectedCursorAfterSkip, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthAfterSkip, testMessage1.Length);

            var bytesInfoRead = testMessage1.ReadAsBytesInfo();

            Assert.IsNotNull(bytesInfoRead?.Bytes);
            CollectionAssert.AreEqual(expectedBytesRead, bytesInfoRead.Bytes);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.Copy"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_CopyTest()
        {
            const int ExpectedCursorBeforeReset = DefaultTestBufferIncomingMessageCursor;
            const int ExpectedLengthBeforeReset = DefaultTestBufferIncomingMessageLength;

            INetworkMessage testMessage1 = this.SetupInboundMessage(resetCursor: false);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(ExpectedCursorBeforeReset, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLengthBeforeReset, testMessage1.Length);

            INetworkMessage copiedMessage = testMessage1.Copy();

            Assert.IsNotNull(copiedMessage);
            Assert.AreNotSame(copiedMessage, testMessage1);

            Assert.AreEqual(testMessage1.Cursor, copiedMessage.Cursor);
            Assert.AreEqual(testMessage1.Length, copiedMessage.Length);
            CollectionAssert.AreEqual(copiedMessage.Buffer.ToArray(), testMessage1.Buffer.ToArray());
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.AddByte"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_AddByteTest()
        {
            const byte TestByteValue = 0xAB;

            INetworkMessage testMessage1 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage1.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage1.Length);

            // Add to the message
            testMessage1.AddByte(TestByteValue);

            Assert.AreEqual(TestByteValue, testMessage1.Buffer[testMessage1.Cursor - 1]);

            // Edge case: add when the buffer is full should throw.
            // In this case the easiest thing to do is call it at least 1 + NetworkMessage.BufferSize - currentLength times.
            var addCount = NetworkMessage.BufferSize - testMessage1.Length;
            for (int i = 0; i < addCount; i++)
            {
                // these should all succeed.
                testMessage1.AddByte(TestByteValue);
            }

            // Then this one is the straw that breaks the camel's back.
            ExceptionAssert.Throws<InvalidOperationException>(() => testMessage1.AddByte(TestByteValue), "Message buffer is full.");
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.AddBytes(ReadOnlySpan{byte})"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_AddBytesTest()
        {
            byte[] bytesToAdd = Encoding.Default.GetBytes("Are your base are belong to us.");

            INetworkMessage testMessage1 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage1.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage1.Length);

            // Add to the message
            testMessage1.AddBytes(bytesToAdd);

            for (int i = 0; i < bytesToAdd.Length; i++)
            {
                Assert.AreEqual(bytesToAdd[i], testMessage1.Buffer[testMessage1.Cursor - bytesToAdd.Length + i]);
            }

            // Edge case: add when the buffer is full should throw.
            // In this case the easiest thing to do is call it at least 1 + NetworkMessage.BufferSize - currentLength times.
            var addCount = NetworkMessage.BufferSize - testMessage1.Length;
            for (int i = 0; i < addCount; i++)
            {
                // adding padded values, these should all succeed.
                testMessage1.AddByte(0x00);
            }

            // Then this one is the straw that breaks the camel's back.
            ExceptionAssert.Throws<InvalidOperationException>(() => testMessage1.AddBytes(bytesToAdd), "Message buffer is full.");

            // Repeat the same test, but using the overflow with ReadOnlySequence.
            var sequence = new ReadOnlySequence<byte>(bytesToAdd);

            INetworkMessage testMessage2 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage2);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage2.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage2.Length);

            // Add to the message
            testMessage2.AddBytes(sequence);

            for (int i = 0; i < bytesToAdd.Length; i++)
            {
                Assert.AreEqual(bytesToAdd[i], testMessage2.Buffer[testMessage2.Cursor - bytesToAdd.Length + i]);
            }

            // Edge case: add when the buffer is full should throw.
            // In this case the easiest thing to do is call it at least 1 + NetworkMessage.BufferSize - currentLength times.
            var addCount2 = NetworkMessage.BufferSize - testMessage2.Length;
            for (int i = 0; i < addCount2; i++)
            {
                // adding padded values, these should all succeed.
                testMessage2.AddByte(0x00);
            }

            // Then this one is the straw that breaks the camel's back.
            ExceptionAssert.Throws<InvalidOperationException>(() => testMessage2.AddBytes(sequence), "Message buffer is full.");
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.AddString"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_AddStringTest()
        {
            const string StringToAdd = "Moonwalking.";
            const ushort ExpectedStringLen = 12;

            INetworkMessage testMessage1 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage1.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage1.Length);

            // Add to the message
            testMessage1.AddString(StringToAdd);

            var asInboundMessage = ConvertToInboundMessage(testMessage1);

            // Leverage other NetworkMessage methods, which is fine as they have their own tests.
            var strLength = asInboundMessage.GetUInt16();
            var strRead = Encoding.Default.GetString(asInboundMessage.Buffer.ToArray(), asInboundMessage.Cursor, strLength);

            Assert.AreEqual(ExpectedStringLen, strLength);
            Assert.AreEqual(StringToAdd, strRead);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.AddUInt16"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_AddUInt16Test()
        {
            const ushort ValueToAdd = 12345;

            INetworkMessage testMessage1 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage1.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage1.Length);

            // Add to the message
            testMessage1.AddUInt16(ValueToAdd);

            var asInboundMessage = ConvertToInboundMessage(testMessage1);

            // Leverage other NetworkMessage methods, which is fine as they have their own tests.
            var valueRead = asInboundMessage.GetUInt16();

            Assert.AreEqual(ValueToAdd, valueRead);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.AddUInt32"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_AddUInt32Test()
        {
            const uint ValueToAdd = 123456789;

            INetworkMessage testMessage1 = this.SetupOutboundMessage();

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageCursor, testMessage1.Cursor);
            Assert.AreEqual(DefaultTestBufferOutgoingMessageLength, testMessage1.Length);

            // Add to the message
            testMessage1.AddUInt32(ValueToAdd);

            var asInboundMessage = ConvertToInboundMessage(testMessage1);

            // Leverage other NetworkMessage methods, which is fine as they have their own tests.
            var valueRead = asInboundMessage.GetUInt32();

            Assert.AreEqual(ValueToAdd, valueRead);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.GetByte"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_GetByteTest()
        {
            const byte ByteToSet = 0xDF;

            INetworkMessage testMessage1 = this.SetupInboundMessage(ByteToSet);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(1, testMessage1.Length);

            // Test resetting the message.
            var valueRead = testMessage1.GetByte();

            Assert.AreEqual(ByteToSet, valueRead);
            Assert.AreEqual(1, testMessage1.Cursor);
            Assert.AreEqual(1, testMessage1.Length);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.PeekByte"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_PeekByteTest()
        {
            const byte ByteToSet = 0xDF;

            INetworkMessage testMessage1 = this.SetupInboundMessage(ByteToSet);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(1, testMessage1.Length);

            // Test resetting the message.
            var valueRead = testMessage1.PeekByte();

            Assert.AreEqual(ByteToSet, valueRead);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(1, testMessage1.Length);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.GetUInt16"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_GetUInt16Test()
        {
            const ushort ExpectedValue = 12345;

            byte[] bytesToSet = { 0x39, 0x30 };

            INetworkMessage testMessage1 = this.SetupInboundMessage(bytesToSet);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(2, testMessage1.Length);

            // Test resetting the message.
            var valueRead = testMessage1.GetUInt16();

            Assert.AreEqual(ExpectedValue, valueRead);
            Assert.AreEqual(2, testMessage1.Cursor);
            Assert.AreEqual(2, testMessage1.Length);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.GetUInt32"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_GetUInt32Test()
        {
            const uint ExpectedValue = 123456789;

            byte[] bytesToSet = { 0x15, 0xCD, 0x5B, 0x07 };

            INetworkMessage testMessage1 = this.SetupInboundMessage(bytesToSet);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(4, testMessage1.Length);

            // Test resetting the message.
            var valueRead = testMessage1.GetUInt32();

            Assert.AreEqual(ExpectedValue, valueRead);
            Assert.AreEqual(4, testMessage1.Cursor);
            Assert.AreEqual(4, testMessage1.Length);
        }

        /// <summary>
        /// Checks that the <see cref="INetworkMessage.GetString"/> works as expected.
        /// </summary>
        [TestMethod]
        public void NetworkMessage_GetStringTest()
        {
            const string ExpectedValue = "All your base are belong to us.";
            const int ExpectedLength = 31;

            byte[] bytesFromValue = Encoding.Default.GetBytes(ExpectedValue);
            byte[] bytesFromLength = BitConverter.GetBytes((ushort)bytesFromValue.Length);
            byte[] allBytesToAdd = bytesFromLength.Concat(bytesFromValue).ToArray();

            INetworkMessage testMessage1 = this.SetupInboundMessage(allBytesToAdd);

            Assert.IsNotNull(testMessage1);
            Assert.AreEqual(0, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLength + 2, testMessage1.Length);

            // Test resetting the message.
            var valueRead = testMessage1.GetString();

            Assert.AreEqual(ExpectedValue, valueRead);
            Assert.AreEqual(ExpectedLength + 2, testMessage1.Cursor);
            Assert.AreEqual(ExpectedLength + 2, testMessage1.Length);
        }

        private static INetworkMessage ConvertToInboundMessage(INetworkMessage outboundMessage)
        {
            return outboundMessage.Copy(DefaultTestBufferOutgoingMessageCursor);
        }

        private INetworkMessage SetupInboundMessage(bool resetCursor = true)
        {
            var message = new NetworkMessage(isOutbound: false);

            byte[] mockedBuffer = Encoding.Default.GetBytes(DefaultTestBuffer);

            // mock GetBytes because it's not the object of this test.
            message.AddBytes(mockedBuffer);
            message.Resize(message.Cursor, resetCursor);

            return message;
        }

        private INetworkMessage SetupInboundMessage(params byte[] bytesContent)
        {
            var newMessage = new NetworkMessage(isOutbound: false);

            bytesContent.CopyTo(newMessage.Buffer);

            var lengthPropertyInfo = newMessage.GetType().GetProperty(nameof(NetworkMessage.Length));

            lengthPropertyInfo.SetValue(newMessage, bytesContent.Length);

            return newMessage;
        }

        private INetworkMessage SetupOutboundMessage()
        {
            return new NetworkMessage(isOutbound: true);
        }
    }
}
