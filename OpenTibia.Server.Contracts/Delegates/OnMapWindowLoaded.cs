// -----------------------------------------------------------------
// <copyright file="OnMapWindowLoaded.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    /// <summary>
    /// Delegate for when a window in the map is loaded by the map loader.
    /// </summary>
    /// <param name="fromX">The start X coordinate for the load window.</param>
    /// <param name="toX">The end X coordinate for the load window.</param>
    /// <param name="fromY">The start Y coordinate for the load window.</param>
    /// <param name="toY">The end Y coordinate for the load window.</param>
    /// <param name="fromZ">The start Z coordinate for the load window.</param>
    /// <param name="toZ">The end Z coordinate for the load window.</param>
    public delegate void OnMapWindowLoaded(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ);
}
