// <copyright file="SlotContainsItemAndCountPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class SlotContainsItemAndCountPolicy : IMovementPolicy
    {
        public uint RequestorId { get; }

        public IItem ItemMoving { get; }

        public byte Slot { get; }

        public byte Count { get; }

        public string ErrorMessage => "You are too far away.";

        public SlotContainsItemAndCountPolicy(uint requestorId, IItem movingItem, byte slot, byte count = 1)
        {
            this.RequestorId = requestorId;
            this.ItemMoving = movingItem;
            this.Slot = slot;
            this.Count = count;
        }

        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null || this.ItemMoving == null)
            {
                return false;
            }

            var itemAtSlot = requestor.Inventory?[this.Slot];

            return itemAtSlot != null && this.ItemMoving.Type.TypeId == itemAtSlot.Type.TypeId && itemAtSlot.Count >= this.Count;
        }
    }
}