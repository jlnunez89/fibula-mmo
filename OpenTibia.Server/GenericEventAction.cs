// -----------------------------------------------------------------
// <copyright file="GenericEventAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Internal class that represents a generic event action.
    /// </summary>
    internal class GenericEventAction : IEventAction
    {
        /// <summary>
        /// Stores the delegate method to execute.
        /// </summary>
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEventAction"/> class.
        /// </summary>
        /// <param name="action">The delegate method to execute.</param>
        public GenericEventAction(Action action)
        {
            action.ThrowIfNull();

            this.action = action;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        public void Execute()
        {
            this.action();
        }
    }
}