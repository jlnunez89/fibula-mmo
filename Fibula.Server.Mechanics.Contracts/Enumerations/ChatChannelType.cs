// -----------------------------------------------------------------
// <copyright file="ChatChannelType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Contracts.Enumerations
{
    public enum ChatChannelType : ushort
    {
        RuleViolations = 0x03,
        Game = 0x04,
        Trade = 0x05,
        RealLife = 0x06,
        Help = 0x08,
        Private = 0xFFFF,
        None = 0xAAAA
    }
}
