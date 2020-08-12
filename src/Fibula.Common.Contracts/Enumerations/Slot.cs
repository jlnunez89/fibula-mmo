// -----------------------------------------------------------------
// <copyright file="Slot.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible inventory slots.
    /// </summary>
    public enum Slot : byte
    {
        /// <summary>
        /// Any of the hand slots.
        /// Uses both hands.
        /// </summary>
        TwoHanded = 0x00,

        /// <summary>
        /// The head slot.
        /// </summary>
        Head = 0x01,

        /// <summary>
        /// The neck slot.
        /// </summary>
        Neck = 0x02,

        /// <summary>
        /// The back slot.
        /// </summary>
        Back = 0x03,

        /// <summary>
        /// The chest slot.
        /// </summary>
        Body = 0x04,

        /// <summary>
        /// The right hand slot.
        /// </summary>
        RightHand = 0x05,

        /// <summary>
        /// The left hand slot.
        /// </summary>
        LeftHand = 0x06,

        /// <summary>
        /// The pants slot.
        /// </summary>
        Legs = 0x07,

        /// <summary>
        /// The feet slot.
        /// </summary>
        Feet = 0x08,

        /// <summary>
        /// The ring slot.
        /// </summary>
        Ring = 0x09,

        /// <summary>
        /// The ammunition slot.
        /// </summary>
        Ammo = 0x0A,

        /// <summary>
        /// Special slot that is a wildcard.
        /// </summary>
        Anywhere = 0x0B,

        /// <summary>
        /// Special slot that is an unset or invalid value.
        /// </summary>
        UnsetInvalid = byte.MaxValue,
    }
}
