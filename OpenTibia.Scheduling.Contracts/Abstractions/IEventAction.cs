// -----------------------------------------------------------------
// <copyright file="IEventAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Contracts.Abstractions
{
    /// <summary>
    /// Interface for event actions.
    /// </summary>
    public interface IEventAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        void Execute();
    }
}