// <copyright file="SlotHasContainerAndContainerHasEnoughCapacityPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;

    internal class SlotHasContainerAndContainerHasEnoughCapacityPolicy : IMovementPolicy
    {
        public IItem ItemInSlot { get; }

        public uint PlayerId { get; }

        public SlotHasContainerAndContainerHasEnoughCapacityPolicy(uint playerId, IItem itemInSlot)
        {
            this.PlayerId = playerId;
            this.ItemInSlot = itemInSlot;
        }

        public string ErrorMessage => "The container is full.";

        public bool Evaluate()
        {
            var player = this.PlayerId == 0 ? null : Game.Instance.GetCreatureWithId(this.PlayerId);

            if (player == null)
            {
                return false;
            }

            if (this.ItemInSlot == null)
            {
                return true;
            }

            var itemAsContainer = this.ItemInSlot as IContainer;
            return !this.ItemInSlot.IsContainer || itemAsContainer != null && itemAsContainer.Content.Count < itemAsContainer.Volume;
        }
    }
}