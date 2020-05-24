// -----------------------------------------------------------------
// <copyright file="ObjectsFileItemTypeLoaderOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Items.ObjectsFile
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="ObjectsFileItemTypeLoader"/>.
    /// </summary>
    public class ObjectsFileItemTypeLoaderOptions
    {
        /// <summary>
        /// Gets or sets the path to the file to load.
        /// </summary>
        [Required(ErrorMessage = "A path for the objects file must be specified.")]
        public string FilePath { get; set; }
    }
}
