// -----------------------------------------------------------------
// <copyright file="ValidateCollectionAttribute.cs" company="2Dudes">
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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Class that extends a <see cref="ValidationAttribute"/>, which marks a collection for validation.
    /// </summary>
    public class ValidateCollectionAttribute : ValidationAttribute
    {
        public Type ValidationType { get; set; }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> object that describes the context
        /// where the validation checks are performed. This parameter cannot be null.
        /// </param>
        /// <returns>True if the specified value is valid, false otherwise.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var collectionResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");

            var validators = this.GetValidators().ToList();

            if (value is IEnumerable enumerable)
            {
                var index = 0;

                foreach (var val in enumerable)
                {
                    var results = new List<ValidationResult>();
                    var context = new ValidationContext(val);

                    if (this.ValidationType != null)
                    {
                        Validator.TryValidateValue(val, context, results, validators);
                    }
                    else
                    {
                        Validator.TryValidateObject(val, context, results, true);
                    }

                    if (results.Count != 0)
                    {
                        var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName}[{index}] failed!");

                        results.ForEach(compositeResults.AddResult);

                        collectionResults.AddResult(compositeResults);
                    }

                    index++;
                }
            }

            if (collectionResults.Results.Any())
            {
                return collectionResults;
            }

            return ValidationResult.Success;
        }

        private IEnumerable<ValidationAttribute> GetValidators()
        {
            if (this.ValidationType == null)
            {
                yield break;
            }

            yield return (ValidationAttribute)Activator.CreateInstance(this.ValidationType);
        }
    }
}
