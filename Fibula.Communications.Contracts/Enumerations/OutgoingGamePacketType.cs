// -----------------------------------------------------------------
// <copyright file="OutgoingGamePacketType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        /// Player login.
        /// </summary>
        PlayerLogin = 0x0A,

        /// <summary>
        /// Gamemaster flags.
        /// </summary>
        GamemasterFlags = 0x0B,

        /// <summary>
        /// Disconnection information.
        /// </summary>
        Disconnect = 0x14,

        /// <summary>
        /// Character placed on the waitlist for the world.
        /// </summary>
        WaitingList = 0x16,

        /// <summary>
        /// A heartbeat response.
        /// </summary>
        HeartbeatResponse = 0x1D,

        /// <summary>
        /// A heartbeat request.
        /// </summary>
        Heartbeat = 0x1E,

        /// <summary>
        /// Player death notification.
        /// </summary>
        Death = 0x28,

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
        /// Add a thing.
        /// </summary>
        AddThing = 0x6A,

        /// <summary>
        /// Update a thing.
        /// </summary>
        UpdateThing = 0x6B,

        /// <summary>
        /// Remove a thing.
        /// </summary>
        RemoveThing = 0x6C,

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
        /// Update creature health.
        /// </summary>
        CreatureHealth = 0x8C,

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
        /// Update creature skull.
        /// </summary>
        CreatureSkull = 0x90,

        /// <summary>
        /// Update creature shield.
        /// </summary>
        CreatureShield = 0x91,

        /// <summary>
        /// Sends a text window.
        /// </summary>
        TextWindow = 0x96,

        /// <summary>
        /// Sends a house window.
        /// </summary>
        HouseWindow = 0x97,

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
        CancelAttack = 0xA3,

        /// <summary>
        /// Updates a player's modes.
        /// </summary>
        PlayerModes = 0xA7,

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
        CancelWalk = 0xB5,

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

        /// <summary>
        /// The details of a Vip contact.
        /// </summary>
        VipDetails = 0xD2,

        /// <summary>
        /// Update when a Vip contact goes online.
        /// </summary>
        VipOnline = 0xD3,

        /// <summary>
        /// Update when a Vip contact goes offline.
        /// </summary>
        VipOffline = 0xD4,
    }
}
