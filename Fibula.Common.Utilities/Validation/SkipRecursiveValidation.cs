// -----------------------------------------------------------------
// <copyright file="SkipRecursiveValidation.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an <see cref="Attribute"/> which prevents a property from being validated
    /// during recursive validation.
    /// </summary>
    public class SkipRecursiveValidation : Attribute
    {
    }
}
