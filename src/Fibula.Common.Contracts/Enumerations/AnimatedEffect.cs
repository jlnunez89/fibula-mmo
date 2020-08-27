// -----------------------------------------------------------------
// <copyright file="AnimatedEffect.cs" company="2Dudes">
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
    /// Enumerates the possible animated effects in the game.
    /// </summary>
    public enum AnimatedEffect : byte
    {
        /// <summary>
        /// The bleeding animation.
        /// </summary>
        XBlood,

        /// <summary>
        /// Three blue rings.
        /// </summary>
        RingsBlue,

        /// <summary>
        /// A puff of smoke.
        /// </summary>
        Puff,

        /// <summary>
        /// Yellow sparks.
        /// </summary>
        SparkYellow,

        /// <summary>
        /// Explosion animation.
        /// </summary>
        DamageExplosion,

        /// <summary>
        /// Magic missile impact animation.
        /// </summary>
        DamageMagicMissile,

        /// <summary>
        /// Flame animation.
        /// </summary>
        AreaFlame,

        /// <summary>
        /// Three yellow rings.
        /// </summary>
        RingsYellow,

        /// <summary>
        /// Three green rings.
        /// </summary>
        RingsGreen,

        /// <summary>
        /// Bones being cut.
        /// </summary>
        XGray,

        /// <summary>
        /// Blue bubble animation.
        /// </summary>
        BubbleBlue,

        /// <summary>
        /// Electricity damage animation.
        /// </summary>
        DamageEnergy,

        /// <summary>
        /// Blue glitter animation.
        /// </summary>
        GlitterBlue,

        /// <summary>
        /// Red glitter animation.
        /// </summary>
        GlitterRed,

        /// <summary>
        /// Green glitter animation.
        /// </summary>
        GlitterGreen,

        /// <summary>
        /// Flame animation.
        /// </summary>
        Flame,

        /// <summary>
        /// Poison animation.
        /// </summary>
        Poison,

        /// <summary>
        /// Black bubble animation.
        /// </summary>
        BubbleBlack,

        /// <summary>
        /// Green notes of sound animation.
        /// </summary>
        SoundGreen,

        /// <summary>
        /// Red notes of sound animation.
        /// </summary>
        SoundRed,

        /// <summary>
        /// Venom missile impact animation.
        /// </summary>
        DamageVenomMissile,

        /// <summary>
        /// Yellow notes of sound animation.
        /// </summary>
        SoundYellow,

        /// <summary>
        /// Purple notes of sound animation.
        /// </summary>
        SoundPurple,

        /// <summary>
        /// Blue notes of sound animation.
        /// </summary>
        SoundBlue,

        /// <summary>
        /// White notes of sound animation.
        /// </summary>
        SoundWhite,

        /// <summary>
        /// No animation.
        /// Don't send to client.
        /// </summary>
        None = byte.MaxValue,
    }
}
