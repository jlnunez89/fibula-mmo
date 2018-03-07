using System;
using System.Collections.Generic;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement
{
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

                if (!Force)
                {
                    foreach (var policy in Policies)
                    {
                        allPassed &= policy.Evaluate();

                        if (!allPassed)
                        {
                            Console.WriteLine($"Failed movement policy {policy.GetType().Name}.");
                            LastError = policy.ErrorMessage;
                            break;
                        }
                    }
                }

                return allPassed;
            }
        }

        protected MovementBase(uint requestorId)
        {
            RequestorId = requestorId; // allowed to be null.
            Policies = new List<IMovementPolicy>();
        }
        
        public abstract void Perform();
    }
}
