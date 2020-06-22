// -----------------------------------------------------------------
// <copyright file="LiquidType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the different types of liquid in the game.
    /// </summary>
    public enum LiquidType
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None,

        /// <summary>
        /// H2O.
        /// </summary>
        Water,

        /// <summary>
        /// Wine
        /// </summary>
        Wine,

        /// <summary>
        /// Beer, of course.
        /// </summary>
        Beer,

        /// <summary>
        /// Mud.
        /// </summary>
        Mud,

        /// <summary>
        /// Vampires love this.
        /// </summary>
        Blood,

        /// <summary>
        /// Seems poison-y.
        /// </summary>
        Slime,

        /// <summary>
        /// Oil.
        /// </summary>
        Oil,

        /// <summary>
        /// You probably shouldn't drink it.
        /// </summary>
        Urine,

        /// <summary>
        /// Yum, milk!
        /// </summary>
        Milk,

        /// <summary>
        /// Mana replentishing fluid.
        /// </summary>
        ManaFluid,

        /// <summary>
        /// Life replentishing fluid.
        /// </summary>
        LifeFluid,

        /// <summary>
        /// Tasty lemonade.
        /// </summary>
        Lemonade,
    }
}
