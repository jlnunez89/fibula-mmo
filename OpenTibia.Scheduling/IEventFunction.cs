// <copyright file="IEventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for event functions.
    /// </summary>
    public interface IEventFunction
    {
        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a dictionary of <see cref="IEventArgument"/> parameters indexed by their <see cref="string"/> names as the keys.
        /// </summary>
        IDictionary<string, IEventArgument> Parameters { get; }
    }
}