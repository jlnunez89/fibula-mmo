﻿// -----------------------------------------------------------------
// <copyright file="MapDescriptionSegment.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts
{
    using System;
    using System.Buffers;

    /// <summary>
    /// Class that represents a segment of a map description.
    /// </summary>
    public class MapDescriptionSegment : ReadOnlySequenceSegment<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapDescriptionSegment"/> class.
        /// </summary>
        /// <param name="memory">The memory to define for this segment.</param>
        public MapDescriptionSegment(ReadOnlyMemory<byte> memory)
        {
            this.Memory = memory;
        }

        /// <summary>
        /// Appends the given <see cref="MapDescriptionSegment"/> to another.
        /// </summary>
        /// <param name="nextSegment">A reference to the next segment.</param>
        public void Append(MapDescriptionSegment nextSegment)
        {
            nextSegment.RunningIndex = this.RunningIndex + this.Memory.Length;

            this.Next = nextSegment;
        }

        /// <summary>
        /// Appends the given memory bytes by pointing <see cref="ReadOnlySequenceSegment{T}.Next"/> to a new <see cref="MapDescriptionSegment"/>.
        /// </summary>
        /// <param name="mem">The memory bytes to append.</param>
        /// <returns>The new instance of <see cref="MapDescriptionSegment"/>, pointing to the segment after this one.</returns>
        public MapDescriptionSegment Append(ReadOnlyMemory<byte> mem)
        {
            var segment = new MapDescriptionSegment(mem)
            {
                RunningIndex = this.RunningIndex + this.Memory.Length,
            };

            this.Next = segment;

            return segment;
        }
    }
}
