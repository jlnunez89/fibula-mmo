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
                value.ThrowIfNull(nameof(value));

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
        public Location Location
        {
            get
            {
                return this.ParentCylinder?.Location ?? default;
            }
        }

        /// <summary>
        /// Gets this thing's cylinder hierarchy.
        /// </summary>
        /// <returns>The ordered collection of <see cref="ICylinder"/>s in this thing's parent hierarchy.</returns>
        public IEnumerable<ICylinder> GetParentHierarchy()
        {
            ICylinder current = this.ParentCylinder;

            while (current != null)
            {
                yield return current;

                if (current is ITile)
                {
                    current = null;
                }
                else if (current is IContainerItem containerCylinder)
                {
                    current = containerCylinder.ParentCylinder;
                }
                else if (current is ICreature creatureCylinder)
                {
                    current = creatureCylinder.ParentCylinder;
                }
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
    }
}