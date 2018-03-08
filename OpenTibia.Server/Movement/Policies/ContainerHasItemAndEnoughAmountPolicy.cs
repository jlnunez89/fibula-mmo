// <copyright file="ContainerHasItemAndEnoughAmountPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;

    internal class ContainerHasItemAndEnoughAmountPolicy : IMovementPolicy
    {
        public IItem ItemToCheck { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public byte Count { get; }

        public string ErrorMessage => "There is not enough quantity.";

        public ContainerHasItemAndEnoughAmountPolicy(IItem itemToCheck, IContainer fromContainer, byte indexToCheck, byte countToCheck)
        {
            this.ItemToCheck = itemToCheck;
            this.FromContainer = fromContainer;
            this.FromIndex = indexToCheck;
            this.Count = countToCheck;
        }

        public bool Evaluate()
        {
            if (this.ItemToCheck == null || this.FromContainer == null)
            {
                return false;
            }

            return this.FromContainer.CountContentAmountAt(this.FromIndex, this.ItemToCheck.Type.TypeId) >= this.Count;
        }
    }
}