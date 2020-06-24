// -----------------------------------------------------------------
// <copyright file="IInsertHouseInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    public interface IInsertHouseInfo
    {
        ushort Count { get; set; }

        ushort HouseId { get; set; }

        string Name { get; set; }

        uint Rent { get; set; }

        string Description { get; set; }

        ushort SquareMeters { get; set; }

        ushort EntranceX { get; set; }

        ushort EntranceY { get; set; }

        byte EntranceZ { get; set; }

        string Town { get; set; }

        byte IsGuildHouse { get; set; }
    }
}
