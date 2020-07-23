// -----------------------------------------------------------------
// <copyright file="IElevatedOperationContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for an elevated operation context.
    /// </summary>
    public interface IElevatedOperationContext : IOperationContext
    {
        /// <summary>
        /// Gets the reference to the application context.
        /// </summary>
        IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Gets the reference to the creature manager in use.
        /// </summary>
        ICreatureManager CreatureManager { get; }
    }
}
