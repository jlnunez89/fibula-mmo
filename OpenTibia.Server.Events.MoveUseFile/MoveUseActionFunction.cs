// -----------------------------------------------------------------
// <copyright file="MoveUseActionFunction.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an action function for the move/use event rules.
    /// </summary>
    internal class MoveUseActionFunction : IEventRuleFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseActionFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="parameters">The parameters of the function.</param>
        public MoveUseActionFunction(string name, params object[] parameters)
        {
            this.FunctionName = name;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// Gets the parameters for the function.
        /// </summary>
        public object[] Parameters { get; }
    }
}