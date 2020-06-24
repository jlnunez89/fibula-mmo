// -----------------------------------------------------------------
// <copyright file="ByteExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities
{
    using System;

    public static class ByteExtensions
    {
        public static byte[] ToByteArray(this uint[] unsignedIntegers)
        {
            var temp = new byte[unsignedIntegers.Length * sizeof(uint)];

            for (var i = 0; i < unsignedIntegers.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(unsignedIntegers[i]), 0, temp, i * 4, 4);
            }

            return temp;
        }

        public static uint[] ToUInt32Array(this byte[] bytes)
        {
            if (bytes.Length % 4 > 0)
            {
                throw new Exception();
            }

            var temp = new uint[bytes.Length / 4];

            for (var i = 0; i < temp.Length; i++)
            {
                temp[i] = BitConverter.ToUInt32(bytes, i * 4);
            }

            return temp;
        }
    }
}
