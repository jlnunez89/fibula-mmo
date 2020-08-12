// -----------------------------------------------------------------
// <copyright file="INetworkMessage.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    using System;
    using System.Buffers;
    using System.Net.Sockets;

    /// <summary>
    /// Interface that represents a network message.
    /// </summary>
    public interface INetworkMessage
    {
        /// <summary>
        /// Gets the byte buffer for this message.
        /// </summary>
        Span<byte> Buffer { get; }

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
        void AddBytes(ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a byte sequence to the message.
        /// </summary>
        /// <param name="value">The bytes span to add.</param>
        void AddBytes(ReadOnlySequence<byte> value);

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
        /// Reads a byte value from the message without advancing the read cursor.
        /// </summary>
        /// <returns>The value read.</returns>
        byte PeekByte();

        /// <summary>
        /// Reads a byte value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        byte GetByte();

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
        /// Reads some bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        ReadOnlySpan<byte> GetBytes(int count);

        /// <summary>
        /// Resets the message.
        /// </summary>
        void Reset();

        /// <summary>
        /// Changes the <see cref="Length"/> of this message and resets its <see cref="Cursor"/>.
        /// </summary>
        /// <param name="size">The new size of the message.</param>
        /// <param name="resetCursor">Optional. A value indicating whether to reset the cursor in the message to zero.</param>
        void Resize(int size, bool resetCursor = true);

        /// <summary>
        /// Reads information sent in the message as bytes.
        /// </summary>
        /// <returns>The information read.</returns>
        IBytesInfo ReadAsBytesInfo();

        /// <summary>
        /// Moves the read pointer in the message, essentially skipping a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        void SkipBytes(int count);

        /// <summary>
        /// Creates a copy of this message.
        /// </summary>
        /// <param name="overrideCursor">Optional. A value to override the cursor with.</param>
        /// <returns>The copy of the message.</returns>
        INetworkMessage Copy(int? overrideCursor = null);

        /// <summary>
        /// Reads <paramref name="numberOfBytesToRead"/> bytes from the supplied network stream into the message buffer.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="numberOfBytesToRead">The number of bytes to read.</param>
        void ReadBytesFromStream(NetworkStream stream, ushort numberOfBytesToRead);
    }
}
