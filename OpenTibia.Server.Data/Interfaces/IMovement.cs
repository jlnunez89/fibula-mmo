using System.Collections.Generic;

namespace OpenTibia.Server.Data.Interfaces
{
    public enum MovementState
    {
        Requested,
        Evaluated,
        Performed
    }

    public interface IMovement
    {
        uint RequestorId { get; }

        bool CanBePerformed { get; }

        string LastError { get; }

        IList<IMovementPolicy> Policies { get; }

        void Perform();
    }
}
