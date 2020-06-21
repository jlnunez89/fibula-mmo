// -----------------------------------------------------------------
// <copyright file="SkillType.cs" company="2Dudes">
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
    /// Enumeration of the possible skills.
    /// </summary>
    public enum SkillType : byte
    {
        /// <summary>
        /// The experience level.
        /// </summary>
        Experience,

        /// <summary>
        /// The magic level.
        /// </summary>
        Magic,

        /// <summary>
        /// The bare-hands or non-weapon fighting level.
        /// </summary>
        NoWeapon,

        /// <summary>
        /// The axe fighting level.
        /// </summary>
        Axe,

        /// <summary>
        /// The club fighting level.
        /// </summary>
        Club,

        /// <summary>
        /// The sword fighting level.
        /// </summary>
        Sword,

        /// <summary>
        /// The shielding level.
        /// </summary>
        Shield,

        /// <summary>
        /// The ranged fighting level.
        /// </summary>
        Ranged,

        /// <summary>
        /// The fishing level.
        /// </summary>
        Fishing,
    }
}
