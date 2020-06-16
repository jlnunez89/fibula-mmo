// -----------------------------------------------------------------
// <copyright file="IEventRulesFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for event rule factories.
    /// </summary>
    public interface IEventRulesFactory
    {
        /// <summary>
        /// Creates a new item event from the given <see cref="IEventRuleCreationArguments"/>.
        /// </summary>
        /// <param name="gameApi">A reference to the game API to initialize rules with.</param>
        /// <param name="eventRuleArgs">The event rule creation arguments..</param>
        /// <returns>A new instance of a <see cref="IEventRule"/> implementation, based on the parsed event.</returns>
        IEventRule Create(IGameOperationsApi gameApi, IEventRuleCreationArguments eventRuleArgs);
    }
}
