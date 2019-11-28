// -----------------------------------------------------------------
// <copyright file="IAccountEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities.Contracts.Abstractions
{
    using System;

    /// <summary>
    /// Interface for account entities.
    /// </summary>
    public interface IAccountEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets the number for this account.
        /// </summary>
        uint Number { get; }

        /// <summary>
        /// Gets the password on this account.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the email on file for this account.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Gets the account's creation date and time.
        /// </summary>
        DateTimeOffset Creation { get; }

        /// <summary>
        /// Gets the IP address that created this account.
        /// </summary>
        string CreationIp { get; }

        /// <summary>
        /// Gets the account's last successfully login date and time.
        /// </summary>
        DateTimeOffset LastLogin { get; }

        /// <summary>
        /// Gets the last IP address that successfully connected to this account.
        /// </summary>
        string LastLoginIp { get; }

        /// <summary>
        /// Gets the current IP address in use for this account, if any.
        /// </summary>
        string SessionIp { get; }

        /// <summary>
        /// Gets the number of premium days left on the account.
        /// </summary>
        ushort PremiumDays { get; }

        /// <summary>
        /// Gets the number of trial/bonus premium days left on the account.
        /// </summary>
        ushort TrialOrBonusPremiumDays { get; }

        /// <summary>
        /// Gets the access level on the account.
        /// </summary>
        byte AccessLevel { get; }

        /// <summary>
        /// Gets a value indicating whether this account is on premium status.
        /// </summary>
        bool Premium { get; }

        /// <summary>
        /// Gets a value indicating whether this account is on premium status but only because of bonus/trial days.
        /// </summary>
        bool TrialPremium { get; }

        /// <summary>
        /// Gets a value indicating whether this account is currently banished.
        /// </summary>
        bool Banished { get; }

        /// <summary>
        /// Gets the date and time that the banishment on this accont lasts until.
        /// </summary>
        DateTimeOffset BanishedUntil { get; }

        /// <summary>
        /// Gets a value indicating whether this account was deleted.
        /// </summary>
        bool Deleted { get; }

        // int Lastrecover { get; }

        // short Posts { get; }

        // int Last_Post { get; }

        // byte Roses { get; }
    }
}
