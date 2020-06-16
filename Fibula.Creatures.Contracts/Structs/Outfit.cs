// -----------------------------------------------------------------
// <copyright file="Outfit.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Structs
{
    /// <summary>
    /// Structure for creature outfits.
    /// </summary>
    public struct Outfit
    {
        /// <summary>
        /// Gets or sets the outfit's look alike id of an item.
        /// </summary>
        public ushort ItemIdLookAlike { get; set; }

        /// <summary>
        /// Gets or sets this outfit's id.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets this outfit's head value.
        /// </summary>
        public byte Head { get; set; }

        /// <summary>
        /// Gets or sets this outfit's body value.
        /// </summary>
        public byte Body { get; set; }

        /// <summary>
        /// Gets or sets this outfit's legs value.
        /// </summary>
        public byte Legs { get; set; }

        /// <summary>
        /// Gets or sets this outfit's feet value.
        /// </summary>
        public byte Feet { get; set; }
    }
}