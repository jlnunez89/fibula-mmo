// -----------------------------------------------------------------
// <copyright file="ActionFunction.cs" company="2Dudes">
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
    /// <summary>
    /// Class that represents a script function.
    /// </summary>
    public class ActionFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="parameters">The function parameters.</param>
        public ActionFunction(string name, params object[] parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the function's parameters.
        /// </summary>
        public object[] Parameters { get; }
    }
}
