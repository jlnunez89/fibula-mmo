// -----------------------------------------------------------------
// <copyright file="AnimatedEffectNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Notifications.Arguments
{
    using System;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Mechanics.Contracts.Enumerations;
    using Fibula.Server.Notifications.Contracts.Abstractions;

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