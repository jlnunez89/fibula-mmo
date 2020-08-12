// -----------------------------------------------------------------
// <copyright file="SectorMapLoaderOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.SectorFiles
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="SectorMapLoader"/>.
    /// </summary>
    public class SectorMapLoaderOptions
    {
        /// <summary>
        /// Gets or sets the directory for the live map.
        /// </summary>
        [Required(ErrorMessage = "A directory for the live map sector files must be specified.")]
        public string LiveMapDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory for the original map.
        /// </summary>
        [Required(ErrorMessage = "A directory for the original map sector files must be specified.")]
        public string OriginalMapDirectory { get; set; }
    }
}
