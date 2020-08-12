// -----------------------------------------------------------------
// <copyright file="ConnectionMessageProccessedDelegate.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Delegates
{
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Represents a delegate to call after a message is processed in a connection.
    /// </summary>
    /// <param name="connection">The connection that had the message proccesed.</param>
    public delegate void ConnectionPacketProccessedDelegate(IConnection connection);
}
