// -----------------------------------------------------------------
// <copyright file="CipItemAttributeExtensions.cs" company="2Dudes">
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
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Parsing.CipFiles.Enumerations;

    /// <summary>
    /// Helper class that contains extension methods for <see cref="CipItemAttribute"/>s.
    /// </summary>
    public static class CipItemAttributeExtensions
    {
        /// <summary>
        /// Converts a <see cref="CipItemAttribute"/> to an <see cref="ItemAttribute"/>.
        /// </summary>
        /// <param name="cipAttribute">The Cip item attribute to convert.</param>
        /// <returns>The <see cref="ItemAttribute"/> picked, if any.</returns>
        public static ItemAttribute? ToItemAttribute(this CipItemAttribute cipAttribute)
        {
            // We need to support liquid types at all times because the client will error if we don't.
            // So even if not implemented yet or anything, send something.
            return cipAttribute switch
            {
                CipItemAttribute.Amount => ItemAttribute.Amount,
                CipItemAttribute.DisguiseTarget => ItemAttribute.DisguiseAs,
                CipItemAttribute.Waypoints => ItemAttribute.MovementPenalty,
                CipItemAttribute.ContainerLiquidType => ItemAttribute.LiquidType,
                CipItemAttribute.PoolLiquidType => ItemAttribute.LiquidType,
                CipItemAttribute.SourceLiquidType => ItemAttribute.LiquidType,
                _ => null,
            };
        }
    }
}
