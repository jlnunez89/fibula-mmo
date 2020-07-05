// -----------------------------------------------------------------
// <copyright file="ItemFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        /// An item that is an aesthetic fix to the ground tile.
        /// </summary>
        IsGroundFix,

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
