// -----------------------------------------------------------------
// <copyright file="NetworkMessageExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Static class that contains extension methods for an <see cref="INetworkMessage"/> to include
    /// descriptions of common server entities.
    /// </summary>
    public static class NetworkMessageExtensions
    {
        /// <summary>
        /// Adds a <see cref="Location"/>'s description to the message.
        /// </summary>
        /// <param name="message">The message to add the location to.</param>
        /// <param name="location">The location to add.</param>
        public static void AddLocation(this INetworkMessage message, Location location)
        {
            message.AddUInt16((ushort)location.X);
            message.AddUInt16((ushort)location.Y);
            message.AddByte((byte)location.Z);
        }

        /// <summary>
        /// Add a <see cref="ICreature"/>'s description to the message.
        /// </summary>
        /// <param name="message">The message to add the creature description to.</param>
        /// <param name="creature">The creature to describe and add.</param>
        /// <param name="asKnown">A value indicating whether this creature is known.</param>
        /// <param name="creatureToRemoveId">The id of another creature to replace if the client buffer is known to be full.</param>
        public static void AddCreature(this INetworkMessage message, ICreature creature, bool asKnown, uint creatureToRemoveId)
        {
            if (asKnown)
            {
                message.AddByte((byte)OutgoingGamePacketType.AddKnownCreature); // known
                message.AddByte(0x00);
                message.AddUInt32(creature.Id);
            }
            else
            {
                message.AddByte((byte)OutgoingGamePacketType.AddUnknownCreature); // unknown
                message.AddByte(0x00);
                message.AddUInt32(creatureToRemoveId);
                message.AddUInt32(creature.Id);
                message.AddString(creature.Name);
            }

            message.AddByte(Convert.ToByte(Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints))); // health bar, needs a percentage.
            message.AddByte(Convert.ToByte(creature.Direction.GetClientSafeDirection()));

            if (creature.IsInvisible)
            {
                message.AddUInt16(0x00);
                message.AddUInt16(0x00);
            }
            else
            {
                message.AddOutfit(creature.Outfit);
            }

            message.AddByte(creature.EmittedLightLevel);
            message.AddByte(creature.EmittedLightColor);
            message.AddUInt16(creature.Speed);

            message.AddByte(creature.Skull);
            message.AddByte(creature.Shield);
        }

        /// <summary>
        /// Adds an <see cref="Outfit"/>'s description to the message.
        /// </summary>
        /// <param name="message">The message to add the outfit to.</param>
        /// <param name="outfit">The outfit to add.</param>
        public static void AddOutfit(this INetworkMessage message, Outfit outfit)
        {
            // Add creature outfit
            message.AddUInt16(outfit.Id);

            if (outfit.Id > 0)
            {
                message.AddByte(outfit.Head);
                message.AddByte(outfit.Body);
                message.AddByte(outfit.Legs);
                message.AddByte(outfit.Feet);
            }
            else
            {
                message.AddUInt16(outfit.ItemIdLookAlike);
            }
        }

        /// <summary>
        /// Adds an <see cref="IItem"/>'s description to a message.
        /// </summary>
        /// <param name="message">The message to add the item description to.</param>
        /// <param name="item">The item to describe and add.</param>
        public static void AddItem(this INetworkMessage message, IItem item)
        {
            if (item == null)
            {
                // TODO: log this.
                Console.WriteLine("Add: Null item passed.");
                return;
            }

            message.AddUInt16(item.Type.ClientId);

            if (item.IsCumulative)
            {
                message.AddByte(item.Amount);
            }
            else if (item.IsLiquidPool || item.IsLiquidSource || item.IsLiquidContainer)
            {
                message.AddByte((byte)item.LiquidType.ToLiquidColor());
            }
        }

        /// <summary>
        /// Converts a <see cref="LiquidType"/> to the client supported <see cref="LiquidColor"/>.
        /// </summary>
        /// <param name="liquidType">The type of liquid.</param>
        /// <returns>The color supported by the client.</returns>
        public static LiquidColor ToLiquidColor(this LiquidType liquidType)
        {
            switch (liquidType)
            {
                default:
                case LiquidType.None:
                    return LiquidColor.None;
                case LiquidType.Water:
                    return LiquidColor.Blue;
                case LiquidType.Wine:
                case LiquidType.ManaFluid:
                    return LiquidColor.Purple;
                case LiquidType.Beer:
                case LiquidType.Mud:
                case LiquidType.Oil:
                    return LiquidColor.Brown;
                case LiquidType.Blood:
                case LiquidType.LifeFluid:
                    return LiquidColor.Red;
                case LiquidType.Slime:
                case LiquidType.Lemonade:
                    return LiquidColor.Green;
                case LiquidType.Urine:
                    return LiquidColor.Yellow;
                case LiquidType.Milk:
                    return LiquidColor.White;
            }
        }
    }
}
