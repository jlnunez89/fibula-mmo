// -----------------------------------------------------------------
// <copyright file="OutgoingPacketType.cs" company="2Dudes">
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
    public enum OutgoingPacketType : byte
    {
        /// <summary>
        /// The message of the day.
        /// </summary>
        MessageOfTheDay,

        /// <summary>
        /// A character list.
        /// </summary>
        CharacterList,

        /// <summary>
        /// Player login.
        /// </summary>
        PlayerLogin,

        /// <summary>
        /// Gamemaster flags.
        /// </summary>
        GamemasterFlags,

        /// <summary>
        /// Disconnection from game world.
        /// </summary>
        GameDisconnect,

        /// <summary>
        /// Disconnection from gateway server.
        /// </summary>
        GatewayDisconnect,

        /// <summary>
        /// Character placed on the waitlist for the world.
        /// </summary>
        WaitingList,

        /// <summary>
        /// A heartbeat response.
        /// </summary>
        HeartbeatResponse,

        /// <summary>
        /// A heartbeat request.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Player death notification.
        /// </summary>
        Death,

        /// <summary>
        /// Adding an unknown creature description.
        /// </summary>
        AddUnknownCreature,

        /// <summary>
        /// Adding a known creature description.
        /// </summary>
        AddKnownCreature,

        /// <summary>
        /// Full map description.
        /// </summary>
        MapDescription,

        /// <summary>
        /// Partial north map description.
        /// </summary>
        MapSliceNorth,

        /// <summary>
        /// Partial east map description.
        /// </summary>
        MapSliceEast,

        /// <summary>
        /// Partial south map description.
        /// </summary>
        MapSliceSouth,

        /// <summary>
        /// Partial west map description.
        /// </summary>
        MapSliceWest,

        /// <summary>
        /// Tile description.
        /// </summary>
        TileUpdate,

        /// <summary>
        /// Add a thing.
        /// </summary>
        AddThing,

        /// <summary>
        /// Update a thing.
        /// </summary>
        UpdateThing,

        /// <summary>
        /// Remove a thing.
        /// </summary>
        RemoveThing,

        /// <summary>
        /// A creature moving in the map.
        /// </summary>
        CreatureMoved,

        /// <summary>
        /// A container being open.
        /// </summary>
        ContainerOpen,

        /// <summary>
        /// A container being closed.
        /// </summary>
        ContainerClose,

        /// <summary>
        /// An item being added to an open container.
        /// </summary>
        ContainerAddItem,

        /// <summary>
        /// An item being updated to an open container.
        /// </summary>
        ContainerUpdateItem,

        /// <summary>
        /// An item being removed to an open container.
        /// </summary>
        ContainerRemoveItem,

        /// <summary>
        /// An item description in the inventory.
        /// </summary>
        InventoryItem,

        /// <summary>
        /// Clear an item description in the inventory.
        /// </summary>
        InventoryEmpty,

        /// <summary>
        /// World light description.
        /// </summary>
        WorldLight,

        /// <summary>
        /// A magic effect on the map.
        /// </summary>
        MagicEffect,

        /// <summary>
        /// An animated text on the map.
        /// </summary>
        AnimatedText,

        /// <summary>
        /// An animated projectile.
        /// </summary>
        ProjectileEffect,

        /// <summary>
        /// A square around a creature.
        /// </summary>
        Square,

        /// <summary>
        /// Update creature health.
        /// </summary>
        CreatureHealth,

        /// <summary>
        /// Update creature light.
        /// </summary>
        CreatureLight,

        /// <summary>
        /// Update creature outfit.
        /// </summary>
        CreatureOutfit,

        /// <summary>
        /// Update creature speed.
        /// </summary>
        CreatureSpeedChange,

        /// <summary>
        /// Update creature skull.
        /// </summary>
        CreatureSkull,

        /// <summary>
        /// Update creature shield.
        /// </summary>
        CreatureShield,

        /// <summary>
        /// Sends a text window.
        /// </summary>
        TextWindow,

        /// <summary>
        /// Sends a house window.
        /// </summary>
        HouseWindow,

        /// <summary>
        /// Send a player stats.
        /// </summary>
        PlayerStats,

        /// <summary>
        /// Update a player skill.
        /// </summary>
        PlayerSkills,

        /// <summary>
        /// Update a player's conditions.
        /// </summary>
        PlayerConditions,

        /// <summary>
        /// Cancel an attack target.
        /// </summary>
        CancelAttack,

        /// <summary>
        /// Updates a player's modes.
        /// </summary>
        PlayerModes,

        /// <summary>
        /// A creature speech shown on the map.
        /// </summary>
        CreatureSpeech,

        /// <summary>
        /// A direct text message.
        /// </summary>
        TextMessage,

        /// <summary>
        /// Cancel walking.
        /// </summary>
        CancelWalk,

        /// <summary>
        /// Going up a floor.
        /// </summary>
        FloorChangeUp,

        /// <summary>
        /// Going down a floor.
        /// </summary>
        FloorChangeDown,

        /// <summary>
        /// Show the outfit window.
        /// </summary>
        OutfitWindow,

        /// <summary>
        /// The details of a Vip contact.
        /// </summary>
        VipDetails,

        /// <summary>
        /// Update when a Vip contact goes online.
        /// </summary>
        VipOnline,

        /// <summary>
        /// Update when a Vip contact goes offline.
        /// </summary>
        VipOffline,
    }
}
