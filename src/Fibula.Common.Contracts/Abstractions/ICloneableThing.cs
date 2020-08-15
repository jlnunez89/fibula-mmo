// -----------------------------------------------------------------
// <copyright file="ICloneableThing.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    /// <summary>
    /// Interface for cloneable things.
    /// </summary>
    /// <typeparam name="TThing">The type of thing being cloned.</typeparam>
    public interface ICloneableThing<out TThing>
        where TThing : IThing
    {
        /// <summary>
        /// Creates a new <typeparamref name="TThing"/> that is a shallow copy of the current instance.
        /// </summary>
        /// <returns>A new <typeparamref name="TThing"/> that is a shallow copy of this instance.</returns>
        TThing Clone();
    }
}
