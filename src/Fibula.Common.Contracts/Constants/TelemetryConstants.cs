// -----------------------------------------------------------------
// <copyright file="TelemetryConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Constants
{
    /// <summary>
    /// Static class that contains names for different metrics.
    /// </summary>
    public static class TelemetryConstants
    {
        /// <summary>
        /// The name of the metric for the number of online players.
        /// </summary>
        public const string OnlinePlayersMetricName = "OnlinePlayers";

        /// <summary>
        /// The name of the metric for processed events.
        /// </summary>
        public const string ProcessedEventTimeMetricName = "ProcessedEventTimeMs";

        /// <summary>
        /// The name of the metric for the queue size of the scheduler.
        /// </summary>
        public const string SchedulerQueueSizeMetricName = "SchedulerQueueSize";

        /// <summary>
        /// The name of the metric for when a window of map tiles is loaded.
        /// </summary>
        public const string MapTilesLoadedMetricName = "MapTilesLoaded";

        /// <summary>
        /// The name of the dimension for event types.
        /// </summary>
        public const string EventTypeDimensionName = "EventType";
    }
}
