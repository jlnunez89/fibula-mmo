// -----------------------------------------------------------------
// <copyright file="AnimatedEffectNotificationArguments.cs" company="2Dudes">
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
    /// public class that represents arguments for an animated effect notification.
    /// </summary>
    public class AnimatedEffectNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedEffectNotificationArguments"/> class.
        /// </summary>
        /// <param name="location">The location of the animated text.</param>
        /// <param name="effect">The effect.</param>
        public AnimatedEffectNotificationArguments(Location location, AnimatedEffect effect = AnimatedEffect.None)
        {
            if (effect == AnimatedEffect.None)
            {
                throw new ArgumentException($"Invalid value for effect parameter: {effect}.", nameof(effect));
            }

            this.Location = location;
            this.Effect = effect;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the actual effect.
        /// </summary>
        public AnimatedEffect Effect { get; }
    }
}