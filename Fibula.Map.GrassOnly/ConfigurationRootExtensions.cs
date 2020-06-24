// -----------------------------------------------------------------
// <copyright file="ConfigurationRootExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.GrassOnly
{
    using Fibula.Common.Utilities;
    using Fibula.Map.Contracts.Abstractions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class ConfigurationRootExtensions
    {
        /// <summary>
        /// Adds all implementations related to a dummy map loader that gives only grass tiles contained in this library to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration reference.</param>
        public static void AddGrassOnlyDummyMapLoader(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            services.AddSingleton<IMapLoader, GrassOnlyDummyMapLoader>();
        }
    }
}
