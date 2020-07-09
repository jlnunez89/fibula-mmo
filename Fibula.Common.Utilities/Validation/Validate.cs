// -----------------------------------------------------------------
// <copyright file="Validate.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// Class that provides extension methods for common validation operations.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Throws am <see cref="ArgumentNullException"/> if the analyzed string is null or white space only.
        /// </summary>
        /// <param name="str">The string to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfNullOrWhiteSpace(this string str, string paramName = "")
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(paramName) ? nameof(str) : paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the analyzed object reference is null.
        /// </summary>
        /// <param name="o">The object reference to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfNull(this object o, string paramName = "")
        {
            if (o == null)
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(paramName) ? nameof(o) : paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the analyzed variable has the default value for it's type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IConvertible"/>.</typeparam>
        /// <param name="o">The variable to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfDefaultValue<T>(this T o, string paramName = "")
            where T : IConvertible
        {
            o.ThrowIfNull();

            if (EqualityComparer<T>.Default.Equals(o, default))
            {
                throw new ArgumentException($"Parameter {(string.IsNullOrWhiteSpace(paramName) ? nameof(o) : paramName)} has the default value.");
            }
        }
    }
}
