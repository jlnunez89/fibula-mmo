// -----------------------------------------------------------------
// <copyright file="ItemConstants.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Constants
{
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Static class that contains constants regarding items.
    /// </summary>
    public static class ItemConstants
    {
        /// <summary>
        /// The maximum amount value of cummulative items.
        /// </summary>
        public const byte MaximumAmountOfCummulativeItems = 100;

        /// <summary>
        /// The position to use for unset or new containers.
        /// </summary>
        public const byte UnsetContainerPosition = 0xFF;

        /// <summary>
        /// The maximum number of containers to maintain per creature.
        /// </summary>
        public const int MaxContainersPerCreature = 16;

        /// <summary>
        /// The id of the type for a blood pool on the floor.
        /// </summary>
        // TODO: this id is version specific (7.72), and should be moved to somewhwere else.
        public const ushort BloodPoolTypeId = 2886;

        /// <summary>
        /// The id of the type for a blood splatter on the floor.
        /// </summary>
        // TODO: this id is version specific (7.72), and should be moved to somewhwere else.
        public const ushort BloodSplatterTypeId = 2889;
    }
}
