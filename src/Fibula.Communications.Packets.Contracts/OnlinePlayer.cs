// -----------------------------------------------------------------
// <copyright file="OnlinePlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    public class OnlinePlayer : IOnlinePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlinePlayer"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <param name="vocation"></param>
        public OnlinePlayer(string name, ushort level, string vocation)
        {
            this.Name = name;
            this.Level = level;
            this.Vocation = vocation;
        }

        public string Name { get; }

        public ushort Level { get; }

        public string Vocation { get; }
    }
}
