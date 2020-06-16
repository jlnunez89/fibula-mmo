// -----------------------------------------------------------------
// <copyright file="LoginListenerTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Listeners.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.TestingUtilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Listeners;
    using Fibula.Security.Contracts;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Serilog;

    /// <summary>
    /// Tests for the <see cref="GatewayListener{T}"/> class.
    /// </summary>
    [TestClass]
    public class LoginListenerTests
    {
        /// <summary>
        /// Checks <see cref="GatewayListener{T}"/> initialization.
        /// </summary>
        [TestMethod]
        public void LoginListener_Initialization()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ISocketConnectionFactory> connectionFactoryMock = new Mock<ISocketConnectionFactory>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();

            GatewayListenerOptions loginListenerOptions = new GatewayListenerOptions()
            {
                Port = 7171,
            };

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(() => new GatewayListener<ISocketConnectionFactory>(null, Options.Create(loginListenerOptions), connectionFactoryMock.Object, defenderMock.Object), $"Value cannot be null. (Parameter 'logger')");
            ExceptionAssert.Throws<ArgumentNullException>(() => new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, null, connectionFactoryMock.Object, defenderMock.Object), $"Value cannot be null. (Parameter 'options')");
            ExceptionAssert.Throws<ArgumentNullException>(() => new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(loginListenerOptions), null, defenderMock.Object), $"Value cannot be null. (Parameter 'socketConnectionFactory')");

            // A null DoS defender is OK.
            new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(loginListenerOptions), connectionFactoryMock.Object, null);

            // And initialize with all good values.
            new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(loginListenerOptions), connectionFactoryMock.Object, defenderMock.Object);
        }

        /// <summary>
        /// Checks the <see cref="GatewayListener{T}"/>'s options validation.
        /// </summary>
        [TestMethod]
        public void GameListener_OptionsValidation()
        {
            const ushort AnyEphemerealPort = 4323;

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ISocketConnectionFactory> connectionFactoryMock = new Mock<ISocketConnectionFactory>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();

            GatewayListenerOptions goodGatewayListenerOptions = new GatewayListenerOptions() { Port = AnyEphemerealPort };

            GatewayListenerOptions badGatewayListenerOptions = new GatewayListenerOptions();

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(
                () => new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, null, connectionFactoryMock.Object, defenderMock.Object),
                "Value cannot be null. (Parameter 'options')");

            ExceptionAssert.Throws<ValidationException>(
                () => new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(badGatewayListenerOptions), connectionFactoryMock.Object, defenderMock.Object),
                "A port for the gateway listener must be speficied.");

            // And initialize with all good values.
            new GatewayListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(goodGatewayListenerOptions), connectionFactoryMock.Object, defenderMock.Object);
        }

        /// <summary>
        /// Checks that the <see cref="GatewayListener{T}"/> invokes the <see cref="ITcpListener.NewConnection"/> event.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Listener_CallsNewConnectionEvent()
        {
            const ushort AnyEphemerealPort = 1234;
            const int ExpectedConnectionCount = 1;

            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();

            Mock<ISocketConnectionFactory> connectionFactoryMock = this.SetupSocketConnectionFactory();

            GatewayListenerOptions loginListenerOptions = new GatewayListenerOptions()
            {
                Port = AnyEphemerealPort,
            };

            ITcpListener loginListener = new GatewayListener<ISocketConnectionFactory>(
                loggerMock.Object,
                Options.Create(loginListenerOptions),
                connectionFactoryMock.Object,
                defenderMock.Object);

            var connectionCount = 0;
            var listenerTask = loginListener.StartAsync(CancellationToken.None);

            loginListener.NewConnection += (connection) =>
            {
                connectionCount++;
            };

            // Now make a connection to the listener.
            using var tcpClient = new TcpClient();

            tcpClient.Connect(IPAddress.Loopback, AnyEphemerealPort);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has gone up on connections count.
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedConnectionCount, connectionCount, "New connections events counter does not match.");
            });
        }

        private Mock<ISocketConnectionFactory> SetupSocketConnectionFactory()
        {
            var mockedCreatedConnection = new Mock<ISocketConnection>();

            var connectionFactoryMock = new Mock<ISocketConnectionFactory>();

            connectionFactoryMock.Setup(c => c.Create(It.IsAny<Socket>()))
                                 .Returns(mockedCreatedConnection.Object);

            return connectionFactoryMock;
        }
    }
}
