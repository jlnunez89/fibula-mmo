// <copyright file="IMovement.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System.Collections.Generic;

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
