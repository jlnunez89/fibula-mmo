// -----------------------------------------------------------------
// <copyright file="IOnlineCharacterEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities.Contracts.Abstractions
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
