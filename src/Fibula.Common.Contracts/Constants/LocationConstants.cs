// -----------------------------------------------------------------
// <copyright file="LocationConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
