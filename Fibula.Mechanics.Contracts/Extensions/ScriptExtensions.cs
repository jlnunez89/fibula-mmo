// -----------------------------------------------------------------
// <copyright file="ScriptExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    public static class ScriptExtensions
    {
        public static object ConvertSingleItem(this string value, Type newType)
        {
            if (typeof(IConvertible).IsAssignableFrom(newType))
            {
                return Convert.ChangeType(value, newType);
            }

            var converter = CustomConvertersFactory.GetConverter(newType);

            if (converter == null)
            {
                throw new InvalidOperationException($"No suitable Converter found for type {newType}.");
            }

            return converter.Convert(value);
        }

        public static object ConvertStringToNewNonNullableType(this string value, Type newType)
        {
            // Do conversion form string to array - not sure how array will be stored in string
            if (newType.IsArray)
            {
                // For comma separated list
                var singleItemType = newType.GetElementType();

                var elements = new ArrayList();

                foreach (var element in value.Split(','))
                {
                    var convertedSingleItem = element.ConvertSingleItem(singleItemType);
                    elements.Add(convertedSingleItem);
                }

                return elements.ToArray(singleItemType);
            }

            return value.ConvertSingleItem(newType);
        }

        public static object ConvertStringToNewType(this string value, Type newType)
        {
            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType
            if (newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }

                return value.ConvertStringToNewNonNullableType(new NullableConverter(newType).UnderlyingType);
            }

            return value.ConvertStringToNewNonNullableType(newType);
        }
    }
}
