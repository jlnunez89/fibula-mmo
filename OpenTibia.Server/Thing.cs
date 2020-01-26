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
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
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
        /// Holds this thing's parent cylinder.
        /// </summary>
        private ICylinder parentCylinder;

        /// <summary>
        /// Event to invoke when any of the properties of this thing have changed.
        /// </summary>
        public event OnThingStateChanged OnThingChanged;

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
        public ICylinder ParentCylinder
        {
            get
            {
                return this.parentCylinder;
            }

            set
            {
                var oldLocation = this.Location;

                this.parentCylinder = value;

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
                return this.ParentCylinder?.Location ?? default;
            }
        }

        /// <summary>
        /// Gets the location where this thing is being carried at, if any.
        /// </summary>
        public abstract Location? CarryLocation { get; }

        /// <summary>
        /// Gets this entity's cylinder hierarchy.
        /// </summary>
        /// <param name="includeTiles">Optional. A value indicating whether to include <see cref="ITile"/>s in the hierarchy. Defaults to true.</param>
        /// <returns>The ordered collection of <see cref="ICylinder"/>s in this thing's cylinder hierarchy.</returns>
        public IEnumerable<ICylinder> GetCylinderHierarchy(bool includeTiles = true)
        {
            ICylinder current = (this is ICylinder thisAsCylinder) ? thisAsCylinder : this.ParentCylinder;

            while (current != null)
            {
                yield return current;

                current = (includeTiles || !(current.ParentCylinder is ITile)) ? current.ParentCylinder : null;
            }
        }

        /// <summary>
        /// Invokes the <see cref="OnThingChanged"/> event on this thing.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public void InvokePropertyChanged(string propertyName)
        {
            propertyName.ThrowIfNullOrWhiteSpace(propertyName);

            this.OnThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = propertyName });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.DescribeForLogger();
        }

        /// <summary>
        /// Gets the description of the thing as seen by the given player.
        /// </summary>
        /// <param name="forPlayer">The player as which to get the description.</param>
        /// <returns>The description string.</returns>
        public abstract string GetDescription(IPlayer forPlayer);

        /// <summary>
        /// Provides a string describing the current thing for logging purposes.
        /// </summary>
        /// <returns>The string to log.</returns>
        public abstract string DescribeForLogger();
    }
}