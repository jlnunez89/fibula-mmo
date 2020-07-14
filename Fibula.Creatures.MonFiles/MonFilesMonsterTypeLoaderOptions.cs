// -----------------------------------------------------------------
// <copyright file="MonFilesMonsterTypeLoaderOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.MonFiles
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="MonFilesMonsterTypeLoader"/>.
    /// </summary>
    public class MonFilesMonsterTypeLoaderOptions
    {
        /// <summary>
        /// Gets or sets the directory for the monster (*.mon) files.
        /// </summary>
        [Required(ErrorMessage = "A directory for the monster (*.mon) files files must be specified.")]
        public string MonsterFilesDirectory { get; set; }
    }
}
