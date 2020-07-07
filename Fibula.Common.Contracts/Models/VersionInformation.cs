// -----------------------------------------------------------------
// <copyright file="VersionInformation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
