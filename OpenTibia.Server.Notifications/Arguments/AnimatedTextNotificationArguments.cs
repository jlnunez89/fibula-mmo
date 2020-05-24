// -----------------------------------------------------------------
// <copyright file="AnimatedTextNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications.Arguments
{
    using System;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// public class that represents arguments for an animated text notification.
    /// </summary>
    public class AnimatedTextNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedTextNotificationArguments"/> class.
        /// </summary>
        /// <param name="location">The location of the animated text.</param>
        /// <param name="color">The color of text to send.</param>
        /// <param name="text">The text to send.</param>
        public AnimatedTextNotificationArguments(Location location, TextColor color, string text)
        {
            if (color == TextColor.None)
            {
                throw new ArgumentException($"Invalid value for animated text parameter: {color}.", nameof(color));
            }

            this.Location = location;
            this.Color = color;
            this.Text = text;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the text color.
        /// </summary>
        public TextColor Color { get; }

        /// <summary>
        /// Gets the text value.
        /// </summary>
        public string Text { get; }
    }
}