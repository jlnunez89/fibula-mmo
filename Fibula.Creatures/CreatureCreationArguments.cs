// -----------------------------------------------------------------
// <copyright file="CreatureCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for creatures.
    /// </summary>
    public class CreatureCreationArguments : IThingCreationArguments
    {
        /// <summary>
        /// Gets or sets the type of creature being created.
        /// </summary>
        public CreatureType Type { get; set; }

        /// <summary>
        /// Gets or sets the metadata for the creature being created.
        /// </summary>
        public ICreatureCreationMetadata Metadata { get; set; }
    }
}
