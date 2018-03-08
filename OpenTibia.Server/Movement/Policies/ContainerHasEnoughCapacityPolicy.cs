// <copyright file="ContainerHasEnoughCapacityPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;

    internal class ContainerHasEnoughCapacityPolicy : IMovementPolicy
    {
        public IContainer TargetContainer { get; }

        public string ErrorMessage => "There is not enough room.";

        public ContainerHasEnoughCapacityPolicy(IContainer destinationContainer)
        {
            this.TargetContainer = destinationContainer;
        }

        public bool Evaluate()
        {
            return this.TargetContainer != null && this.TargetContainer?.Volume - this.TargetContainer.Content.Count > 0;
        }
    }
}