// -----------------------------------------------------------------
// <copyright file="ISeparationEventRule.cs" company="2Dudes">
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
    /// Interface for separation event rules.
    /// </summary>
    public interface ISeparationEventRule : IEventRule
    {
        /// <summary>
        /// Gets the id of the thing involved in the separation.
        /// </summary>
        ushort SeparatingThingId { get; }
    }
}