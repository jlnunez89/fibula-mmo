// -----------------------------------------------------------------
// <copyright file="CanDressThingAtTargetSlotEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Conditions
{
    using System;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a thing can be dressed at the target slot.
    /// </summary>
    public class CanDressThingAtTargetSlotEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanDressThingAtTargetSlotEventCondition"/> class.
        /// </summary>
        /// <param name="determineTargetCreatureFunc">A function delegate to determine the target creature.</param>
        /// <param name="movingThing">The thing to check.</param>
        /// <param name="slot">The slot to check.</param>
        public CanDressThingAtTargetSlotEventCondition(Func<ICreature> determineTargetCreatureFunc, IThing movingThing, Slot slot)
        {
            determineTargetCreatureFunc.ThrowIfNull(nameof(determineTargetCreatureFunc));

            this.GetTargetCreature = determineTargetCreatureFunc;
            this.Thing = movingThing;
            this.TargetSlot = slot;
        }

        /// <summary>
        /// Gets a delegate function to determine the <see cref="ICreature"/> to check.
        /// </summary>
        public Func<ICreature> GetTargetCreature { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being checked.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the slot being checked.
        /// </summary>
        public Slot TargetSlot { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You cannot dress this in that way.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (!(this.GetTargetCreature() is IPlayer player) || this.Thing == null || !(this.Thing is IItem item))
            {
                return false;
            }

            switch (this.TargetSlot)
            {
                // Not valid targets
                default:
                case Slot.UnsetInvalid:
                case Slot.Anywhere:
                    return false;

                // Valid target, wildcard slot
                case Slot.Ammo:
                    return true;

                // Valid target, straightforward slots
                case Slot.Head:
                case Slot.Neck:
                case Slot.Back:
                case Slot.Body:
                case Slot.Legs:
                case Slot.Ring:
                case Slot.Feet:
                    return item.CanBeDressed && item.DressPosition == this.TargetSlot;

                // Valid target, special slots
                case Slot.LeftHand:
                    if (!(player.Inventory[(byte)Slot.RightHand] is IContainerItem rightHandContainer))
                    {
                        return false;
                    }

                    var rightHandItem = rightHandContainer.Content.FirstOrDefault();

                    return rightHandItem == null || (item.DressPosition != Slot.TwoHanded && rightHandItem.DressPosition != Slot.TwoHanded);

                case Slot.RightHand:
                    if (!(player.Inventory[(byte)Slot.LeftHand] is IContainerItem leftHandContainer))
                    {
                        return false;
                    }

                    var leftHandItem = leftHandContainer.Content.FirstOrDefault();

                    return leftHandItem == null || (item.DressPosition != Slot.TwoHanded && leftHandItem.DressPosition != Slot.TwoHanded);
            }
        }
    }
}