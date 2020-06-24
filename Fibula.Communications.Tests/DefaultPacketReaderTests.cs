// -----------------------------------------------------------------
// <copyright file="DefaultPacketReaderTests.cs" company="2Dudes">
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
    using Fibula.Common.TestingUtilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Serilog;

    /// <summary>
    /// Tests for the <see cref="DefaultPacketReader"/> class.
    /// </summary>
    [TestClass]
    public class DefaultPacketReaderTests
    {
        /// <summary>
        /// Checks <see cref="DefaultPacketReader"/> initialization.
        /// </summary>
        [TestMethod]
        public void DefaultPacketReader_Initialization()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(() => new DefaultPacketReader(null), $"Value cannot be null. (Parameter 'logger')");

            new DefaultPacketReader(loggerMock.Object);
        }

        /// <summary>
        /// Checks that the <see cref="DefaultPacketReader"/> calls <see cref="IPacketReader.ReadFromMessage(INetworkMessage)"/>.
        /// </summary>
        [TestMethod]
        public void DefaultPacketReader_CallsReadFromMessage()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<INetworkMessage> networkMessageMock = new Mock<INetworkMessage>();
            Mock<IBytesInfo> bytesInfoMock = new Mock<IBytesInfo>();

            // Setup networkMessage as mock since it's not the target of this test.
            networkMessageMock.Setup(m => m.ReadAsBytesInfo())
                              .Returns(bytesInfoMock.Object);

            var reader = new DefaultPacketReader(loggerMock.Object);

            var resultInfo = reader.ReadFromMessage(networkMessageMock.Object);

            Assert.IsNotNull(resultInfo);
            Assert.AreSame(bytesInfoMock.Object, resultInfo, "The information returned doesn't match.");
        }
    }
}
