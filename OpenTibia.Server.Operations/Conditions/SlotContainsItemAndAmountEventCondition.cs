// -----------------------------------------------------------------
// <copyright file="SlotContainsItemAndAmountEventCondition.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a slot contains an item and enough amount of it.
    /// </summary>
    public class SlotContainsItemAndAmountEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlotContainsItemAndAmountEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="requestorId">The id of the requesting creature.</param>
        /// <param name="movingItem">The item to check.</param>
        /// <param name="slot">The slot to check.</param>
        /// <param name="amount">The amount to check for.</param>
        public SlotContainsItemAndAmountEventCondition(ICreatureFinder creatureFinder, uint requestorId, IItem movingItem, byte slot, byte amount = 1)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.RequestorId = requestorId;
            this.ItemMoving = movingItem;
            this.Slot = (Slot)slot;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the id of the requesting creature.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the <see cref="IItem"/> being checked.
        /// </summary>
        public IItem ItemMoving { get; }

        /// <summary>
        /// Gets the slot being checked.
        /// </summary>
        public Slot Slot { get; }

        /// <summary>
        /// Gets the amount expected.
        /// </summary>
        public byte Amount { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You are too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            if (requestor == null || this.ItemMoving == null)
            {
                return false;
            }

            var containerAtSlot = requestor.Inventory?[(byte)this.Slot] as IContainerItem;
            var itemAtSlot = containerAtSlot?.Content.FirstOrDefault();

            return itemAtSlot != null && this.ItemMoving.Type.TypeId == itemAtSlot.Type.TypeId && itemAtSlot.Amount >= this.Amount;
        }
    }
}