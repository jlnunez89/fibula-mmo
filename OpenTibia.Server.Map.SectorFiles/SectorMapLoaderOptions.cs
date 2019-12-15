// -----------------------------------------------------------------
// <copyright file="SectorMapLoaderOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Map.SectorFiles
{
    /// <summary>
    /// Class that represents options for the <see cref="SectorMapLoader"/>.
    /// </summary>
    public class SectorMapLoaderOptions
    {
        /// <summary>
        /// Gets or sets the directory for the live map.
        /// </summary>
        public string LiveMapDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory for the original map.
        /// </summary>
        public string OriginalMapDirectory { get; set; }
    }
}
