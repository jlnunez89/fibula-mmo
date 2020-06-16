// -----------------------------------------------------------------
// <copyright file="CompositeValidationResult.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Utilities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that extends a <see cref="ValidationResult"/>, composed of multiple results.
    /// </summary>
    public class CompositeValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">An error message to initialize the result with.</param>
        public CompositeValidationResult(string errorMessage)
            : base(errorMessage)
        {
            this.Results = new List<ValidationResult>();
        }

        /// <summary>
        /// Gets the collection of results in this composite result.
        /// </summary>
        public IList<ValidationResult> Results { get; }

        /// <summary>
        /// Adds a result to this composite's list.
        /// </summary>
        /// <param name="validationResult">The result to add.</param>
        public void AddResult(ValidationResult validationResult)
        {
            validationResult.ThrowIfNull(nameof(validationResult));

            this.Results.Add(validationResult);
        }
    }
}
