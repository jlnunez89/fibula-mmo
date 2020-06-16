// -----------------------------------------------------------------
// <copyright file="WorldInformation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server
{
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Contracts.Enumerations;

    internal class WorldInformation : IWorldInformation
    {
        public WorldState Status { get; set; }

        public byte LightColor { get; set; }

        public byte LightLevel { get; set; }
    }
}