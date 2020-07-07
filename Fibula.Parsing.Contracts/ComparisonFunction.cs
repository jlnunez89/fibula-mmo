// -----------------------------------------------------------------
// <copyright file="ComparisonFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.Contracts
{
    using Fibula.Common.Utilities;
    using Fibula.Parsing.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a function that is a comparison to a value or constant.
    /// </summary>
    public class ComparisonFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="constantOrValue">The constant or value being compared against.</param>
        /// <param name="parameters">The parameters of the function.</param>
        public ComparisonFunction(string name, string comparisonType, string constantOrValue, params string[] parameters)
        {
            name.ThrowIfNull(nameof(name));
            comparisonType.ThrowIfNull(nameof(comparisonType));
            constantOrValue.ThrowIfNull(nameof(constantOrValue));
            parameters.ThrowIfNull(nameof(parameters));

            this.Name = name;
            this.Parameters = parameters;
            this.ConstantOrValue = constantOrValue;
            this.Parameters = parameters;

            if (comparisonType == ">=")
            {
                this.Type = FunctionComparisonType.GreaterThanOrEqual;
            }
            else if (comparisonType == "<=")
            {
                this.Type = FunctionComparisonType.LessThanOrEqual;
            }
            else if (comparisonType == ">")
            {
                this.Type = FunctionComparisonType.GreaterThan;
            }
            else if (comparisonType == "<")
            {
                this.Type = FunctionComparisonType.LessThan;
            }
            else if (comparisonType == "==")
            {
                this.Type = FunctionComparisonType.Equal;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        /// Gets the type of comparison to the <see cref="ConstantOrValue"/>.
        /// </summary>
        public FunctionComparisonType Type { get; }

        /// <summary>
        /// Gets the constant or value to compare against.
        /// </summary>
        public string ConstantOrValue { get; }
    }
}
