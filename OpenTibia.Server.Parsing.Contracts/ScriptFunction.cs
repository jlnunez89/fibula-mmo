// -----------------------------------------------------------------
// <copyright file="ScriptFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Contracts
{
    public class ScriptFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFunction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public ScriptFunction(string name, params object[] parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }

        public string Name { get; }

        public object[] Parameters { get; }
    }
}