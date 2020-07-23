// -----------------------------------------------------------------
// <copyright file="CipCreatureFlag.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Enumerations
{
    /// <summary>
    /// Enumerates all known CIP creature flags.
    /// </summary>
    public enum CipCreatureFlag : uint
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// The creature can kick movable objects and destroy them.
        /// </summary>
        KickBoxes = 1 << 1,

        /// <summary>
        /// The creature can push or kill other cretuatures that don't have the <see cref="Unpushable"/> flag.
        /// </summary>
        KickCreatures = 1 << 2,

        /// <summary>
        /// The creature can see invisible players.
        /// </summary>
        SeeInvisible = 1 << 3,

        /// <summary>
        /// The creature cannot pushed or killed by other cretuatures with the <see cref="KickCreatures"/> flag.
        /// </summary>
        Unpushable = 1 << 4,

        /// <summary>
        /// The creature figths at a distance.
        /// </summary>
        DistanceFighting = 1 << 5,

        /// <summary>
        /// The creature cannot be summoned.
        /// </summary>
        NoSummon = 1 << 6,

        /// <summary>
        /// The illusion spell cannot be used to turn into this creature.
        /// </summary>
        NoIllusion = 1 << 7,

        /// <summary>
        /// The creature cannot be convinced.
        /// </summary>
        NoConvince = 1 << 8,

        /// <summary>
        /// The creature is immune to fire.
        /// </summary>
        NoBurning = 1 << 9,

        /// <summary>
        /// The creature is immune to poison.
        /// </summary>
        NoPoison = 1 << 10,

        /// <summary>
        /// The creature is immune to energy.
        /// </summary>
        NoEnergy = 1 << 11,

        /// <summary>
        /// The creature is immune to movement impairness spells.
        /// </summary>
        NoParalyze = 1 << 12,

        /// <summary>
        /// The creature cannot be targeted.
        /// </summary>
        NoHit = 1 << 13,

        /// <summary>
        /// The creature is immune to life draining.
        /// </summary>
        NoLifeDrain = 1 << 14,
    }
}
