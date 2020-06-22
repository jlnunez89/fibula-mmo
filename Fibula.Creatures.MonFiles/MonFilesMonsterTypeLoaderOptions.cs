// -----------------------------------------------------------------
// <copyright file="MonFilesMonsterTypeLoaderOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

using OpenTibia.Server.Monsters.MonFiles;

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
