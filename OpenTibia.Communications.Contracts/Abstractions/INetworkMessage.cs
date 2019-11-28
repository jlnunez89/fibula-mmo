// -----------------------------------------------------------------
// <copyright file="INetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using System;

    /// <summary>
    /// Interface that represents a network message.
    /// </summary>
    public interface INetworkMessage
    {
        /// <summary>
        /// Gets the byte buffer for this message.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Gets the message length.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the message position.
        /// </summary>
        int Cursor { get; }

        /// <summary>
        /// Adds a byte to the message.
        /// </summary>
        /// <param name="value">The byte value to add.</param>
        void AddByte(byte value);

        /// <summary>
        /// Adds a byte range to the message.
        /// </summary>
        /// <param name="value">The bytes to add.</param>
        void AddBytes(byte[] value);

        /// <summary>
        /// Adds a byte span to the message.
        /// </summary>
        /// <param name="value">The bytes span to add.</param>
        void AddBytes(ReadOnlySpan<byte> value);

        /// <summary>
        /// Add a string to the message.
        /// </summary>
        /// <param name="value">The string value to add.</param>
        void AddString(string value);

        /// <summary>
        /// Add an unsigned short integer to the message.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddUInt16(ushort value);

        /// <summary>
        /// Add an unsigned integer to the message.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddUInt32(uint value);

        /// <summary>
        /// Reads a byte value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        byte GetByte();

        /// <summary>
        /// Reads some bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        byte[] GetBytes(int count);

        /// <summary>
        /// Reads a string value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        string GetString();

        /// <summary>
        /// Reads an unsigned short value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        ushort GetUInt16();

        /// <summary>
        /// Reads an unsigned integer value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        uint GetUInt32();

        bool PrepareToSend(uint[] xteaKey);

        bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false);

        void Reset();

        //void Reset(int startingIndex);

        void Resize(int size);

        void RsaDecrypt(bool useCipKeys = true);

        /// <summary>
        /// Moves the read pointer in the message, essentially skipping a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        void SkipBytes(int count);

        bool XteaDecrypt(uint[] key);

        /// <summary>
        /// Creates a copy of this message.
        /// </summary>
        /// <returns>The copy of the message.</returns>
        INetworkMessage Copy();
    }
}