// -----------------------------------------------------------------
// <copyright file="IEventContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Abstractions
{
    using Serilog;

    /// <summary>
    /// Interface for an operation context.
    /// </summary>
    public interface IEventContext
    {
        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        ILogger Logger { get; }
    }
}
