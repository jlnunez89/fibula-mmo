// -----------------------------------------------------------------
// <copyright file="OnContentAdded.cs" company="2Dudes">
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
    /// Delegate meant for when content is added to a container.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="addedItem">The added item.</param>
    public delegate void OnContentAdded(IContainerItem container, IItem addedItem);
}
