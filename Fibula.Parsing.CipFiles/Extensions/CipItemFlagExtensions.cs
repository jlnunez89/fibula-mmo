// -----------------------------------------------------------------
// <copyright file="CipItemFlagExtensions.cs" company="2Dudes">
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
    /// Helper class that contains extension methods for <see cref="CipItemFlag"/>s.
    /// </summary>
    public static class CipItemFlagExtensions
    {
        /// <summary>
        /// Converts a <see cref="CipItemFlag"/> to an <see cref="ItemFlag"/>.
        /// </summary>
        /// <param name="cipFlag">The CIP item flag to convert.</param>
        /// <returns>The <see cref="ItemFlag"/> picked, if any.</returns>
        public static ItemFlag? ToItemFlag(this CipItemFlag cipFlag)
        {
            return cipFlag switch
            {
                CipItemFlag.Bank => ItemFlag.IsGround,
                CipItemFlag.Top => ItemFlag.StaysOnTop,
                CipItemFlag.Bottom => ItemFlag.StaysOnBottom,
                CipItemFlag.Clip => ItemFlag.IsGroundBorder,
                CipItemFlag.Hang => ItemFlag.IsHung,
                CipItemFlag.Unmove => ItemFlag.IsUnmoveable,
                CipItemFlag.Unpass => ItemFlag.BlocksWalk,
                CipItemFlag.Unlay => ItemFlag.BlocksLay,
                CipItemFlag.Unthrow => ItemFlag.BlocksThrow,
                CipItemFlag.Take => ItemFlag.CanBePickedUp,
                CipItemFlag.Chest => ItemFlag.IsQuestChest,
                CipItemFlag.Container => ItemFlag.IsContainer,
                CipItemFlag.Disguise => ItemFlag.IsDisguised,
                CipItemFlag.Cumulative => ItemFlag.IsCumulative,
                CipItemFlag.Expire => ItemFlag.HasExpiration,
                CipItemFlag.ExpireStop => ItemFlag.HasToggledExpiration,
                CipItemFlag.LiquidPool => ItemFlag.IsLiquidPool,
                CipItemFlag.LiquidContainer => ItemFlag.IsLiquidContainer,
                CipItemFlag.LiquidSource => ItemFlag.IsLiquidSource,
                _ => null,
            };
        }
    }
}
