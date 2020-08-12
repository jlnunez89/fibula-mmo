// -----------------------------------------------------------------
// <copyright file="DataAnnotationsValidator.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class that represents a helper for dana annotations validator.
    /// </summary>
    public static class DataAnnotationsValidator
    {
        /// <summary>
        /// Performs validation on an object and all of it's primitive properties.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <param name="results">The results of the validation.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        /// <returns>True if the validation passes, false otherwise.</returns>
        public static bool TryValidateObject(object obj, out IList<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems), results, true);
        }

        /// <summary>
        /// Performs validation on an object and all of it's properties, recursing into any non-primitive properties.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="results">The results of the validation.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        /// <returns>True if the validation passes, false otherwise.</returns>
        public static bool TryValidateObjectRecursive<T>(T obj, out IList<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
        {
            return TryValidateObjectRecursive(obj, out results, new HashSet<object>(), validationContextItems);
        }

        /// <summary>
        /// Performs validation on an object and all of it's properties, recursing into any non-primitive properties.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="validationContextItems">Auxiliary items to initialize validation context with.</param>
        public static void ValidateObjectRecursive<T>(T obj, IDictionary<object, object> validationContextItems = null)
        {
            TryValidateObjectRecursive(obj, out IList<ValidationResult> results, validationContextItems);

            if (results.Any())
            {
                StringBuilder sb = new StringBuilder();
                int index = 1;

                foreach (var result in results)
                {
                    sb.Append($"{Environment.NewLine}  {index++}) {result}");
                }

                throw new ValidationException($"At least one validation error found for {obj.GetType().Name}:{sb}");
            }
        }

        private static bool TryValidateObjectRecursive<T>(T obj, out IList<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems = null)
        {
            results = new List<ValidationResult>();

            // Short-circuit to avoid infinite loops on cyclic object graphs
            if (validatedObjects.Contains(obj))
            {
                return true;
            }

            validatedObjects.Add(obj);

            bool validationPassed = TryValidateObject(obj, out results, validationContextItems);

            var propertiesInfo = obj.GetType().GetProperties()
                    .Where(prop => prop.CanRead && !prop.GetCustomAttributes(typeof(SkipRecursiveValidation), false).Any() && prop.GetIndexParameters().Length == 0)
                    .ToList();

            foreach (var propertyInfo in propertiesInfo)
            {
                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsValueType)
                {
                    continue;
                }

                var value = obj.GetPropertyValue(propertyInfo.Name);

                if (value == null)
                {
                    continue;
                }

                // Treat as a collection
                if (value is IEnumerable asEnumerable)
                {
                    foreach (var enumObj in asEnumerable)
                    {
                        if (enumObj == null)
                        {
                            continue;
                        }

                        if (!TryValidateObjectRecursive(enumObj, out IList<ValidationResult> nestedResults, validatedObjects, validationContextItems))
                        {
                            validationPassed = false;

                            foreach (var validationResult in nestedResults)
                            {
                                results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => propertyInfo.Name + '.' + x)));
                            }
                        }
                    }
                }

                // Or as an indivitual object.
                else if (!TryValidateObjectRecursive(value, out IList<ValidationResult> nestedResults, validatedObjects, validationContextItems))
                {
                    validationPassed = false;

                    foreach (var validationResult in nestedResults)
                    {
                        results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => propertyInfo.Name + '.' + x)));
                    }
                }
            }

            return validationPassed;
        }
    }
}
