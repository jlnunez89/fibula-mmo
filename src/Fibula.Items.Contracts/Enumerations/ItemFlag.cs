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
    public enum ItemFlag : ulong
    {
        /// <summary>
        /// An item that blocks other items to be laid on top of it.
        /// </summary>
        BlocksLay = 1 << 1,

        /// <summary>
        /// An item that blocks walking on it.
        /// </summary>
        BlocksWalk = 1 << 2,

        /// <summary>
        /// An item that blocks throwing or projectiles throught it.
        /// </summary>
        BlocksThrow = 1 << 3,

        /// <summary>
        /// An item that can be picked up.
        /// </summary>
        CanBePickedUp = 1 << 4,

        /// <summary>
        /// An item that can be rotated.
        /// </summary>
        CanBeRotated = 1 << 5,

        /// <summary>
        /// An item that changes when used.
        /// </summary>
        ChangesOnUse = 1 << 6,

        /// <summary>
        /// An item that expires starting from creation.
        /// </summary>
        HasExpiration = 1 << 7,

        /// <summary>
        /// An item that expires but expiration toggles.
        /// </summary>
        HasToggledExpiration = 1 << 8,

        /// <summary>
        /// An item that is a ground border.
        /// </summary>
        IsGroundBorder = 1 << 9,

        /// <summary>
        /// An item that is a container.
        /// </summary>
        IsContainer = 1 << 10,

        /// <summary>
        /// An item that is cumulative.
        /// </summary>
        IsCumulative = 1 << 11,

        /// <summary>
        /// An item that disguises as another.
        /// </summary>
        IsDisguised = 1 << 12,

        /// <summary>
        /// An item that is dressable.
        /// </summary>
        IsDressable = 1 << 13,

        /// <summary>
        /// An item that is ground.
        /// </summary>
        IsGround = 1 << 14,

        /// <summary>
        /// An item that is a container for liquids.
        /// </summary>
        IsLiquidContainer = 1 << 15,

        /// <summary>
        /// An item that is a pool of liquid.
        /// </summary>
        IsLiquidPool = 1 << 16,

        /// <summary>
        /// An item that is a source of a certain liquid.
        /// </summary>
        IsLiquidSource = 1 << 17,

        /// <summary>
        /// An item that contains text and is readable.
        /// </summary>
        IsReadable = 1 << 18,

        /// <summary>
        /// An item that is a quest chest.
        /// </summary>
        IsQuestChest = 1 << 19,

        /// <summary>
        /// An item that cannot be moved.
        /// </summary>
        IsUnmoveable = 1 << 20,

        /// <summary>
        /// And item that is hung.
        /// </summary>
        IsHung = 1 << 21,

        /// <summary>
        /// An item that should be avoided by certain damage types, but does not actually <see cref="BlocksWalk"/>.
        /// </summary>
        ShouldBeAvoided = 1 << 22,

        /// <summary>
        /// An item that stays on bottom of the stack.
        /// </summary>
        StaysOnBottom = 1 << 23,

        /// <summary>
        /// An item that stays on top of the stack.
        /// </summary>
        StaysOnTop = 1 << 24,

        /// <summary>
        /// An item that triggers collision.
        /// </summary>
        TriggersCollision = 1 << 25,

        /// <summary>
        /// An item that triggers separation.
        /// </summary>
        TriggersSeparation = 1 << 26,
    }
}
