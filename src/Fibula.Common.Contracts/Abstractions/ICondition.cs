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
        /// Aggregates this condition's properties with another of the same type.
        /// </summary>
        /// <param name="conditionOfSameType">The condition to aggregate with.</param>
        /// <returns>True if this condition's properties were changed as a result, and false if nothing changed.</returns>
        bool Aggregate(ICondition conditionOfSameType);
    }
}
