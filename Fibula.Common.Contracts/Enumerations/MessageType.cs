// -----------------------------------------------------------------
// <copyright file="MessageType.cs" company="2Dudes">
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
    /// Enumeration of the possible types of messages.
    /// </summary>
    public enum MessageType : byte
    {
        /// <summary>
        /// Yellow message in the console.
        /// </summary>
        ConsoleYellow = 0x01,

        /// <summary>
        /// Light blue message in the console.
        /// </summary>
        ConsoleLightBlue = 0x04,

        /// <summary>
        /// Orange message in the console
        /// </summary>
        ConsoleOrange = 0x11,

        /// <summary>
        /// Red message in game window and in the console.
        /// </summary>
        Warning = 0x12,

        /// <summary>
        /// White message in game window and in the console.
        /// </summary>
        EventAdvance = 0x13,

        /// <summary>
        /// White message at the bottom of the game window and in the console.
        /// </summary>
        EventDefault = 0x14,

        /// <summary>
        /// White message at the bottom of the game window and in the console.
        /// </summary>
        StatusDefault = 0x15,

        /// <summary>
        /// Green message in game window and in the console.
        /// </summary>
        DescriptionGreen = 0x16,

        /// <summary>
        /// White message at the bottom of the game window.
        /// </summary>
        StatusSmall = 0x17,

        /// <summary>
        /// Blue message in the console.
        /// </summary>
        ConsoleBlue = 0x18,

        /// <summary>
        /// Red message in the console.
        /// </summary>
        ConsoleRed = 0x19,
    }
}
