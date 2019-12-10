// -----------------------------------------------------------------
// <copyright file="WorldLightChangedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents arguments for the world light changed notification.
    /// </summary>
    internal class WorldLightChangedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightChangedNotificationArguments"/> class.
        /// </summary>
        /// <param name="lightLevel">The new world light level.</param>
        /// <param name="lightColor">The new world light color.</param>
        public WorldLightChangedNotificationArguments(byte lightLevel, byte lightColor = (byte)LightColors.White)
        {
            this.LightLevel = lightLevel;
            this.LightColor = lightColor;
        }

        /// <summary>
        /// Gets the new world light level.
        /// </summary>
        public byte LightLevel { get; }

        /// <summary>
        /// Gets the new world light color.
        /// </summary>
        public byte LightColor { get; }
    }
}