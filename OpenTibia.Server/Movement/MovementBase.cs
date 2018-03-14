// <copyright file="MovementBase.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using OpenTibia.Scheduling;
    using OpenTibia.Server.Data;

    internal abstract class MovementBase : BaseEvent
    {
        protected MovementBase(uint requestorId)
            : base(requestorId)
        {
        }

        public MovementState State { get; protected set; }

        public bool Force { get; protected set; }

        /// <inheritdoc/>
        public override bool CanBeExecuted
        {
            get
            {
                var allPassed = true;

                if (!this.Force)
                {
                    foreach (var policy in this.Conditions)
                    {
                        allPassed &= policy.Evaluate();

                        if (!allPassed)
                        {
                            Console.WriteLine($"Failed movement policy {policy.GetType().Name}.");
                            this.ErrorMessage = policy.ErrorMessage;
                            break;
                        }
                    }
                }

                return allPassed;
            }
        }
    }
}
