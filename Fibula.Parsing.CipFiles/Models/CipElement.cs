// -----------------------------------------------------------------
// <copyright file="CipElement.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using Fibula.Parsing.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an element.
    /// </summary>
    public class CipElement : IParsedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CipElement"/> class.
        /// </summary>
        /// <param name="id">The id of the element.</param>
        /// <param name="attributes">The attributes of this element.</param>
        public CipElement(int id, IList<IParsedAttribute> attributes = null)
        {
            this.Id = id;
            this.Attributes = attributes ?? new List<IParsedAttribute>();
        }

        /// <summary>
        /// Gets the id of the element.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets a value indicating whether  this element is a flag.
        /// </summary>
        public bool IsFlag => this.Id < 0;

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        public IList<IParsedAttribute> Attributes { get; }
    }
}
