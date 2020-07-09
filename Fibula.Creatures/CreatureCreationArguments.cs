// -----------------------------------------------------------------
// <copyright file="CreatureCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;

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
