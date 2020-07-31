// -----------------------------------------------------------------
// <copyright file="OnLocationChanged.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Delegates
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Delegate for <see cref="IThing"/> state changing.
    /// </summary>
    /// <param name="thing">A reference to the <see cref="IThing"/> which's location changed.</param>
    /// <param name="previousLocation">The thing's previous location.</param>
    public delegate void OnLocationChanged(IThing thing, Location previousLocation);
}
