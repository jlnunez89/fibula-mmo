// -----------------------------------------------------------------
// <copyright file="IOutfitInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

using Fibula.Server.Contracts.Structs;

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    public interface IOutfitInfo
    {
        Outfit Outfit { get; }
    }
}
