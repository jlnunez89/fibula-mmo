// -----------------------------------------------------------------
// <copyright file="EventContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents an event context.
    /// </summary>
    public class EventContext : IEventContext
    {
        /// <summary>
        /// The function to get current time with.
        /// </summary>
        private readonly Func<DateTimeOffset> getCurrentTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContext"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="currentTimeFunc">A functiont to get the current time.</param>
        public EventContext(ILogger logger, Func<DateTimeOffset> currentTimeFunc)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext(this.GetType());
            this.getCurrentTime = currentTimeFunc;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public DateTimeOffset CurrentTime => this.getCurrentTime();
    }
}
