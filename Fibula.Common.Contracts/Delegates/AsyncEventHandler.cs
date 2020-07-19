// -----------------------------------------------------------------
// <copyright file="AsyncEventHandler.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Delegates
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for asynchronous events.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments.</typeparam>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <param name="token">A token to observe for cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous event.</returns>
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e, CancellationToken token);
}
