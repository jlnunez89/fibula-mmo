// -----------------------------------------------------------------
// <copyright file="NetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using System.Text;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Security.Encryption;

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
        private const int DefaultStartingIndex = 2;

        /// <summary>
        /// The buffer of the message.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// The length of the message.
        /// </summary>
        private int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        /// <param name="isOutbound">A value indicating whether this message is an outboud message.</param>
        public NetworkMessage(bool isOutbound = true)
            : this (isOutbound ? OutboundMessageStartingIndex : 0)
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
        public int Length => this.length;

        /// <summary>
        /// Gets the position that the message will read from next.
        /// </summary>
        public int Cursor { get; private set; }

        /// <summary>
        /// Gets the buffer of this message.
        /// </summary>
        public byte[] Buffer => this.buffer;

        /// <summary>
        /// Clears the message buffer and resets the <see cref="Cursor"/> to the given index.
        /// </summary>
        /// <param name="startingIndex">The index at which to reset the <see cref="Cursor"/> of this message.</param>
        public void Reset(int startingIndex)
        {
            this.buffer = new byte[NetworkMessage.BufferSize];
            this.length = startingIndex;
            this.Cursor = startingIndex;
        }

        /// <summary>
        /// Resets the message.
        /// </summary>
        public void Reset()
        {
            this.Reset(DefaultStartingIndex);
        }

        /// <summary>
        /// Reads a byte value from the message.
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
        /// Reads some bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        public byte[] GetBytes(int count)
        {
            if (this.Cursor + count > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.GetBytes)} out of range.");
            }

            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Cursor, t, 0, count);

            this.Cursor += count;
            return t;
        }

        /// <summary>
        /// Creates a copy of this message.
        /// </summary>
        /// <returns>The copy of the message.</returns>
        public INetworkMessage Copy()
        {
            NetworkMessage newMessage = new NetworkMessage
            {
                length = this.Length,
                Cursor = this.Cursor,
            };

            this.Buffer.CopyTo(newMessage.buffer, 0);

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
            return BitConverter.ToUInt16(this.GetBytes(sizeof(ushort)), 0);
        }

        /// <summary>
        /// Reads an unsigned integer value from the message.
        /// </summary>
        /// <returns>The value read.</returns>
        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(this.GetBytes(sizeof(uint)), 0);
        }

        public byte[] GetPacket()
        {
            byte[] t = new byte[this.Length - 2];
            Array.Copy(this.buffer, 2, t, 0, this.Length - 2);
            return t;
        }

        /// <summary>
        /// Adds a byte to the message.
        /// </summary>
        /// <param name="value">The byte value to add.</param>
        public void AddByte(byte value)
        {
            if (1 + this.Length > NetworkMessage.BufferSize)
            {
                throw new Exception("Message buffer is full.");
            }

            this.AddBytes(new[] { value });
        }

        /// <summary>
        /// Adds a byte range to the message.
        /// </summary>
        /// <param name="value">The bytes to add.</param>
        public void AddBytes(byte[] value)
        {
            if (value.Length + this.Length > NetworkMessage.BufferSize)
            {
                throw new Exception("Message buffer is full.");
            }

            Array.Copy(value, 0, this.buffer, this.Cursor, value.Length);

            this.Cursor += value.Length;

            if (this.Cursor > this.Length)
            {
                this.length = this.Cursor;
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
        /// Adds a number of bytes with value Zero to the message.
        /// </summary>
        /// <param name="count">The number of zero-bytes to add.</param>
        public void AddPaddingBytes(int count)
        {
            this.Cursor += count;

            if (this.Cursor > this.Length)
            {
                this.length = this.Cursor;
            }
        }

        /// <summary>
        /// Peeks the next byte from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public byte PeekByte()
        {
            return this.buffer[this.Cursor];
        }

        public void Resize(int size)
        {
            this.length = size;
            this.Cursor = 0;
        }

        /// <summary>
        /// Peeks the next bytes from the message.
        /// </summary>
        /// <param name="count">The number of bytes to peek.</param>
        /// <returns>The bytes peeked.</returns>
        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Cursor, t, 0, count);
            return t;
        }

        /// <summary>
        /// Peeks an unsigned short value from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public ushort PeekUInt16()
        {
            return BitConverter.ToUInt16(this.PeekBytes(sizeof(ushort)), 0);
        }

        /// <summary>
        /// Peeks an unsigned integer value from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public uint PeekUInt32()
        {
            return BitConverter.ToUInt32(this.PeekBytes(sizeof(uint)), 0);
        }

        /// <summary>
        /// Peeks a string from the message.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public string PeekString()
        {
            int len = this.PeekUInt16();
            return Encoding.ASCII.GetString(this.PeekBytes(len + 2), 2, len);
        }

        /// <summary>
        /// Replaces a range of bytes in the message.
        /// </summary>
        /// <param name="index">The index at which to begin replacing the byte range.</param>
        /// <param name="value">The byte range to replace in the message.</param>
        public void ReplaceBytes(int index, byte[] value)
        {
            if (this.Length - index >= value.Length)
            {
                Array.Copy(value, 0, this.buffer, index, value.Length);
            }
        }

        /// <summary>
        /// Moves the read pointer in the message, essentially skipping a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        public void SkipBytes(int count)
        {
            if (this.Cursor + count > this.Length)
            {
                throw new IndexOutOfRangeException($"{nameof(this.SkipBytes)} out of range.");
            }

            this.Cursor += count;
        }

        public void RsaDecrypt(bool useCipKeys = true)
        {
            Rsa.Decrypt(ref this.buffer, this.Cursor, this.length, useCipKeys);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.Decrypt(ref this.buffer, ref this.length, 2, key);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.Encrypt(ref this.buffer, ref this.length, 2, key);
        }

        public bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false)
        {
            if (!insertOnlyOneLength)
            {
                this.InsertPacketLength();
            }

            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToSend(uint[] xteaKey)
        {
            // Must be before Xtea, because the packet length is encrypted as well
            this.InsertPacketLength();

            if (!this.XteaEncrypt(xteaKey))
            {
                return false;
            }

            // Must be after Xtea, because takes the checksum of the encrypted packet
            // InsertAdler32();
            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToRead(uint[] xteaKey)
        {
            if (!this.XteaDecrypt(xteaKey))
            {
                return false;
            }

            this.Cursor = 4;
            return true;
        }

        private void InsertPacketLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 4)), 0, this.buffer, 2, 2);
        }

        private void InsertTotalLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 2)), 0, this.buffer, 0, 2);
        }

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(this.buffer, 0);
        }
    }
}
