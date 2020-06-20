// -----------------------------------------------------------------
// <copyright file="AnimatedEffect.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible animated effects in the game.
    /// </summary>
    public enum AnimatedEffect : byte
    {
        /// <summary>
        /// A pointer to the first valid value in this enumeration.
        /// </summary>
        First = XBlood,

        /// <summary>
        /// The bleeding animation.
        /// </summary>
        XBlood = 0x01,

        /// <summary>
        /// Three blue rings.
        /// </summary>
        RingsBlue = 0x02,

        /// <summary>
        /// A puff of smoke.
        /// </summary>
        Puff = 0x03,

        /// <summary>
        /// Yellow sparks.
        /// </summary>
        SparkYellow = 0x04,

        /// <summary>
        /// Explosion animation.
        /// </summary>
        DamageExplosion = 0x05,

        /// <summary>
        /// Magic missile impact animation.
        /// </summary>
        DamageMagicMissile = 0x06,

        /// <summary>
        /// Flame animation.
        /// </summary>
        AreaFlame = 0x07,

        /// <summary>
        /// Three yellow rings.
        /// </summary>
        RingsYellow = 0x08,

        /// <summary>
        /// Three green rings.
        /// </summary>
        RingsGreen = 0x09,

        /// <summary>
        /// Bones being cut.
        /// </summary>
        XGray = 0x0A,

        /// <summary>
        /// Blue bubble animation.
        /// </summary>
        BubbleBlue = 0x0B,

        /// <summary>
        /// Electricity damage animation.
        /// </summary>
        DamageEnergy = 0x0C,

        /// <summary>
        /// Blue glitter animation.
        /// </summary>
        GlitterBlue = 0x0D,

        /// <summary>
        /// Red glitter animation.
        /// </summary>
        GlitterRed = 0x0E,

        /// <summary>
        /// Green glitter animation.
        /// </summary>
        GlitterGreen = 0x0F,

        /// <summary>
        /// Flame animation.
        /// </summary>
        Flame = 0x10,

        /// <summary>
        /// Poison animation.
        /// </summary>
        Poison = 0x11,

        /// <summary>
        /// Black bubble animation.
        /// </summary>
        BubbleBlack = 0x12,

        /// <summary>
        /// Green notes of sound animation.
        /// </summary>
        SoundGreen = 0x13,

        /// <summary>
        /// Red notes of sound animation.
        /// </summary>
        SoundRed = 0x14,

        /// <summary>
        /// Venom missile impact animation.
        /// </summary>
        DamageVenomMissile = 0x15,

        /// <summary>
        /// Yellow notes of sound animation.
        /// </summary>
        SoundYellow = 0x16,

        /// <summary>
        /// Purple notes of sound animation.
        /// </summary>
        SoundPurple = 0x17,

        /// <summary>
        /// Blue notes of sound animation.
        /// </summary>
        SoundBlue = 0x18,

        /// <summary>
        /// White notes of sound animation.
        /// </summary>
        SoundWhite = 0x19,

        /// <summary>
        /// A pointer to the last valid value in this enumeration.
        /// </summary>
        Last = SoundWhite,

        /// <summary>
        /// No animation.
        /// Don't send to client.
        /// </summary>
        None = 0xFF,
    }
}
