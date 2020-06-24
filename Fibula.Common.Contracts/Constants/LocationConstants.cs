// -----------------------------------------------------------------
// <copyright file="LocationConstants.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Constants
{
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Static class that containts constants related to locations.
    /// </summary>
    public static class LocationConstants
    {
        /// <summary>
        /// The bit flag for containers encoded in location.
        /// </summary>
        public const int ContainerFlag = 0x40;

        /// <summary>
        /// A value for X that denotes a location type other than <see cref="LocationType.Map"/>.
        /// </summary>
        public const int NonMapLocationX = 0xFFFF;
    }
}
