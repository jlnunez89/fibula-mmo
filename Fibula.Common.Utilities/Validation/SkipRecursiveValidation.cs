// -----------------------------------------------------------------
// <copyright file="SkipRecursiveValidation.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an <see cref="Attribute"/> which prevents a property from being validated
    /// during recursive validation.
    /// </summary>
    public class SkipRecursiveValidation : Attribute
    {
    }
}
