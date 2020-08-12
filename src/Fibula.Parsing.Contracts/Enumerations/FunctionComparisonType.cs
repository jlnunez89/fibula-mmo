// -----------------------------------------------------------------
// <copyright file="FunctionComparisonType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the types of function comparison.
    /// </summary>
    public enum FunctionComparisonType : byte
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// Greater than.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Less than.
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Equal.
        /// </summary>
        Equal,
    }
}
