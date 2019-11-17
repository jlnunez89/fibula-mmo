// <copyright file="ISpeechInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for speech information.
    /// </summary>
    public interface ISpeechInfo
    {
        /// <summary>
        /// Gets the speech instance.
        /// </summary>
        Speech Speech { get; }
    }
}