// -----------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="2Dudes">
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
