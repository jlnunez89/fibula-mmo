// -----------------------------------------------------------------
// <copyright file="SimpleDosDefenderOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the login listener.
    /// </summary>
    public class SimpleDosDefenderOptions
    {
        /// <summary>
        ///  Gets or sets the maximum of IP addresses to block.
        /// </summary>
        [DefaultValue(1000000)]
        [Range(100, int.MaxValue, ErrorMessage = "The value of " + nameof(ListSizeLimit) + " must be in the range [100, 2147483647].")]
        public uint ListSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds per timeframe.
        /// </summary>
        [Required(ErrorMessage = "A time frame duration (in seconds) must be speficied.")]
        [Range(1, 3600, ErrorMessage = "The value of " + nameof(TimeframeInSeconds) + " must be in the range [1, 3600].")]
        public int TimeframeInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the count to reach before blocking an IP address, within a timeframe.
        /// </summary>
        [Required(ErrorMessage = "A value at which to block requests must be speficied.")]
        [Range(5, 10000, ErrorMessage = "The value of " + nameof(BlockAtCount) + " must be in the range [5, 10000].")]
        public uint BlockAtCount { get; set; }
    }
}
