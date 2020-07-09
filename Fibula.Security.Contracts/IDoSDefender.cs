// -----------------------------------------------------------------
// <copyright file="IDoSDefender.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security.Contracts
{
    /// <summary>
    /// Interface for a DoS defender service.
    /// </summary>
    public interface IDoSDefender
    {
        /// <summary>
        /// Blocks a given address.
        /// </summary>
        /// <param name="addressStr">The address to block.</param>
        void BlockAddress(string addressStr);

        /// <summary>
        /// Checks if a given address is blocked.
        /// </summary>
        /// <param name="addressStr">The address to check for.</param>
        /// <returns>True if the address is blocked, false otherwise.</returns>
        bool IsBlocked(string addressStr);

        /// <summary>
        /// Logs a connection attempt.
        /// </summary>
        /// <param name="addressStr">The address from which the connection attempt took place.</param>
        void LogConnectionAttempt(string addressStr);
    }
}
