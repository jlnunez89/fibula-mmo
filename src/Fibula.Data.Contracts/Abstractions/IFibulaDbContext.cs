// -----------------------------------------------------------------
// <copyright file="IFibulaDbContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Contracts.Abstractions
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Interface for the Fibula database context.
    /// </summary>
    public interface IFibulaDbContext
    {
        /// <summary>
        /// Gets this context as the <see cref="DbContext"/>. This exists for the sole purpose of forcing implentations
        /// of this interface to inherit also to <see cref="DbContext"/>.
        /// </summary>
        /// <returns>This instance casted as <see cref="DbContext"/>.</returns>
        DbContext AsDbContext();
    }
}
