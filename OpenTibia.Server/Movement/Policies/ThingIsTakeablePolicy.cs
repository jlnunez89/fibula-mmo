// <copyright file="ThingIsTakeablePolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    internal class ThingIsTakeablePolicy : IMovementPolicy
    {
        public IThing Thing { get; }

        public uint GrabberId { get; }

        public string ErrorMessage => "You may not move this object.";

        public ThingIsTakeablePolicy(uint grabberId, IThing thingMoving)
        {
            this.GrabberId = grabberId;
            this.Thing = thingMoving;
        }

        public bool Evaluate()
        {
            var grabber = this.GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(this.GrabberId);

            if (grabber == null || this.Thing == null)
            {
                return false;
            }

            var item = this.Thing as IItem;

            // TODO: GrabberId access level?
            return item != null && item.Type.Flags.Contains(ItemFlag.Take);
        }
    }
}