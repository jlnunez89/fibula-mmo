// -----------------------------------------------------------------
// <copyright file="IDataAnnotationsValidator.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Interface for data annotations validators.
    /// </summary>
    public interface IDataAnnotationsValidator
    {
        /// <summary>
        /// Performs validation on an object and all of it's properties, recursing into any non-primitive properties.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        void ValidateObjectRecursive<T>(T obj, IDictionary<object, object> validationContextItems = null);

        /// <summary>
        /// Performs validation on an object and all of it's primitive properties.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <param name="results">The results of the validation.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        /// <returns>True if the validation passes, false otherwise.</returns>
        bool TryValidateObject(object obj, out IList<ValidationResult> results, IDictionary<object, object> validationContextItems = null);

        /// <summary>
        /// Performs validation on an object and all of it's properties, recursing into any non-primitive properties.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="results">The results of the validation.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        /// <returns>True if the validation passes, false otherwise.</returns>
        bool TryValidateObjectRecursive<T>(T obj, out IList<ValidationResult> results, IDictionary<object, object> validationContextItems = null);
    }
}
