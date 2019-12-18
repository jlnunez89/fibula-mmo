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

    internal class MoveUseComparisonFunction : IEventRuleFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseComparisonFunction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="compareIdentifier"></param>
        /// <param name="parameters"></param>
        public MoveUseComparisonFunction(string name, FunctionComparisonType type, string compareIdentifier, object[] parameters)
        {
            this.FunctionName = name;
            this.Type = type;
            this.CompareToIdentifier = compareIdentifier;
            this.Parameters = parameters;
        }

        public string FunctionName { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public object[] Parameters { get; }
    }
}