// <copyright file="GrabberHasEnoughCarryStrengthPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;

    internal class GrabberHasEnoughCarryStrengthPolicy : IMovementPolicy
    {
        public IThing ThingPicking { get; }

        public IThing ThingDropping { get; }

        public uint PickerId { get; }

        public GrabberHasEnoughCarryStrengthPolicy(uint pickerId, IThing thingPicking, IThing thingDropping = null)
        {
            this.PickerId = pickerId;
            this.ThingPicking = thingPicking;
            this.ThingDropping = thingDropping is IContainer ? null : thingDropping; // We're actually trying to put this item in, so no dropping is happening.
        }

        public string ErrorMessage => "The object is too heavy.";

        public bool Evaluate()
        {
            var itemBeingPicked = this.ThingPicking as IItem;
            var itemBeingDropped = this.ThingDropping as IItem;
            var picker = this.PickerId == 0 ? null : Game.Instance.GetCreatureWithId(this.PickerId);

            if (itemBeingPicked == null || picker == null)
            {
                return false;
            }

            // TODO: PlayerId special access check
            return // (this.PlayerId is IPlayer && (this.PlayerId as IPlayer).AccessLevel > 0) ||
                picker.CarryStrength - itemBeingPicked.Weight + (itemBeingDropped?.Weight ?? 0) >= 0;
        }
    }
}