// -----------------------------------------------------------------
// <copyright file="IEventRulesLoader.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for an event rules loader.
    /// </summary>
    public interface IEventRulesLoader
    {
        /// <summary>
        /// Loads all the events.
        /// </summary>
        /// <param name="gameApi">A reference to the game API.</param>
        /// <returns>A mapping between <see cref="EventRuleType"/> and a set of <see cref="IEventRule"/>s of such type.</returns>
        IDictionary<EventRuleType, ISet<IEventRule>> LoadEventRules(IGameOperationsApi gameApi);
    }
}
