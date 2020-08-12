// -----------------------------------------------------------------
// <copyright file="ClientInformation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
