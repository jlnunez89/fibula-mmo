// -----------------------------------------------------------------
// <copyright file="IParsedElement.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a parsed element.
    /// </summary>
    public interface IParsedElement
    {
        /// <summary>
        /// Gets the id of the element.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets a value indicating whether this element is a flag.
        /// </summary>
        bool IsFlag { get; }

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        IList<IParsedAttribute> Attributes { get; }
    }
}
