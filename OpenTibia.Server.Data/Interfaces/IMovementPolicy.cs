// <copyright file="IMovementPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    public interface IMovementPolicy
    {
        string ErrorMessage { get; }

        bool Evaluate();
    }
}
