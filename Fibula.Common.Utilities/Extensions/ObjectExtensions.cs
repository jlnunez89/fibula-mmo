// -----------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// Helper class that provides methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> YieldSingleItem<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Gets the value of a given object's property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property, or null if not defined.</returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            object objValue = null;

            var propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                objValue = propertyInfo.GetValue(obj, null);
            }

            return objValue;
        }
    }
}
