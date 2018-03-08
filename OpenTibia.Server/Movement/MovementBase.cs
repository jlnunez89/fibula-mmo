// <copyright file="MovementBase.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Server.Data.Interfaces;

    internal abstract class MovementBase : IMovement
    {
        public uint RequestorId { get; protected set; }

        public MovementState State { get; protected set; }

        public IList<IMovementPolicy> Policies { get; protected set; }

        public bool Force { get; protected set; }

        public string LastError { get; protected set; }

        public bool CanBePerformed
        {
            get
            {
                var allPassed = true;

                if (!this.Force)
                {
                    foreach (var policy in this.Policies)
                    {
                        allPassed &= policy.Evaluate();

                        if (!allPassed)
                        {
                            Console.WriteLine($"Failed movement policy {policy.GetType().Name}.");
                            this.LastError = policy.ErrorMessage;
                            break;
                        }
                    }
                }

                return allPassed;
            }
        }

        protected MovementBase(uint requestorId)
        {
            this.RequestorId = requestorId; // allowed to be null.
            this.Policies = new List<IMovementPolicy>();
        }

        public abstract void Perform();
    }
}
