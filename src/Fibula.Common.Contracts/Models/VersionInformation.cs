// -----------------------------------------------------------------
// <copyright file="VersionInformation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents a version's information.
    /// </summary>
    public class VersionInformation
    {
        /// <summary>
        /// Gets or sets the numeric value of the version.
        /// </summary>
        [Required(ErrorMessage = "A numeric value for the version must be speficied.")]
        public int Numeric { get; set; }

        /// <summary>
        /// Gets or sets the description of the version.
        /// </summary>
        [Required(ErrorMessage = "A description for the version must be speficied.")]
        public string Description { get; set; }
    }
}
