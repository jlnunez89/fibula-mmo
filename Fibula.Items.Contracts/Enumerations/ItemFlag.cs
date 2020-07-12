// -----------------------------------------------------------------
// <copyright file="ItemFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates all known item flags.
    /// </summary>
    public enum ItemFlag
    {
        /// <summary>
        /// An item that blocks other items to be laid on top of it.
        /// </summary>
        BlocksLay,

        /// <summary>
        /// An item that blocks walking on it.
        /// </summary>
        BlocksWalk,

        /// <summary>
        /// An item that blocks throwing or projectiles throught it.
        /// </summary>
        BlocksThrow,

        /// <summary>
        /// An item that can be picked up.
        /// </summary>
        CanBePickedUp,

        /// <summary>
        /// An item that can be rotated.
        /// </summary>
        CanBeRotated,

        /// <summary>
        /// An item that changes when used.
        /// </summary>
        ChangesOnUse,

        /// <summary>
        /// An item that expires starting from creation.
        /// </summary>
        HasExpiration,

        /// <summary>
        /// An item that expires but expiration toggles.
        /// </summary>
        HasToggledExpiration,

        /// <summary>
        /// An item that is a ground border.
        /// </summary>
        IsGroundBorder,

        /// <summary>
        /// An item that is a container.
        /// </summary>
        IsContainer,

        /// <summary>
        /// An item that is cumulative.
        /// </summary>
        IsCumulative,

        /// <summary>
        /// An item that disguises as another.
        /// </summary>
        IsDisguised,

        /// <summary>
        /// An item that is dressable.
        /// </summary>
        IsDressable,

        /// <summary>
        /// An item that is ground.
        /// </summary>
        IsGround,

        /// <summary>
        /// An item that is a container for liquids.
        /// </summary>
        IsLiquidContainer,

        /// <summary>
        /// An item that is a pool of liquid.
        /// </summary>
        IsLiquidPool,

        /// <summary>
        /// An item that is a source of a certain liquid.
        /// </summary>
        IsLiquidSource,

        /// <summary>
        /// An item that contains text and is readable.
        /// </summary>
        IsReadable,

        /// <summary>
        /// An item that is a quest chest.
        /// </summary>
        IsQuestChest,

        /// <summary>
        /// An item that cannot be moved.
        /// </summary>
        IsUnmoveable,

        /// <summary>
        /// And item that is hung.
        /// </summary>
        IsHung,

        /// <summary>
        /// An item that should be avoided by certain damage types, but does not actually <see cref="BlocksWalk"/>.
        /// </summary>
        ShouldBeAvoided,

        /// <summary>
        /// An item that stays on bottom of the stack.
        /// </summary>
        StaysOnBottom,

        /// <summary>
        /// An item that stays on top of the stack.
        /// </summary>
        StaysOnTop,

        /// <summary>
        /// An item that triggers collision.
        /// </summary>
        TriggersCollision,

        /// <summary>
        /// An item that triggers separation.
        /// </summary>
        TriggersSeparation,
    }
}
