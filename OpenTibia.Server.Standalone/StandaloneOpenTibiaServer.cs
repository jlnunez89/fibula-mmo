// <copyright file="StandaloneOpenTibiaServer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Standalone
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Class that represents a standalone OpenTibia server.
    /// </summary>
    public class StandaloneOpenTibiaServer
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The arguments for this program.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
                    configHost.AddEnvironmentVariables(prefix: "OTS_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configApp.AddEnvironmentVariables(prefix: "OTS_");
                    configApp.AddCommandLine(args);
                })
                .Build();

            await host.RunAsync(cancellationTokenSource.Token);
        }
    }
}
