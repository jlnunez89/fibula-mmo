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
        public const ushort CreatureThingId = 99;

        /// <summary>
        /// The maximum speed allowed for creatures.
        /// </summary>
        public const ushort MaxCreatureSpeed = 1500;

        /// <summary>
        /// The minimum speed allowed for creatures.
        /// </summary>
        public const ushort MinCreatureSpeed = 0;

        /// <summary>
        /// The maximum speed allowed for players.
        /// </summary>
        public const ushort MaxPlayerSpeed = MaxCreatureSpeed;

        /// <summary>
        /// The minimum speed allowed for players.
        /// </summary>
        public const ushort MinPlayerSpeed = 10;
    }
}
