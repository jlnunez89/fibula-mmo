// -----------------------------------------------------------------
// <copyright file="AgentType.cs" company="2Dudes">
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
    /// Enumeration of the possible type of client agents.
    /// </summary>
    public enum AgentType : ushort
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The Linux client.
        /// </summary>
        Linux = 1,

        /// <summary>
        /// The Windows client.
        /// </summary>
        Windows = 2,

        /// <summary>
        /// The flash client.
        /// </summary>
        Flash = 3,
    }
}
