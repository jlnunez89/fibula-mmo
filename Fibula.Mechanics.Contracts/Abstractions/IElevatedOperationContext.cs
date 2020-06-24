// -----------------------------------------------------------------
// <copyright file="IElevatedOperationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for an elevated operation context.
    /// </summary>
    public interface IElevatedOperationContext : IOperationContext
    {
        /// <summary>
        /// Gets the reference to the creature manager in use.
        /// </summary>
        ICreatureManager CreatureManager { get; }
    }
}