// -----------------------------------------------------------------
// <copyright file="ConfigurationRootExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Security
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Security.Contracts;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class ConfigurationRootExtensions
    {
        /// <summary>
        /// Adds the <see cref="SimpleDosDefender"/> contained in this library to the services collection.
        /// It aslo configures the options required by it: <see cref="SimpleDosDefenderOptions"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddSimpleDosDefender(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            // Configure the options required by the services we're about to add.
            services.Configure<SimpleDosDefenderOptions>(configuration.GetSection(nameof(SimpleDosDefenderOptions)));

            services.TryAddSingleton<SimpleDosDefender>();
            services.TryAddSingleton<IDoSDefender>(s => s.GetService<SimpleDosDefender>());

            // Since it's derived from IHostedService should be also registered as such.
            services.AddHostedService(s => s.GetService<SimpleDosDefender>());
        }
    }
}
