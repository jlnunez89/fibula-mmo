// -----------------------------------------------------------------
// <copyright file="IOnlineCharacterEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities.Contracts.Abstractions
{
    /// <summary>
    /// Interface for an online character list entry entity.
    /// </summary>
    public interface IOnlineCharacterEntity : IEntity
    {
        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// Gets the character level.
        /// </summary>
        ushort Level { get; }

        /// <summary>
        /// Gets the vocation of the character.
        /// </summary>
        string Vocation { get; }

        /// <summary>
        /// Gets the world where the character is currently online in.
        /// </summary>
        string World { get; }
    }
}
