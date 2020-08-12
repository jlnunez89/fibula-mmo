// -----------------------------------------------------------------
// <copyright file="IBanishmentEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities.Contracts.Abstractions
{
    using System;
    using System.Net;

    /// <summary>
    /// Interface for banishment entities.
    /// </summary>
    public interface IBanishmentEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets the id of the account to which this banishment references.
        /// </summary>
        Guid AccountId { get; }

        /// <summary>
        /// Gets the id of the gamemaster who issued this banishment.
        /// </summary>
        Guid GamemasterId { get; }

        /// <summary>
        /// Gets the IP Address that was connected to the account at the time this banisment was issued.
        /// </summary>
        IPAddress IpAddress { get; }

        /// <summary>
        /// Gets the violation description of the banishment.
        /// </summary>
        string Violation { get; }

        /// <summary>
        /// Gets the gamemaster's comment on this banishment.
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// Gets the timestamp of this banishment.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the date until which this banisment will last.
        /// </summary>
        DateTimeOffset BanishedUntil { get; }

        /// <summary>
        /// Gets the type of punishment issued.
        /// </summary>
        byte PunishmentType { get; }
    }
}
