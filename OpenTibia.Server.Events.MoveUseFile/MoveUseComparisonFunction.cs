// -----------------------------------------------------------------
// <copyright file="MoveUseComparisonFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Parsing.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an comparison function for the move/use event rules.
    /// </summary>
    internal class MoveUseComparisonFunction : IEventRuleFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseComparisonFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the comparison.</param>
        /// <param name="type">The type of the comparison to make.</param>
        /// <param name="compareIdentifier">The comparison identifier.</param>
        /// <param name="parameters">The parameters to compare.</param>
        public MoveUseComparisonFunction(string name, FunctionComparisonType type, string compareIdentifier, object[] parameters)
        {
            this.FunctionName = name;
            this.Type = type;
            this.CompareToIdentifier = compareIdentifier;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// Gets the function's type.
        /// </summary>
        public FunctionComparisonType Type { get; }

        /// <summary>
        /// Gets the benchmark to compare to.
        /// </summary>
        public string CompareToIdentifier { get; }

        /// <summary>
        /// Gets the parameters from the function.
        /// </summary>
        public object[] Parameters { get; }
    }
}