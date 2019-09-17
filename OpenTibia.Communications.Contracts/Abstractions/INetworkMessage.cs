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
        int Position { get; }

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
        /// Adds a number of bytes with value Zero to the message.
        /// </summary>
        /// <param name="count">The number of zero-bytes to add.</param>
        void AddPaddingBytes(int count);

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

        byte[] GetPacket();

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

        /// <summary>
        /// Peeks the next byte from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        byte PeekByte();

        /// <summary>
        /// Peeks the next bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to peek.</param>
        /// <returns>The bytes peeked.</returns>
        byte[] PeekBytes(int count);

        /// <summary>
        /// Peeks a string from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        string PeekString();

        /// <summary>
        /// Peeks an unsigned short value from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        ushort PeekUInt16();

        /// <summary>
        /// Peeks an unsigned integer value from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        uint PeekUInt32();

        bool PrepareToRead(uint[] xteaKey);

        bool PrepareToSend(uint[] xteaKey);

        bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false);

        /// <summary>
        /// Replaces a range of bytes in the message.
        /// </summary>
        /// <param name="index">The index at which to begin replacing the byte range.</param>
        /// <param name="value">The byte range to replace in the message.</param>
        void ReplaceBytes(int index, byte[] value);

        void Reset();

        void Reset(int startingIndex);

        void Resize(int size);

        void RsaDecrypt(bool useCipKeys = true);

        /// <summary>
        /// Moves the read pointer in the message, essentially skipping a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        void SkipBytes(int count);

        bool XteaDecrypt(uint[] key);

        bool XteaEncrypt(uint[] key);

        /// <summary>
        /// Creates a copy of this message.
        /// </summary>
        /// <returns>The copy of the message.</returns>
        INetworkMessage Copy();
    }
}