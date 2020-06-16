// -----------------------------------------------------------------
// <copyright file="OutgoingGamePacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different outgoing game server packet types.
    /// </summary>
    public enum OutgoingGamePacketType : byte
    {
        /// <summary>
        /// Self description about the player.
        /// </summary>
        SelfAppear = 0x0A,

        /// <summary>
        /// Disconnection information.
        /// </summary>
        Disconnect = 0x14,

        /// <summary>
        /// A server initiated ping.
        /// </summary>
        Ping = 0x1D,

        /// <summary>
        /// Response for a ping.
        /// </summary>
        Pong = 0x1E,

        /// <summary>
        /// Adding an unknown creature description.
        /// </summary>
        AddUnknownCreature = 0x61,

        /// <summary>
        /// Adding a known creature description.
        /// </summary>
        AddKnownCreature = 0x62,

        /// <summary>
        /// Full map description.
        /// </summary>
        MapDescription = 0x64,

        /// <summary>
        /// Partial north map description.
        /// </summary>
        MapSliceNorth = 0x65,

        /// <summary>
        /// Partial east map description.
        /// </summary>
        MapSliceEast = 0x66,

        /// <summary>
        /// Partial south map description.
        /// </summary>
        MapSliceSouth = 0x67,

        /// <summary>
        /// Partial west map description.
        /// </summary>
        MapSliceWest = 0x68,

        /// <summary>
        /// Tile description.
        /// </summary>
        TileUpdate = 0x69,

        /// <summary>
        /// Add item description at stack position.
        /// </summary>
        AddAtStackpos = 0x6A,

        /// <summary>
        /// Thing transformation.
        /// </summary>
        TransformThing = 0x6B,

        /// <summary>
        /// Remove item at stack position.
        /// </summary>
        RemoveAtStackpos = 0x6C,

        /// <summary>
        /// A creature moving in the map.
        /// </summary>
        CreatureMoved = 0x6D,

        /// <summary>
        /// A container being open.
        /// </summary>
        ContainerOpen = 0x6E,

        /// <summary>
        /// A container being closed.
        /// </summary>
        ContainerClose = 0x6F,

        /// <summary>
        /// An item being added to an open container.
        /// </summary>
        ContainerAddItem = 0x70,

        /// <summary>
        /// An item being updated to an open container.
        /// </summary>
        ContainerUpdateItem = 0x71,

        /// <summary>
        /// An item being removed to an open container.
        /// </summary>
        ContainerRemoveItem = 0x72,

        /// <summary>
        /// An item description in the inventory.
        /// </summary>
        InventoryItem = 0x78,

        /// <summary>
        /// Clear an item description in the inventory.
        /// </summary>
        InventoryEmpty = 0x79,

        /// <summary>
        /// World light description.
        /// </summary>
        WorldLight = 0x82,

        /// <summary>
        /// A magic effect on the map.
        /// </summary>
        MagicEffect = 0x83,

        /// <summary>
        /// An animated text on the map.
        /// </summary>
        AnimatedText = 0x84,

        /// <summary>
        /// An animated projectile.
        /// </summary>
        ProjectileEffect = 0x85,

        /// <summary>
        /// A square around a creature.
        /// </summary>
        Square = 0x86,

        /// <summary>
        /// Update creature light.
        /// </summary>
        CreatureLight = 0x8D,

        /// <summary>
        /// Update creature outfit.
        /// </summary>
        CreatureOutfit = 0x8E,

        /// <summary>
        /// Update creature speed.
        /// </summary>
        CreatureSpeedChange = 0x8F,

        /// <summary>
        /// Send a player stats.
        /// </summary>
        PlayerStats = 0xA0,

        /// <summary>
        /// Update a player skill.
        /// </summary>
        PlayerSkills = 0xA1,

        /// <summary>
        /// Update a player's conditions.
        /// </summary>
        PlayerConditions = 0xA2,

        /// <summary>
        /// Cancel an attack target.
        /// </summary>
        CancelTarget = 0xA3,

        /// <summary>
        /// A creature speech shown on the map.
        /// </summary>
        CreatureSpeech = 0xAA,

        /// <summary>
        /// A direct text message.
        /// </summary>
        TextMessage = 0xB4,

        /// <summary>
        /// Cancel walking.
        /// </summary>
        WalkCancel = 0xB5,

        /// <summary>
        /// Going up a floor.
        /// </summary>
        FloorChangeUp = 0xBE,

        /// <summary>
        /// Going down a floor.
        /// </summary>
        FloorChangeDown = 0xBF,

        /// <summary>
        /// Show the outfit window.
        /// </summary>
        OutfitWindow = 0xC8,
    }
}
