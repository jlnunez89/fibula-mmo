// -----------------------------------------------------------------
// <copyright file="TextMessageNotificationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// public class that represents arguments for an animated text notification.
    /// </summary>
    public class TextMessageNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageNotificationArguments"/> class.
        /// </summary>
        /// <param name="type">The type of text to send.</param>
        /// <param name="message">The text to send.</param>
        public TextMessageNotificationArguments(MessageType type, string message)
        {
            this.Type = type;
            this.Text = message;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the text message type.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Gets the text value.
        /// </summary>
        public string Text { get; }
    }
}