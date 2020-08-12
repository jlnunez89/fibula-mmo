// -----------------------------------------------------------------
// <copyright file="IUseItemOnEventRule.cs" company="2Dudes">
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
    /// Interface for event rules for using items on something else.
    /// </summary>
    public interface IUseItemOnEventRule : IEventRule
    {
        /// <summary>
        /// Gets the id of the item to use.
        /// </summary>
        ushort ItemToUseId { get; }

        /// <summary>
        /// Gets the id of the thing to use on.
        /// </summary>
        ushort ThingToUseOnId { get; }
    }
}