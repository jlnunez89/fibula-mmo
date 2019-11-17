// <copyright file="IDebugAssertionInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Interface for debug assertion information.
    /// </summary>
    public interface IDebugAssertionInfo
    {
        /// <summary>
        /// Gets the line of the assertion.
        /// </summary>
        string AssertionLine { get; }

        /// <summary>
        /// Gets the date of the assertion.
        /// </summary>
        string Date { get; }

        /// <summary>
        /// Gets the description of the assertion.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the comment on the assertion.
        /// </summary>
        string Comment { get; }
    }
}