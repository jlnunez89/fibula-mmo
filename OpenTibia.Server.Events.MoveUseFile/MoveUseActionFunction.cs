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

    internal class MoveUseActionFunction : IEventRuleFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseActionFunction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public MoveUseActionFunction(string name, params object[] parameters)
        {
            this.FunctionName = name;
            this.Parameters = parameters;
        }

        public string FunctionName { get; }

        public object[] Parameters { get; }
    }
}