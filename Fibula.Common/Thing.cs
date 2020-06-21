// -----------------------------------------------------------------
// <copyright file="Thing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server
{
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Delegates;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents all things in the game.
    /// </summary>
    public abstract class Thing : IThing
    {
        /// <summary>
        /// Holds this thing's parent container.
        /// </summary>
        private IThingContainer parentContainer;

        /// <summary>
        /// Event to invoke when any of the properties of this thing have changed.
        /// </summary>
        public event OnThingStateChanged ThingChanged;

        /// <summary>
        /// Gets the id of this thing.
        /// </summary>
        public abstract ushort ThingId { get; }

        /// <summary>
        /// Gets a value indicating whether this thing can be moved.
        /// </summary>
        public abstract bool CanBeMoved { get; }

        /// <summary>
        /// Gets or sets the parent cylinder of this thing.
        /// </summary>
        public IThingContainer ParentContainer
        {
            get
            {
                return this.parentContainer;
            }

            set
            {
                var oldLocation = this.Location;

                this.parentContainer = value;

                if (oldLocation != this.Location)
                {
                    // The things's location changed since the parent changed.
                    this.InvokePropertyChanged(nameof(this.Location));
                }
            }
        }

        /// <summary>
        /// Gets this thing's location.
        /// </summary>
        public virtual Location Location
        {
            get
            {
                return this.ParentContainer?.Location ?? default;
            }
        }

        /// <summary>
        /// Gets the location where this thing is being carried at, if any.
        /// </summary>
        public abstract Location? CarryLocation { get; }

        /// <summary>
        /// Invokes the <see cref="ThingChanged"/> event on this thing.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public void InvokePropertyChanged(string propertyName)
        {
            propertyName.ThrowIfNullOrWhiteSpace(propertyName);

            this.ThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = propertyName });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.DescribeForLogger();
        }

        /// <summary>
        /// Provides a string describing the current thing for logging purposes.
        /// </summary>
        /// <returns>The string to log.</returns>
        public abstract string DescribeForLogger();
    }
}