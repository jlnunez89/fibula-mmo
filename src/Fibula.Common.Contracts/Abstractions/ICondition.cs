// -----------------------------------------------------------------
// <copyright file="ICondition.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for all conditions.
    /// </summary>
    public interface ICondition : IEvent
    {
        /// <summary>
        /// Gets the type of this condition.
        /// </summary>
        ConditionType Type { get; }

        /// <summary>
        /// Gets or sets the end time for this condition.
        /// </summary>
        DateTimeOffset EndTime { get; set; }
    }
}
