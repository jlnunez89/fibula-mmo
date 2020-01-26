// -----------------------------------------------------------------
// <copyright file="OnContentUpdated.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when content is updated in a container.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="updatedIndex">The index of the updated item.</param>
    /// <param name="updatedItem">The updated item.</param>
    public delegate void OnContentUpdated(IContainerItem container, byte updatedIndex, IItem updatedItem);
}
