// -----------------------------------------------------------------
// <copyright file="CreatureFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates all the different creature flags.
    /// </summary>
    public enum CreatureFlag
    {
        /// <summary>
        /// The creature attempts to clear it's path by pushing movable items or destroying them.
        /// </summary>
        CanPushItems,

        /// <summary>
        /// The creature attempts to clear it's path by pushing other creatures.
        /// </summary>
        CanPushCreatures,

        /// <summary>
        /// The creature cannot be pushed by others.
        /// </summary>
        CannotBePushed,

        /// <summary>
        /// The creature cannot be targetted for attack.
        /// </summary>
        CannotBeTargetted,

        /// <summary>
        /// The creature keeps their distance when fighting.
        /// </summary>
        // TODO: will be removed in favor of an attribute.
        KeepsDistance,
    }
}
