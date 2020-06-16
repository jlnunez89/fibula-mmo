// -----------------------------------------------------------------
// <copyright file="FunctionComparisonType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
