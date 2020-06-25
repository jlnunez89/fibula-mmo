// -----------------------------------------------------------------
// <copyright file="GameListenerTests.cs" company="2Dudes">
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
    /// Tests for the <see cref="GameListener{T}"/> class.
    /// </summary>
    [TestClass]
    public class GameListenerTests
    {
        /// <summary>
        /// Checks <see cref="GameListener{T}"/> initialization.
        /// </summary>
        [TestMethod]
        public void GameListener_Initialization()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ISocketConnectionFactory> connectionFactoryMock = new Mock<ISocketConnectionFactory>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();

            GameListenerOptions gameListenerOptions = new GameListenerOptions()
            {
                Port = 7171,
            };

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(() => new GameListener<ISocketConnectionFactory>(null, Options.Create(gameListenerOptions), connectionFactoryMock.Object, defenderMock.Object), "Value cannot be null. (Parameter 'logger')");
            ExceptionAssert.Throws<ArgumentNullException>(() => new GameListener<ISocketConnectionFactory>(loggerMock.Object, null, connectionFactoryMock.Object, defenderMock.Object), "Value cannot be null. (Parameter 'options')");
            ExceptionAssert.Throws<ArgumentNullException>(() => new GameListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(gameListenerOptions), null, defenderMock.Object), "Value cannot be null. (Parameter 'socketConnectionFactory')");

            // A null DoS defender is OK.
            new GameListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(gameListenerOptions), connectionFactoryMock.Object, null);

            // And initialize with all good values.
            new GameListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(gameListenerOptions), connectionFactoryMock.Object, defenderMock.Object);
        }

        /// <summary>
        /// Checks the <see cref="GameListener{T}"/>'s options validation.
        /// </summary>
        [TestMethod]
        public void GameListener_OptionsValidation()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ISocketConnectionFactory> connectionFactoryMock = new Mock<ISocketConnectionFactory>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();

            GameListenerOptions goodGameListenerOptions = new GameListenerOptions() { Port = 7171 };

            GameListenerOptions badGameListenerOptions = new GameListenerOptions();

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(
                () => new GameListener<ISocketConnectionFactory>(loggerMock.Object, null, connectionFactoryMock.Object, defenderMock.Object),
                "Value cannot be null. (Parameter 'options')");

            ExceptionAssert.Throws<ValidationException>(
                () => new GameListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(badGameListenerOptions), connectionFactoryMock.Object, defenderMock.Object),
                "A port for the game listener must be speficied.");

            // And initialize with all good values.
            new GameListener<ISocketConnectionFactory>(loggerMock.Object, Options.Create(goodGameListenerOptions), connectionFactoryMock.Object, defenderMock.Object);
        }

        /// <summary>
        /// Checks that the <see cref="GameListener{T}"/> invokes the <see cref="IListener.NewConnection"/> event.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task GameListener_CallsNewConnectionEvent()
        {
            const ushort AnyEphemerealPort = 7171;
            const int ExpectedConnectionCount = 1;
            const int NewConnectionsToEmulate = 1;

            TimeSpan waitForConnectionDelay = TimeSpan.FromSeconds(1);

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<IDoSDefender> defenderMock = new Mock<IDoSDefender>();
            Mock<ITcpListener> tcpListenerMock = this.SetupTcpListenerMock(NewConnectionsToEmulate);

            Mock<ISocketConnectionFactory> connectionFactoryMock = this.SetupSocketConnectionFactory();

            GameListenerOptions gameListenerOptions = new GameListenerOptions()
            {
                Port = AnyEphemerealPort,
            };

            IListener gameListener = new GameListener<ISocketConnectionFactory>(
                loggerMock.Object,
                Options.Create(gameListenerOptions),
                connectionFactoryMock.Object,
                defenderMock.Object,
                tcpListenerMock.Object);

            var connectionCount = 0;
            var listenerTask = gameListener.StartAsync(CancellationToken.None);

            gameListener.NewConnection += (connection) =>
            {
                connectionCount++;
            };

            // Delay for a second and check that the counter has gone up on connections count.
            await Task.Delay(waitForConnectionDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedConnectionCount, connectionCount, "New connections events counter does not match.");
            });
        }

        private Mock<ITcpListener> SetupTcpListenerMock(int newConnectionsToEmulate)
        {
            Mock<ITcpListener> tcpListenerMock = new Mock<ITcpListener>();

            var bigEnoughTimeToWaitAfterGoal = TimeSpan.FromMinutes(1);
            var emulatedConnectionsCount = 0;

            async Task<Socket> WaitForSocketMock()
            {
                if (emulatedConnectionsCount++ == newConnectionsToEmulate)
                {
                    // Wait for a minute if we've reached the target count of connections to emulate.
                    await Task.Delay(bigEnoughTimeToWaitAfterGoal);
                }

                return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            tcpListenerMock.Setup(l => l.AcceptSocketAsync()).Returns(WaitForSocketMock);

            return tcpListenerMock;
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
