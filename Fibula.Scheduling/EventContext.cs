// -----------------------------------------------------------------
// <copyright file="EventContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling
{
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents an event context.
    /// </summary>
    public class EventContext : IEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventContext"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public EventContext(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }
    }
}
