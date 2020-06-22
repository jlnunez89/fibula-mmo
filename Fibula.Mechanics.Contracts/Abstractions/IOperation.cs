// -----------------------------------------------------------------
// <copyright file="IOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for a game operation.
    /// </summary>
    public interface IOperation : IEvent
    {
        ///// <summary>
        ///// Gets the type of exhaustion that this operation produces.
        ///// </summary>
        //ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets the exhaustion cost of this operation.
        /// </summary>
        TimeSpan ExhaustionCost { get; }
    }
}
