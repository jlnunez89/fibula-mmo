// -----------------------------------------------------------------
// <copyright file="NetworkMessage.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications
{
    using System;
    using System.Buffers;
    using System.Text;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a network message.
    /// </summary>
    public class NetworkMessage : INetworkMessage
    {
        /// <summary>
        /// The number of reserved bytes in the network message for outbound messages.
        /// </summary>
        public const int OutboundMessageStartingIndex = 4;

        /// <summary>
        /// The size of the message's buffer.
        /// </summary>
        public const int BufferSize = 16394;

        /// <summary>
        /// The default index at which the message's content starts.
        /// </summary>
        public const int DefaultStartingIndex = 2;

        /// <summary>
        /// The buffer of the message.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        /// <param name="isOutbound">A value indicating whether this message is an outboud message.</param>
        public NetworkMessage(bool isOutbound = true)
            : this(isOutbound ? OutboundMessageStartingIndex : 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        private NetworkMessage()
        {
            this.Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        /// <param name="startingIndex">The index at which to set the <see cref="Cursor"/> of this message.</param>
        private NetworkMessage(int startingIndex)
        {
            this.Reset(startingIndex);
        }

        /// <summary>
        /// Gets the length of the message.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the position that the message will read from next.
        /// </summary>
        public int Cursor { get; private set; }

        /// <summary>
        /// Gets the buffer of this message.
        /// </summary>
        public Span<byte> Buffer => this.buffer;

        /// <summary>
        /// Resets the message.
        /// </summary>
        public void Reset()
        {
            this.Reset(DefaultStartingIndex);
        }

        /// <summary>
        /// Reads a byte value from the message without advancing the read cursor.
        /// </summary>
        /// <returns>The value read.</returns>
        public byte PeekByte()
        {
            if (this.Cursor + 1 > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.GetByte)} out of range.");
            }

            return this.buffer[this.Cursor];
        }

        /// <summary>
        /// Reads a byte value from the message and advances the cursor.
        /// </summary>
        /// <returns>The value read.</returns>
        public byte GetByte()
        {
            if (this.Cursor + 1 > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.GetByte)} out of range.");
            }

            return this.buffer[this.Cursor++];
        }

        /// <summary>
        /// Creates a copy of this message.
        /// </summary>
        /// <param name="overrideCursor">Optional. A value to override the cursor with.</param>
        /// <returns>The copy of the message.</returns>
        public INetworkMessage Copy(int? overrideCursor = null)
        {
            if (overrideCursor.HasValue && (overrideCursor.Value < 0 || overrideCursor.Value > this.buffer.Length))
            {
                throw new ArgumentException($"The {nameof(overrideCursor)} must point to a position within the message's buffer.");
            }

            NetworkMessage newMessage = new NetworkMessage
            {
                Length = this.Length,
                Cursor = overrideCursor ?? this.Cursor,
            };

            this.Buffer.CopyTo(newMessage.buffer);

            return newMessage;
        }

        /// <summary>
        /// Reads a string value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        public string GetString()
        {
            int len = this.GetUInt16();
            string t = Encoding.Default.GetString(this.buffer, this.Cursor, len);

            this.Cursor += len;
            return t;
        }

        /// <summary>
        /// Reads an unsigned short value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(this.GetBytes(sizeof(ushort)));
        }

        /// <summary>
        /// Reads an unsigned integer value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(this.GetBytes(sizeof(uint)));
        }

        /// <summary>
        /// Reads some bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        public ReadOnlySpan<byte> GetBytes(int count)
        {
            if (this.Cursor + count > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.GetBytes)} out of range.");
            }

            var spanToReturn = this.buffer.AsSpan().Slice(this.Cursor, count);

            this.Cursor += count;

            return spanToReturn;
        }

        /// <summary>
        /// Adds a byte to the message.
        /// </summary>
        /// <param name="value">The byte value to add.</param>
        public void AddByte(byte value)
        {
            if (1 + this.Length > BufferSize)
            {
                throw new InvalidOperationException("Message buffer is full.");
            }

            this.buffer[this.Cursor++] = value;

            if (this.Cursor > this.Length)
            {
                this.Length = this.Cursor;
            }
        }

        /// <summary>
        /// Adds a byte range to the message.
        /// </summary>
        /// <param name="value">The bytes to add.</param>
        public void AddBytes(ReadOnlySpan<byte> value)
        {
            if (value.Length + this.Length > BufferSize)
            {
                throw new InvalidOperationException("Message buffer is full.");
            }

            // Before: Array.Copy(value, 0, this.buffer, this.Cursor, value.Length);
            value.CopyTo(this.buffer.AsSpan(this.Cursor));

            this.Cursor += value.Length;

            if (this.Cursor > this.Length)
            {
                this.Length = this.Cursor;
            }
        }

        /// <summary>
        /// Adds a byte sequence to the message.
        /// </summary>
        /// <param name="value">The bytes to add.</param>
        public void AddBytes(ReadOnlySequence<byte> value)
        {
            if (value.Length + this.Length > BufferSize)
            {
                throw new InvalidOperationException("Message buffer is full.");
            }

            value.CopyTo(this.buffer.AsSpan(this.Cursor));

            this.Cursor += (int)value.Length;

            if (this.Cursor > this.Length)
            {
                this.Length = this.Cursor;
            }
        }

        /// <summary>
        /// Add a string to the message.
        /// </summary>
        /// <param name="value">The string value to add.</param>
        public void AddString(string value)
        {
            this.AddUInt16((ushort)value.Length);
            this.AddBytes(Encoding.Default.GetBytes(value));
        }

        /// <summary>
        /// Add an unsigned short integer to the message.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void AddUInt16(ushort value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Add an unsigned integer to the message.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void AddUInt32(uint value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Changes the <see cref="Length"/> of this message and resets it's <see cref="Cursor"/>.
        /// </summary>
        /// <param name="size">The new size of the message.</param>
        /// <param name="resetCursor">Optional. A value indicating whether to reset the cursor in the message to zero.</param>
        public void Resize(int size, bool resetCursor = true)
        {
            this.Length = size;

            if (resetCursor)
            {
                this.Cursor = 0;
            }
        }

        /// <summary>
        /// Moves the read pointer in the message, essentially skipping a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        public void SkipBytes(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException($"The {nameof(count)} must not be negative.");
            }

            if (this.Cursor + count > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.SkipBytes)} out of range.");
            }

            this.Cursor += count;
        }

        /// <summary>
        /// Reads information sent in the message as it's bytes representation.
        /// </summary>
        /// <returns>The default formatted information.</returns>
        public IBytesInfo ReadAsBytesInfo()
        {
            return new DefaultRequestData(this.GetBytes(this.Length - this.Cursor));
        }

        /// <summary>
        /// Clears the message buffer and resets the <see cref="Cursor"/> to the given index.
        /// </summary>
        /// <param name="startingIndex">The index at which to reset the <see cref="Cursor"/> of this message.</param>
        private void Reset(int startingIndex)
        {
            this.buffer = new byte[BufferSize];
            this.Length = startingIndex;
            this.Cursor = startingIndex;
        }
    }
}
