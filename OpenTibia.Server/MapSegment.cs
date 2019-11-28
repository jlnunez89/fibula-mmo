// -----------------------------------------------------------------
// <copyright file="MapSegment.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Buffers;

    internal class MapSegment<T> : ReadOnlySequenceSegment<T>
    {
        public MapSegment(ReadOnlyMemory<T> memory) => this.Memory = memory;

        public MapSegment<T> Add(ReadOnlyMemory<T> mem)
        {
            var segment = new MapSegment<T>(mem)
            {
                RunningIndex = this.RunningIndex + this.Memory.Length,
            };

            this.Next = segment;

            return segment;
        }
    }
}
