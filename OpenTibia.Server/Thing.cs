// -----------------------------------------------------------------
// <copyright file="Thing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents all things in the game.
    /// </summary>
    public abstract class Thing : IThing
    {
        /// <summary>
        /// The id for things that are creatures.
        /// </summary>
        public const ushort CreatureThingId = 0x63;

        /// <summary>
        /// Holds this thing's location.
        /// </summary>
        private Location location;

        /// <summary>
        /// Event to invoke when any of the properties of this thing have changed.
        /// </summary>
        public event OnThingStateChanged OnThingChanged;

        /// <summary>
        /// Gets the id of this thing.
        /// </summary>
        public abstract ushort ThingId { get; }

        /// <summary>
        /// Gets the description of the thing.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the inspection text of the thing.
        /// </summary>
        public abstract string InspectionText { get; }

        /// <summary>
        /// Gets a value indicating whether this thing can be moved.
        /// </summary>
        public abstract bool CanBeMoved { get; }

        /// <summary>
        /// Gets or sets this thing's location.
        /// </summary>
        public Location Location
        {
            get
            {
                return this.location;
            }

            set
            {
                var oldValue = this.location;

                this.location = value;

                if (oldValue != this.location)
                {
                    this.OnThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(this.Location) });
                }
            }
        }
    }
}