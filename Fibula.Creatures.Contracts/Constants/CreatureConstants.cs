// -----------------------------------------------------------------
// <copyright file="CreatureConstants.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Constants
{
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Static class that contains contants for <see cref="ICreature"/> derived clases.
    /// </summary>
    public static class CreatureConstants
    {
        /// <summary>
        /// The id for things that are creatures.
        /// </summary>
        public const ushort CreatureThingId = 0x63;
    }
}
