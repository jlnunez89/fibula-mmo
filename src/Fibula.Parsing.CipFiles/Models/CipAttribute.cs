// -----------------------------------------------------------------
// <copyright file="CipAttribute.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
