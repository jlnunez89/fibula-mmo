// -----------------------------------------------------------------
// <copyright file="OnSent.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Delegates
{
    using Fibula.Client.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a notification is sent to a client.
    /// </summary>
    /// <param name="toClient">The client to which the notification was sent.</param>
    public delegate void OnSent(IClient toClient);
}
