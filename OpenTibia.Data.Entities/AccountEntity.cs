// -----------------------------------------------------------------
// <copyright file="AccountEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities
{
    using System;
    using OpenTibia.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a character entity.
    /// </summary>
    public class AccountEntity : BaseEntity, IAccountEntity
    {
        /// <summary>
        /// Gets or sets the number for this account.
        /// </summary>
        public uint Number { get; set; }

        /// <summary>
        /// Gets or sets the password on this account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email on file for this account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the account's creation date and time.
        /// </summary>
        public DateTimeOffset Creation { get; set; }

        /// <summary>
        /// Gets or sets the IP address that created this account.
        /// </summary>
        public string CreationIp { get; set; }

        /// <summary>
        /// Gets or sets the last IP address that successfully connected to this account.
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// Gets or sets the account's last successfully login date and time.
        /// </summary>
        public DateTimeOffset LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the current IP address in use for this account, if any.
        /// </summary>
        public string SessionIp { get; set; }

        /// <summary>
        /// Gets or sets the session key in use for this account, if any.
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// Gets or sets the number of premium days left on the account.
        /// </summary>
        public ushort PremiumDays { get; set; }

        /// <summary>
        /// Gets or sets the number of trial/bonus premium days left on the account.
        /// </summary>
        public ushort TrialOrBonusPremiumDays { get; set; }

        /// <summary>
        /// Gets or sets the access level on the account.
        /// </summary>
        public byte AccessLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this account is on premium status.
        /// </summary>
        public bool Premium { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this account is on premium status but only because of bonus/trial days.
        /// </summary>
        public bool TrialPremium { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this account is currently banished.
        /// </summary>
        public bool Banished { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the banishment on this accont lasts until.
        /// </summary>
        public DateTimeOffset BanishedUntil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this account was deleted.
        /// </summary>
        public bool Deleted { get; set; }
    }
}