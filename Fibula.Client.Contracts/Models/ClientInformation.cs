// -----------------------------------------------------------------
// <copyright file="ClientInformation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Client.Contracts.Models
{
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Class that represents inforamtion about the client.
    /// </summary>
    public class ClientInformation
    {
        /// <summary>
        /// Gets or sets the operating system.
        /// </summary>
        public AgentType Type { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }
    }
}
