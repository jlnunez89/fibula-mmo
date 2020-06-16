// -----------------------------------------------------------------
// <copyright file="OnContentUpdated.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Delegates
{
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when content is updated in a container.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="updatedIndex">The index of the updated item.</param>
    /// <param name="updatedItem">The updated item.</param>
    public delegate void OnContentUpdated(IContainerItem container, byte updatedIndex, IItem updatedItem);
}
