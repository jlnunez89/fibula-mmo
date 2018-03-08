// <copyright file="GrabberHasContainerOpenPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;

    internal class GrabberHasContainerOpenPolicy : IMovementPolicy
    {
        public IContainer TargetContainer { get; }

        public uint GrabberId { get; }

        public GrabberHasContainerOpenPolicy(uint grabberId, IContainer destinationContainer)
        {
            this.GrabberId = grabberId;
            this.TargetContainer = destinationContainer;
        }

        public string ErrorMessage => "Sorry, not possible.";

        public bool Evaluate()
        {
            var grabber = this.GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(this.GrabberId);

            if (grabber == null || this.TargetContainer == null)
            {
                return false;
            }

            return !(grabber is IPlayer) || (grabber as IPlayer)?.GetContainerId(this.TargetContainer) >= 0;
        }
    }
}