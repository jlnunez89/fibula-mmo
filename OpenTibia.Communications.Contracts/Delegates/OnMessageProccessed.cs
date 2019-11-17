// -----------------------------------------------------------------
// <copyright file="OnMessageProccessed.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Delegates
{
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Represents a delegate to call when a message is processed in a connection.
    /// </summary>
    /// <param name="connection">The connection that had the message proccesed.</param>
    public delegate void OnMessageProccessed(IConnection connection);
}
