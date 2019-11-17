// -----------------------------------------------------------------
// <copyright file="IOpenTibiaDbContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Abstractions
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Interface for the OpenTibia database context.
    /// </summary>
    public interface IOpenTibiaDbContext
    {
        /// <summary>
        /// Gets this context as the <see cref="DbContext"/>. This exists for the sole purpose of forcing implentations
        /// of this interface to inherit also to <see cref="DbContext"/>.
        /// </summary>
        /// <returns>This instance casted as <see cref="DbContext"/>.</returns>
        DbContext AsDbContext();
    }
}
