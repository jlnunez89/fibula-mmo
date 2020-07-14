// -----------------------------------------------------------------
// <copyright file="MonsterDbFileMonsterSpawnLoaderOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.MonstersDbFile
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="MonsterDbFileMonsterSpawnLoader"/>.
    /// </summary>
    public class MonsterDbFileMonsterSpawnLoaderOptions
    {
        /// <summary>
        /// Gets or sets the path to the file to load.
        /// </summary>
        [Required(ErrorMessage = "A path for the monster spawns must be specified.")]
        public string FilePath { get; set; }
    }
}
