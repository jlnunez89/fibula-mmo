// -----------------------------------------------------------------
// <copyright file="IThingContainer.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all containers of <see cref="IThing"/>s.
    /// </summary>
    public interface IThingContainer : ILocatable
    {
        /// <summary>
        /// Attempts to find an <see cref="IThing"/> whitin this container.
        /// </summary>
        /// <param name="index">The index at which to look for the <see cref="IThing"/>.</param>
        /// <returns>The <see cref="IThing"/> found at the index, if any was found.</returns>
        IThing FindThingAtIndex(byte index);

        /// <summary>
        /// Attempts to add a <see cref="IThing"/> to this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index at which to add the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to add the <see cref="IThing"/> at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        (bool result, IThing remainder) AddContent(IThingFactory thingFactory, IThing thing, byte index = byte.MaxValue);

        /// <summary>
        /// Attempts to remove a thing from this container.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="thing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="index">Optional. The index from which to remove the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to remove the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        (bool result, IThing remainder) RemoveContent(IThingFactory thingFactory, ref IThing thing, byte index = byte.MaxValue, byte amount = 1);

        /// <summary>
        /// Attempts to replace a <see cref="IThing"/> from this container with another.
        /// </summary>
        /// <param name="thingFactory">A reference to the factory of things to use.</param>
        /// <param name="fromThing">The <see cref="IThing"/> to remove from the container.</param>
        /// <param name="toThing">The <see cref="IThing"/> to add to the container.</param>
        /// <param name="index">Optional. The index from which to replace the <see cref="IThing"/>. Defaults to byte.MaxValue, which instructs to replace the <see cref="IThing"/> if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="fromThing"/> to replace.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the thing may be returned.</returns>
        (bool result, IThing remainderToChange) ReplaceContent(IThingFactory thingFactory, IThing fromThing, IThing toThing, byte index = byte.MaxValue, byte amount = 1);
    }
}
