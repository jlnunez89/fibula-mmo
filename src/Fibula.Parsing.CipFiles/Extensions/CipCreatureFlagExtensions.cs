// -----------------------------------------------------------------
// <copyright file="CipCreatureFlagExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Extensions
{
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Parsing.CipFiles.Enumerations;

    /// <summary>
    /// Helper class that contains extension methods for <see cref="CipCreatureFlag"/>s.
    /// </summary>
    public static class CipCreatureFlagExtensions
    {
        /// <summary>
        /// Converts a <see cref="CipCreatureFlag"/> to an <see cref="CreatureFlag"/>.
        /// </summary>
        /// <param name="cipFlag">The CIP creature flag to convert.</param>
        /// <returns>The <see cref="CreatureFlag"/> picked, if any.</returns>
        public static CreatureFlag? ToCreatureFlag(this CipCreatureFlag cipFlag)
        {
            return cipFlag switch
            {
                CipCreatureFlag.Unpushable => CreatureFlag.CannotBePushed,
                CipCreatureFlag.KickBoxes => CreatureFlag.CannotBePushed,
                CipCreatureFlag.KickCreatures => CreatureFlag.CannotBePushed,
                CipCreatureFlag.NoHit => CreatureFlag.CannotBeTargetted,
                CipCreatureFlag.DistanceFighting => CreatureFlag.KeepsDistance,
                _ => null,
            };
        }
    }
}
