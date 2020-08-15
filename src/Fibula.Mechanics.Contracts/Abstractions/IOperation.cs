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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for a game operation.
    /// </summary>
    public interface IOperation : IEvent
    {
        /// <summary>
        /// Gets the information about the exhaustion that this operation produces.
        /// </summary>
        (ConditionType Type, TimeSpan Cost)? AssociatedExhaustion { get; }
    }
}
