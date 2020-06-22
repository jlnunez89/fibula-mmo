// -----------------------------------------------------------------
// <copyright file="IThingMovementEventRule.cs" company="2Dudes">
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
    /// Interface for thing movement event rules.
    /// </summary>
    public interface IThingMovementEventRule : IEventRule
    {
        ///// <summary>
        ///// Gets the id of the thing involved in the movement.
        ///// </summary>
        //ushort ThingId { get; }
    }
}