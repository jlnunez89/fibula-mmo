// -----------------------------------------------------------------
// <copyright file="ConfigurationRootExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security
{
    using Fibula.Common.Utilities;
    using Fibula.Security.Contracts;
    using Fibula.Security.Encryption;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

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

        /// <summary>
        /// Adds the <see cref="LocalPemFileRsaDecryptor"/> contained in this library to the services collection.
        /// It aslo configures the options required by it: <see cref="LocalPemFileRsaDecryptorOptions"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration loaded.</param>
        public static void AddLocalPemFileRsaDecryptor(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            // Configure the options required by the services we're about to add.
            services.Configure<LocalPemFileRsaDecryptorOptions>(configuration.GetSection(nameof(LocalPemFileRsaDecryptorOptions)));

            services.TryAddSingleton<LocalPemFileRsaDecryptor>();
            services.TryAddSingleton<IRsaDecryptor>(s => s.GetService<LocalPemFileRsaDecryptor>());
        }
    }
}
