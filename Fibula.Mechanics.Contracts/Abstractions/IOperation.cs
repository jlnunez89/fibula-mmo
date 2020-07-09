// -----------------------------------------------------------------
// <copyright file="IOperation.cs" company="2Dudes">
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
    using System;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for a game operation.
    /// </summary>
    public interface IOperation : IEvent
    {
        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets the exhaustion cost of this operation.
        /// </summary>
        TimeSpan ExhaustionCost { get; }
    }
}
