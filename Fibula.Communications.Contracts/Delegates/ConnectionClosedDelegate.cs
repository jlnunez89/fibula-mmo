// -----------------------------------------------------------------
// <copyright file="ConnectionClosedDelegate.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Delegates
{
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Represents a delegate to call when a connection is closed.
    /// </summary>
    /// <param name="connection">The connection that was closed.</param>
    public delegate void ConnectionClosedDelegate(IConnection connection);
}
