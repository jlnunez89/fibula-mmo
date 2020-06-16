// -----------------------------------------------------------------
// <copyright file="Rsa.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security.Encryption
{
    using System;
    using Fibula.Common.Utilities;

    /// <summary>
    /// Helper class for RSA decryption.
    /// </summary>
    public static class Rsa
    {
        private const int BufferLength = 128;

        private static readonly BigInteger OtServerP = new BigInteger("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113", 10);
        private static readonly BigInteger OtServerQ = new BigInteger("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101", 10);
        private static readonly BigInteger OtServerDp = new BigInteger("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937", 10);
        private static readonly BigInteger OtServerDq = new BigInteger("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973", 10);
        private static readonly BigInteger OtServerInverseQ = new BigInteger("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418", 10);

        private static readonly BigInteger CipsoftP = new BigInteger("12017580013707233233987537782574702577133548287527131234152948150506251412291888866940292054989907714155267326586216043845592229084368540020196135619327879", 10);
        private static readonly BigInteger CipsoftQ = new BigInteger("11898921368616868351880508246112101394478760265769325412746398405473436969889506919017477758618276066588858607419440134394668095105156501566867770737187273", 10);
        private static readonly BigInteger CipsoftDp = new BigInteger("8600096169230651977875330302008843109503996439950294564624155336050523997688169856104180804416233147594214529455018271455182195462973351342710205846262073", 10);
        private static readonly BigInteger CipsoftDq = new BigInteger("2519694687789567709651001624113763876170350107089837772221714711859870886188360422694425383128682046052156487995590127487208810654582326452919586222907441", 10);
        private static readonly BigInteger CipsoftInverseQ = new BigInteger("1174780978094632975480997011665925784916890194200219560434769607142991788322744284466767726580919222136359133434371639500067898638577680086982263461836866", 10);

        /// <summary>
        /// Attempts to decrypt the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="position">The position in the buffer at which to start the decryption.</param>
        /// <param name="length">The length of the message.</param>
        /// <param name="useCipKeys">A value indicating whether to use CipSoft RSA keys, or OTServ keys if false.</param>
        /// <returns>True if the decryption is deemed successful, false otherwise.</returns>
        public static bool Decrypt(Span<byte> buffer, int position, int length, bool useCipKeys)
        {
            if (useCipKeys)
            {
                return DecryptUsigCipSoftKeys(buffer, position, length);
            }

            return DecryptUsigOpenTibiaKeys(buffer, position, length);
        }

        /// <summary>
        /// Attempts to decrypt the given buffer using OTServ RSA keys.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="position">The position in the buffer at which to start the decryption.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>True if the decryption is deemed successful, false otherwise.</returns>
        public static bool DecryptUsigOpenTibiaKeys(Span<byte> buffer, int position, int length)
        {
            if (length - position != BufferLength)
            {
                return false;
            }

            var t = buffer.Slice(length - BufferLength, BufferLength);

            BigInteger input = new BigInteger(t.ToArray());
            BigInteger output;

            BigInteger m1 = input.ModPow(OtServerDp, OtServerP);
            BigInteger m2 = input.ModPow(OtServerDq, OtServerQ);
            BigInteger h;

            if (m2 > m1)
            {
                h = OtServerP - (((m2 - m1) * OtServerInverseQ) % OtServerP);
            }
            else
            {
                h = ((m1 - m2) * OtServerInverseQ) % OtServerP;
            }

            output = m2 + (OtServerQ * h);

            var bytesFromInteger = GetPaddedValue(output);

            for (int i = 0; i < BufferLength; i++)
            {
                buffer[position + i] = bytesFromInteger[i];
            }

            return true;
        }

        /// <summary>
        /// Attempts to decrypt the given buffer using CipSoft RSA keys.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="position">The position in the buffer at which to start the decryption.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>True if the decryption is deemed successful, false otherwise.</returns>
        public static bool DecryptUsigCipSoftKeys(Span<byte> buffer, int position, int length)
        {
            if (length - position != BufferLength)
            {
                return false;
            }

            var t = buffer.Slice(length - BufferLength, BufferLength);

            BigInteger input = new BigInteger(t.ToArray());
            BigInteger output;

            BigInteger m1 = input.ModPow(CipsoftDp, CipsoftP);
            BigInteger m2 = input.ModPow(CipsoftDq, CipsoftQ);
            BigInteger h;

            if (m2 > m1)
            {
                h = CipsoftP - (((m2 - m1) * CipsoftInverseQ) % CipsoftP);
            }
            else
            {
                h = ((m1 - m2) * CipsoftInverseQ) % CipsoftP;
            }

            output = m2 + (CipsoftQ * h);

            var bytesFromInteger = GetPaddedValue(output);

            for (int i = 0; i < BufferLength; i++)
            {
                buffer[position + i] = bytesFromInteger[i];
            }

            return true;
        }

        /// <summary>
        /// Padds a big integers value.
        /// </summary>
        /// <param name="value">The big integer value to pad.</param>
        /// <returns>The padded bytes.</returns>
        private static byte[] GetPaddedValue(BigInteger value)
        {
            byte[] result = value.GetBytes();

            if (result.Length >= BufferLength)
            {
                return result;
            }

            // left-pad 0x00 value on the result (same integer, correct length)
            byte[] padded = new byte[BufferLength];

            Buffer.BlockCopy(result, 0, padded, BufferLength - result.Length, result.Length);

            // Temporary result may contain decrypted (plaintext) data, clear it
            Array.Clear(result, 0, result.Length);

            return padded;
        }
    }
}
