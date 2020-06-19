// -----------------------------------------------------------------
// <copyright file="CipAttribute.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Models
{
    using Fibula.Parsing.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an attribute.
    /// </summary>
    public class CipAttribute : IParsedAttribute
    {
        /// <summary>
        /// Gets or sets the attribute's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attribute's value.
        /// </summary>
        public object Value { get; set; }
    }
}