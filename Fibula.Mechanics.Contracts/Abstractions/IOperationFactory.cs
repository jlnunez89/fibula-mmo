// -----------------------------------------------------------------
// <copyright file="IOperationFactory.cs" company="2Dudes">
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
    using Serilog;

    /// <summary>
    /// Interface for factories of operations.
    /// </summary>
    public interface IOperationFactory
    {
        /// <summary>
        /// Gets the reference to the factory's logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="IOperation"/> based on the type specified with the given arguments.
        /// </summary>
        /// <param name="arguments">The arguments for creation.</param>
        /// <returns>A new instance of <see cref="IOperation"/>.</returns>
        IOperation Create(IOperationCreationArguments arguments);
    }
}
