// -----------------------------------------------------------------
// <copyright file="OutgoingManagementPacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Enumerations
{
    public enum OutgoingManagementPacketType : byte
    {
        NoError = 0x00,
        Error = 0x01,
        Disconnect = 0x0A,
        MessageOfTheDay = 0x14,
        CharacterList = 0x64,
    }
}
