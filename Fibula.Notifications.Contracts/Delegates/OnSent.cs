// -----------------------------------------------------------------
// <copyright file="OnSent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Contracts.Delegates
{
    using Fibula.Client.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a notification is sent to a client.
    /// </summary>
    /// <param name="toClient">The client to which the notification was sent.</param>
    public delegate void OnSent(IClient toClient);
}
