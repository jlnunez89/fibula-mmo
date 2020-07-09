// -----------------------------------------------------------------
// <copyright file="NewConnectionDelegate.cs" company="2Dudes">
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
    /// Represents a delegate to call when a new connection is enstablished.
    /// </summary>
    /// <param name="connection">The connection that was opened.</param>
    public delegate void NewConnectionDelegate(IConnection connection);
}
