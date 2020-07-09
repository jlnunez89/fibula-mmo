// -----------------------------------------------------------------
// <copyright file="ActionFunction.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
