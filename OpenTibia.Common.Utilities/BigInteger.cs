// -----------------------------------------------------------------
// <copyright file="BigInteger.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------
// For BigInteger.cs:
//
// Copyright(c) 2002 Chew Keong TAN
// All rights reserved
//
// http://www.codeproject.com/KB/cs/biginteger.aspx
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, provided that the above copyright notice(s) and this
// permission notice appear in all copies of the Software and that both the
// above copyright notice(s) and this permission notice appear in supporting
// documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
// LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR
// ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER
// IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT
// OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace OpenTibia.Common.Utilities
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Class that represents a big integer.
    /// </summary>
    public class BigInteger
    {
        /// <summary>
        /// Maximum length of the BigInteger in uint (4 bytes). Change this to suit the required level of precision.
        /// </summary>
        private const int MaxLength = 70;

        /// <summary>
        /// Stores bytes from the Big Integer.
        /// </summary>
        private readonly uint[] data;

        /// <summary>
        /// Number of actual chars used.
        /// </summary>
        private int dataLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        public BigInteger()
        {
            this.data = new uint[MaxLength];
            this.dataLength = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public BigInteger(long value)
        {
            this.data = new uint[MaxLength];
            long tempVal = value;

            // copy bytes from long to BigInteger without any assumption of
            // the length of the long datatype
            this.dataLength = 0;
            while (value != 0 && this.dataLength < MaxLength)
            {
                this.data[this.dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                this.dataLength++;
            }

            // overflow check for +ve value
            if (tempVal > 0)
            {
                if (value != 0 || (this.data[MaxLength - 1] & 0x80000000) != 0)
                {
                    throw new ArithmeticException("Positive overflow in constructor.");
                }
            }
            else if (tempVal < 0)
            {
                // underflow check for -ve value
                if (value != -1 || (this.data[this.dataLength - 1] & 0x80000000) == 0)
                {
                    throw new ArithmeticException("Negative underflow in constructor.");
                }
            }

            if (this.dataLength == 0)
            {
                this.dataLength = 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public BigInteger(ulong value)
        {
            this.data = new uint[MaxLength];

            // copy bytes from ulong to BigInteger without any assumption of
            // the length of the ulong datatype
            this.dataLength = 0;
            while (value != 0 && this.dataLength < MaxLength)
            {
                this.data[this.dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                this.dataLength++;
            }

            if (value != 0 || (this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("Positive overflow in constructor.");
            }

            if (this.dataLength == 0)
            {
                this.dataLength = 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="bi">The value to initialize with.</param>
        public BigInteger(BigInteger bi)
        {
            this.data = new uint[MaxLength];

            this.dataLength = bi.dataLength;

            for (int i = 0; i < this.dataLength; i++)
            {
                this.data[i] = bi.data[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        /// <param name="radix">The base of the value to initialize with.</param>
        /// <remarks>
        /// Example (base 10)
        /// -----------------
        /// To initialize "a" with the default value of 1234 in base 10
        ///      BigInteger a = new BigInteger("1234", 10)
        ///
        /// To initialize "a" with the default value of -1234
        ///      BigInteger a = new BigInteger("-1234", 10)
        ///
        /// Example (base 16)
        /// -----------------
        /// To initialize "a" with the default value of 0x1D4F in base 16
        ///      BigInteger a = new BigInteger("1D4F", 16)
        ///
        /// To initialize "a" with the default value of -0x1D4F
        ///      BigInteger a = new BigInteger("-1D4F", 16)
        ///
        /// Note that string values are specified in the {sign}{magnitude} format.
        /// </remarks>
        public BigInteger(string value, int radix)
        {
            value.ThrowIfNullOrWhiteSpace(nameof(value));

            BigInteger multiplier = new BigInteger(1);
            BigInteger result = new BigInteger();

            value = value.ToUpper(CultureInfo.CurrentCulture).Trim();

            int limit = 0;

            if (value[0] == '-')
            {
                limit = 1;
            }

            for (int i = value.Length - 1; i >= limit; i--)
            {
                int posVal = value[i];

                if (posVal >= '0' && posVal <= '9')
                {
                    posVal -= '0';
                }
                else if (posVal >= 'A' && posVal <= 'Z')
                {
                    posVal = posVal - 'A' + 10;
                }
                else
                {
                    posVal = 9999999;       // arbitrary large
                }

                if (posVal >= radix)
                {
                    throw new ArithmeticException("Invalid string in constructor.");
                }

                if (value[0] == '-')
                {
                    posVal = -posVal;
                }

                result += multiplier * posVal;

                if ((i - 1) >= limit)
                {
                    multiplier *= radix;
                }
            }

            // negative values
            if (value[0] == '-')
            {
                if ((result.data[MaxLength - 1] & 0x80000000) == 0)
                {
                    throw new ArithmeticException("Negative underflow in constructor.");
                }
            }
            else
            {
                // positive values
                if ((result.data[MaxLength - 1] & 0x80000000) != 0)
                {
                    throw new ArithmeticException("Positive overflow in constructor.");
                }
            }

            this.data = new uint[MaxLength];
            for (int i = 0; i < result.dataLength; i++)
            {
                this.data[i] = result.data[i];
            }

            this.dataLength = result.dataLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="inData">The bytes of the value to initialize with.</param>
        /// <remarks>
        /// The lowest index of the input byte array (i.e [0]) should contain the
        /// most significant byte of the number, and the highest index should
        /// contain the least significant byte.
        ///
        /// E.g.
        /// To initialize "a" with the default value of 0x1D4F in base 16
        ///      byte[] temp = { 0x1D, 0x4F };
        ///      BigInteger a = new BigInteger(temp)
        ///
        /// Note that this method of initialization does not allow the
        /// sign to be specified.
        /// </remarks>
        public BigInteger(byte[] inData)
        {
            inData.ThrowIfNull(nameof(inData));

            this.dataLength = inData.Length >> 2;

            int leftOver = inData.Length & 0x3;

            // length not multiples of 4
            if (leftOver != 0)
            {
                this.dataLength++;
            }

            if (this.dataLength > MaxLength)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
            {
                this.data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
            {
                this.data[this.dataLength - 1] = inData[0];
            }
            else if (leftOver == 2)
            {
                this.data[this.dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            }
            else if (leftOver == 3)
            {
                this.data[this.dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
            }

            while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
            {
                this.dataLength--;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="inData">The bytes of the value to initialize with.</param>
        /// <param name="inLen">The lenght of the bytes value.</param>
        public BigInteger(byte[] inData, int inLen)
        {
            inData.ThrowIfNull(nameof(inData));

            this.dataLength = inLen >> 2;

            int leftOver = inLen & 0x3;

            // length not multiples of 4
            if (leftOver != 0)
            {
                this.dataLength++;
            }

            if (this.dataLength > MaxLength || inLen > inData.Length)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
            {
                this.data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
            {
                this.data[this.dataLength - 1] = inData[0];
            }
            else if (leftOver == 2)
            {
                this.data[this.dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            }
            else if (leftOver == 3)
            {
                this.data[this.dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
            }

            if (this.dataLength == 0)
            {
                this.dataLength = 1;
            }

            while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
            {
                this.dataLength--;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        /// <param name="inData">The uint collection of the value to initialize with.</param>
        public BigInteger(uint[] inData)
        {
            inData.ThrowIfNull(nameof(inData));

            this.dataLength = inData.Length;

            if (this.dataLength > MaxLength)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = this.dataLength - 1, j = 0; i >= 0; i--, j++)
            {
                this.data[j] = inData[i];
            }

            while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
            {
                this.dataLength--;
            }
        }

        /// <summary>
        /// Overloading of the typecast operator.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Overloading of the typecast operator.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Overloading of the typecast operator.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Overloading of the typecast operator.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger((ulong)value);
        }

        /// <summary>
        /// Overloading of the addition operator.
        /// </summary>
        /// <param name="bi1">The first integer to add.</param>
        /// <param name="bi2">The second integer to add.</param>
        /// <returns>The addition of both integers.</returns>
        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger
            {
                dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength,
            };

            long carry = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long sum = bi1.data[i] + (long)bi2.data[i] + carry;
                carry = sum >> 32;
                result.data[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result.dataLength < MaxLength)
            {
                result.data[result.dataLength] = (uint)carry;
                result.dataLength++;
            }

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException();
            }

            return result;
        }

        /// <summary>
        /// Overloading of the unary ++ operator.
        /// </summary>
        /// <param name="bi1">The integer to increment.</param>
        /// <returns>The big integer with it's value incremented.</returns>
        public static BigInteger operator ++(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if (index > result.dataLength)
            {
                result.dataLength = index;
            }
            else
            {
                while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                {
                    result.dataLength--;
                }
            }

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was +ve but ++ caused a sign
            // change to negative.
            if ((bi1.data[lastPos] & 0x80000000) == 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException("Overflow in ++.");
            }

            return result;
        }

        /// <summary>
        /// Overloading of the subtraction operator.
        /// </summary>
        /// <param name="bi1">The first integer to subtract from.</param>
        /// <param name="bi2">The second integer to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger result = new BigInteger
            {
                dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength,
            };

            long carryIn = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long diff;

                diff = bi1.data[i] - (long)bi2.data[i] - carryIn;
                result.data[i] = (uint)(diff & 0xFFFFFFFF);

                if (diff < 0)
                {
                    carryIn = 1;
                }
                else
                {
                    carryIn = 0;
                }
            }

            // roll over to negative
            if (carryIn != 0)
            {
                for (int i = result.dataLength; i < MaxLength; i++)
                {
                    result.data[i] = 0xFFFFFFFF;
                }

                result.dataLength = MaxLength;
            }

            // fixed in v1.03 to give correct datalength for a - (-b)
            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException();
            }

            return result;
        }

        /// <summary>
        /// Overloading of the unary -- operator.
        /// </summary>
        /// <param name="bi1">The integer to decrement.</param>
        /// <returns>The big integer with it's value decremented.</returns>
        public static BigInteger operator --(BigInteger bi1)
        {
            bi1.ThrowIfNull(nameof(bi1));

            BigInteger result = new BigInteger(bi1);

            long val;
            bool carryIn = true;
            int index = 0;

            while (carryIn && index < MaxLength)
            {
                val = result.data[index];
                val--;

                result.data[index] = (uint)(val & 0xFFFFFFFF);

                if (val >= 0)
                {
                    carryIn = false;
                }

                index++;
            }

            if (index > result.dataLength)
            {
                result.dataLength = index;
            }

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was -ve but -- caused a sign
            // change to positive.
            if ((bi1.data[lastPos] & 0x80000000) != 0 && (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException("Underflow in --.");
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of multiplication operator
        // ***********************************************************************
        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            int lastPos = MaxLength - 1;
            bool bi1Neg = false, bi2Neg = false;

            // take the absolute value of the inputs
            try
            {
                // bi1 negative
                if ((bi1.data[lastPos] & 0x80000000) != 0)
                {
                    bi1Neg = true;
                    bi1 = -bi1;
                }

                // bi2 negative
                if ((bi2.data[lastPos] & 0x80000000) != 0)
                {
                    bi2Neg = true;
                    bi2 = -bi2;
                }
            }
            catch (Exception)
            {
            }

            BigInteger result = new BigInteger();

            // multiply the absolute values
            try
            {
                for (int i = 0; i < bi1.dataLength; i++)
                {
                    if (bi1.data[i] == 0)
                    {
                        continue;
                    }

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
                    {
                        // k = i + j
                        ulong val = (bi1.data[i] * (ulong)bi2.data[j]) + result.data[k] + mcarry;

                        result.data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = val >> 32;
                    }

                    if (mcarry != 0)
                    {
                        result.data[i + bi2.dataLength] = (uint)mcarry;
                    }
                }
            }
            catch (Exception)
            {
                throw new ArithmeticException("Multiplication overflow.");
            }

            result.dataLength = bi1.dataLength + bi2.dataLength;
            if (result.dataLength > MaxLength)
            {
                result.dataLength = MaxLength;
            }

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            // overflow check (result is -ve)
            if ((result.data[lastPos] & 0x80000000) != 0)
            {
                // different sign
                if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)
                {
                    // handle the special case where multiplication produces
                    // a max negative number in 2's complement.
                    if (result.dataLength == 1)
                    {
                        return result;
                    }

                    bool isMaxNeg = true;
                    for (int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
                    {
                        if (result.data[i] != 0)
                        {
                            isMaxNeg = false;
                        }
                    }

                    if (isMaxNeg)
                    {
                        return result;
                    }
                }

                throw new ArithmeticException("Multiplication overflow.");
            }

            // if input has different signs, then result is -ve
            if (bi1Neg != bi2Neg)
            {
                return -result;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of unary << operators
        // ***********************************************************************
        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            bi1.ThrowIfNull(nameof(bi1));

            BigInteger result = new BigInteger(bi1);
            result.dataLength = ShiftLeft(result.data, shiftVal);

            return result;
        }

        // ***********************************************************************
        // Overloading of unary >> operators
        // ***********************************************************************
        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            bi1.ThrowIfNull(nameof(bi1));

            BigInteger result = new BigInteger(bi1);
            result.dataLength = ShiftRight(result.data, shiftVal);

            // negative
            if ((bi1.data[MaxLength - 1] & 0x80000000) != 0)
            {
                for (int i = MaxLength - 1; i >= result.dataLength; i--)
                {
                    result.data[i] = 0xFFFFFFFF;
                }

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data[result.dataLength - 1] & mask) != 0)
                    {
                        break;
                    }

                    result.data[result.dataLength - 1] |= mask;
                    mask >>= 1;
                }

                result.dataLength = MaxLength;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of the NOT operator (1's complement)
        // ***********************************************************************
        public static BigInteger operator ~(BigInteger bi1)
        {
            bi1.ThrowIfNull(nameof(bi1));

            BigInteger result = new BigInteger(bi1);

            for (int i = 0; i < MaxLength; i++)
            {
                result.data[i] = ~bi1.data[i];
            }

            result.dataLength = MaxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of the NEGATE operator (2's complement)
        // ***********************************************************************
        public static BigInteger operator -(BigInteger bi1)
        {
            bi1.ThrowIfNull(nameof(bi1));

            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.
            if (bi1.dataLength == 1 && bi1.data[0] == 0)
            {
                return new BigInteger();
            }

            BigInteger result = new BigInteger(bi1);

            // 1's complement
            for (int i = 0; i < MaxLength; i++)
            {
                result.data[i] = ~bi1.data[i];
            }

            // add one to result of 1's complement
            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1.data[MaxLength - 1] & 0x80000000) == (result.data[MaxLength - 1] & 0x80000000))
            {
                throw new ArithmeticException("Overflow in negation.\n");
            }

            result.dataLength = MaxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of equality operator
        // ***********************************************************************
        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            if (ReferenceEquals(null, bi1))
            {
                return ReferenceEquals(null, bi2);
            }

            return bi1.Equals(bi2);
        }

        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            return !bi1.Equals(bi2);
        }

        // ***********************************************************************
        // Overloading of inequality operator
        // ***********************************************************************
        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
            {
                return false;
            }

            // bi1 is positive, bi2 is negative
            if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
            {
                return true;
            }

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
            {
            }

            if (pos >= 0)
            {
                if (bi1.data[pos] > bi2.data[pos])
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
            {
                return true;
            }

            // bi1 is positive, bi2 is negative
            if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
            {
                return false;
            }

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
            {
            }

            if (pos >= 0)
            {
                if (bi1.data[pos] < bi2.data[pos])
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            return bi1 == bi2 || bi1 > bi2;
        }

        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            return bi1 == bi2 || bi1 < bi2;
        }

        // ***********************************************************************
        // Overloading of division operator
        // ***********************************************************************
        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();

            int lastPos = MaxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            // bi1 negative
            if ((bi1.data[lastPos] & 0x80000000) != 0)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            // bi2 negative
            if ((bi2.data[lastPos] & 0x80000000) != 0)
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            if (bi2.dataLength == 1)
            {
                SingleByteDivide(bi1, bi2, quotient, remainder);
            }
            else
            {
                MultiByteDivide(bi1, bi2, quotient, remainder);
            }

            if (dividendNeg != divisorNeg)
            {
                return -quotient;
            }

            return quotient;
        }

        // ***********************************************************************
        // Overloading of modulus operator
        // ***********************************************************************
        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger(bi1);

            int lastPos = MaxLength - 1;
            bool dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)
            {
                // bi1 negative
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2.data[lastPos] & 0x80000000) != 0)
            {
                // bi2 negative
                bi2 = -bi2;
            }

            if (bi1 < bi2)
            {
                return remainder;
            }

            if (bi2.dataLength == 1)
            {
                SingleByteDivide(bi1, bi2, quotient, remainder);
            }
            else
            {
                MultiByteDivide(bi1, bi2, quotient, remainder);
            }

            if (dividendNeg)
            {
                return -remainder;
            }

            return remainder;
        }

        // ***********************************************************************
        // Overloading of bitwise AND operator
        // ***********************************************************************
        public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger result = new BigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] & bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = MaxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of bitwise OR operator
        // ***********************************************************************
        public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger result = new BigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] | bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = MaxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of bitwise XOR operator
        // ***********************************************************************
        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            BigInteger result = new BigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] ^ bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = MaxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }

            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override bool Equals(object o)
        {
            BigInteger bi = o as BigInteger;

            if (bi == null || this.dataLength != bi.dataLength)
            {
                return false;
            }

            for (int i = 0; i < this.dataLength; i++)
            {
                if (this.data[i] != bi.data[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the maximum between this and another <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="bi">The other integer.</param>
        /// <returns>The maximum between this and another <see cref="BigInteger"/>.</returns>
        public BigInteger Max(BigInteger bi)
        {
            if (this > bi)
            {
                return new BigInteger(this);
            }

            return new BigInteger(bi);
        }

        /// <summary>
        /// Returns the minimum between this and another <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="bi">The other integer.</param>
        /// <returns>The minimum between this and another <see cref="BigInteger"/>.</returns>
        public BigInteger Min(BigInteger bi)
        {
            if (this < bi)
            {
                return new BigInteger(this);
            }

            return new BigInteger(bi);
        }

        /// <summary>
        /// Returns the absolute value of this integer.
        /// </summary>
        /// <returns>The absolute value of this integer.</returns>
        public BigInteger Abs()
        {
            if ((this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                return -this;
            }

            return new BigInteger(this);
        }

        /// <summary>
        /// Returns the string representation of this integer in base 10.
        /// </summary>
        /// <returns>The string representation of this integer in base 10.</returns>
        public override string ToString()
        {
            return this.ToString(10);
        }

        /// <summary>
        /// Returns a string representing the <see cref="BigInteger"/> in sign-and-magnitude format in the specified radix.
        /// </summary>
        /// <param name="radix">The base in which the interger is.</param>
        /// <returns>The string representing the <see cref="BigInteger"/> in sign-and-magnitude format in the specified radix.</returns>
        /// <remarks>
        /// If the value of BigInteger is -255 in base 10, then ToString(16) returns "-FF".
        /// </remarks>
        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
            {
                throw new ArgumentException("Radix must be >= 2 and <= 36");
            }

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = string.Empty;

            BigInteger a = this;

            bool negative = false;
            if ((a.data[MaxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception)
                {
                }
            }

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();
            BigInteger biRadix = new BigInteger(radix);

            if (a.dataLength == 1 && a.data[0] == 0)
            {
                result = "0";
            }
            else
            {
                while (a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
                {
                    SingleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder.data[0] < 10)
                    {
                        result = remainder.data[0] + result;
                    }
                    else
                    {
                        result = charSet[(int)remainder.data[0] - 10] + result;
                    }

                    a = quotient;
                }

                if (negative)
                {
                    result = "-" + result;
                }
            }

            return result;
        }

        // ***********************************************************************
        // Returns a hex string showing the contains of the BigInteger
        //
        // Examples
        // -------
        // 1) If the value of BigInteger is 255 in base 10, then
        //    ToHexString() returns "FF"
        //
        // 2) If the value of BigInteger is -255 in base 10, then
        //    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
        //    which is the 2's complement representation of -255.
        //
        // ***********************************************************************
        public string ToHexString()
        {
            string result = this.data[this.dataLength - 1].ToString("X");

            for (int i = this.dataLength - 2; i >= 0; i--)
            {
                result += this.data[i].ToString("X8");
            }

            return result;
        }

        // ***********************************************************************
        // Modulo Exponentiation
        // ***********************************************************************
        public BigInteger ModPow(BigInteger exp, BigInteger n)
        {
            if ((exp.data[MaxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("Positive exponents only.");
            }

            BigInteger resultNum = 1;
            BigInteger tempNum;
            bool thisNegative = false;

            // negative this
            if ((this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
            {
                tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)
            }

            // negative n
            if ((n.data[MaxLength - 1] & 0x80000000) != 0)
            {
                n = -n;
            }

            // calculate constant = b^(2k) / m
            BigInteger constant = new BigInteger();

            int i = n.dataLength << 1;
            constant.data[i] = 0x00000001;
            constant.dataLength = i + 1;

            constant /= n;
            int totalBits = exp.BitCount();
            int count = 0;

            // perform squaring and multiply exponentiation
            for (int pos = 0; pos < exp.dataLength; pos++)
            {
                uint mask = 0x01;

                for (int index = 0; index < 32; index++)
                {
                    if ((exp.data[pos] & mask) != 0)
                    {
                        resultNum = this.BarrettReduction(resultNum * tempNum, n, constant);
                    }

                    mask <<= 1;

                    tempNum = this.BarrettReduction(tempNum * tempNum, n, constant);

                    if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
                    {
                        // odd exp
                        if (thisNegative && (exp.data[0] & 0x1) != 0)
                        {
                            return -resultNum;
                        }

                        return resultNum;
                    }

                    count++;
                    if (count == totalBits)
                    {
                        break;
                    }
                }
            }

            // odd exp
            if (thisNegative && (exp.data[0] & 0x1) != 0)
            {
                return -resultNum;
            }

            return resultNum;
        }

        /// <summary>
        /// Returns the greatest common divisor between this and another <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="bi">The other <see cref="BigInteger"/>.</param>
        /// <returns>The greatest common divisor between this and another <see cref="BigInteger"/>.</returns>
        public BigInteger Gcd(BigInteger bi)
        {
            bi.ThrowIfNull(nameof(bi));

            BigInteger x;
            BigInteger y;

            // negative
            if ((this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                x = -this;
            }
            else
            {
                x = this;
            }

            // negative
            if ((bi.data[MaxLength - 1] & 0x80000000) != 0)
            {
                y = -bi;
            }
            else
            {
                y = bi;
            }

            BigInteger g = y;

            while (x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }

        /// <summary>
        /// Populates this integer with the specified amount of random bits.
        /// </summary>
        /// <param name="bits">The number of random bits to populate with.</param>
        /// <param name="rand">A <see cref="Random"/> instance to use.</param>
        public void GenRandomBits(int bits, Random rand)
        {
            rand.ThrowIfNull(nameof(rand));

            int dwords = bits >> 5;
            int remBits = bits & 0x1F;

            if (remBits != 0)
            {
                dwords++;
            }

            if (dwords > MaxLength)
            {
                throw new ArithmeticException("Number of required bits > maxLength.");
            }

            for (int i = 0; i < dwords; i++)
            {
                this.data[i] = (uint)(rand.NextDouble() * 0x100000000);
            }

            for (int i = dwords; i < MaxLength; i++)
            {
                this.data[i] = 0;
            }

            if (remBits != 0)
            {
                uint mask = (uint)(0x01 << (remBits - 1));
                this.data[dwords - 1] |= mask;

                mask = 0xFFFFFFFF >> (32 - remBits);
                this.data[dwords - 1] &= mask;
            }
            else
            {
                this.data[dwords - 1] |= 0x80000000;
            }

            this.dataLength = dwords;

            if (this.dataLength == 0)
            {
                this.dataLength = 1;
            }
        }

        // ***********************************************************************
        // Returns the position of the most significant bit in the BigInteger.
        //
        // Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
        //      The result is 1, if the value of BigInteger is 0...0000 0001
        //      The result is 2, if the value of BigInteger is 0...0000 0010
        //      The result is 2, if the value of BigInteger is 0...0000 0011
        //
        // ***********************************************************************
        public int BitCount()
        {
            while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
            {
                this.dataLength--;
            }

            uint value = this.data[this.dataLength - 1];
            uint mask = 0x80000000;
            int bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }

            bits += (this.dataLength - 1) << 5;

            return bits;
        }

        // ***********************************************************************
        // Returns the lowest 4 bytes of the BigInteger as an int.
        // ***********************************************************************
        public int IntValue()
        {
            return (int)this.data[0];
        }

        // ***********************************************************************
        // Returns the lowest 8 bytes of the BigInteger as a long.
        // ***********************************************************************
        public long LongValue()
        {
            long val = this.data[0];

            try
            {
                // exception if maxLength = 1
                val |= (long)this.data[1] << 32;
            }
            catch (Exception)
            {
                // negative
                if ((this.data[0] & 0x80000000) != 0)
                {
                    val = (int)this.data[0];
                }
            }

            return val;
        }

        // ***********************************************************************
        // Computes the Jacobi Symbol for a and b.
        // Algorithm adapted from [3] and [4] with some optimizations
        // ***********************************************************************
        public static int Jacobi(BigInteger a, BigInteger b)
        {
            // Jacobi defined only for odd integers
            if ((b.data[0] & 0x1) == 0)
            {
                throw new ArgumentException("Jacobi defined only for odd integers.");
            }

            if (a >= b)
            {
                a %= b;
            }

            if (a.dataLength == 1 && a.data[0] == 0)
            {
                return 0;  // a == 0
            }

            if (a.dataLength == 1 && a.data[0] == 1)
            {
                return 1;  // a == 1
            }

            if (a < 0)
            {
                if (((b - 1).data[0] & 0x2) == 0)
                {
                    return Jacobi(-a, b);
                }

                return -Jacobi(-a, b);
            }

            int e = 0;
            for (int index = 0; index < a.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((a.data[index] & mask) != 0)
                    {
                        index = a.dataLength;      // to break the outer loop
                        break;
                    }

                    mask <<= 1;
                    e++;
                }
            }

            BigInteger a1 = a >> e;

            int s = 1;
            if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
            {
                s = -1;
            }

            if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
            {
                s = -s;
            }

            if (a1.dataLength == 1 && a1.data[0] == 1)
            {
                return s;
            }

            return s * Jacobi(b % a1, a1);
        }

        // ***********************************************************************
        // Returns the modulo inverse of this.  Throws ArithmeticException if
        // the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
        // ***********************************************************************
        public BigInteger ModInverse(BigInteger modulus)
        {
            BigInteger[] p = { 0, 1 };
            BigInteger[] q = new BigInteger[2];    // quotients
            BigInteger[] r = { 0, 0 };             // remainders

            int step = 0;

            BigInteger a = modulus;
            BigInteger b = this;

            while (b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
            {
                BigInteger quotient = new BigInteger();
                BigInteger remainder = new BigInteger();

                if (step > 1)
                {
                    BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b.dataLength == 1)
                {
                    SingleByteDivide(a, b, quotient, remainder);
                }
                else
                {
                    MultiByteDivide(a, b, quotient, remainder);
                }

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient;
                r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }

            if (r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
            {
                throw new ArithmeticException("No inverse!");
            }

            BigInteger result = (p[0] - (p[1] * q[0])) % modulus;

            if ((result.data[MaxLength - 1] & 0x80000000) != 0)
            {
                result += modulus;  // get the least positive modulus
            }

            return result;
        }

        // ***********************************************************************
        // Returns the value of the BigInteger as a byte array.  The lowest
        // index contains the MSB.
        // ***********************************************************************
        public byte[] GetBytes()
        {
            int numBits = this.BitCount();

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
            {
                numBytes++;
            }

            byte[] result = new byte[numBytes];

            int pos = 0;
            uint tempVal, val = this.data[this.dataLength - 1];

            if ((tempVal = val >> 24 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val >> 16 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val >> 8 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            for (int i = this.dataLength - 2; i >= 0; i--, pos += 4)
            {
                val = this.data[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }

        // ***********************************************************************
        // Sets the value of the specified bit to 1
        // The Least Significant Bit position is 0.
        // ***********************************************************************
        public void SetBit(uint bitNum)
        {
            // divide by 32
            uint bytePos = bitNum >> 5;

            // get the lowest 5 bits
            byte bitPos = (byte)(bitNum & 0x1F);

            uint mask = 1U << bitPos;
            this.data[bytePos] |= mask;

            if (bytePos >= this.dataLength)
            {
                this.dataLength = (int)bytePos + 1;
            }
        }

        // ***********************************************************************
        // Sets the value of the specified bit to 0
        // The Least Significant Bit position is 0.
        // ***********************************************************************
        public void UnsetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;

            if (bytePos < this.dataLength)
            {
                byte bitPos = (byte)(bitNum & 0x1F);

                uint mask = 1U << bitPos;
                uint mask2 = 0xFFFFFFFF ^ mask;

                this.data[bytePos] &= mask2;

                if (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
                {
                    this.dataLength--;
                }
            }
        }

        // ***********************************************************************
        // Returns a value that is equivalent to the integer square root
        // of the BigInteger.
        //
        // The integer square root of "this" is defined as the largest integer n
        // such that (n * n) <= this
        //
        // ***********************************************************************
        public BigInteger Sqrt()
        {
            uint numBits = (uint)this.BitCount();

            // odd number of bits
            if ((numBits & 0x1) != 0)
            {
                numBits = (numBits >> 1) + 1;
            }
            else
            {
                numBits >>= 1;
            }

            uint bytePos = numBits >> 5;
            byte bitPos = (byte)(numBits & 0x1F);

            uint mask;

            BigInteger result = new BigInteger();
            if (bitPos == 0)
            {
                mask = 0x80000000;
            }
            else
            {
                mask = 1U << bitPos;
                bytePos++;
            }

            result.dataLength = (int)bytePos;

            for (int i = (int)bytePos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // guess
                    result.data[i] ^= mask;

                    // undo the guess if its square is larger than this
                    if ((result * result) > this)
                    {
                        result.data[i] ^= mask;
                    }

                    mask >>= 1;
                }

                mask = 0x80000000;
            }

            return result;
        }

        // ***********************************************************************
        // Returns the k_th number in the Lucas Sequence reduced modulo n.
        //
        // Uses index doubling to speed up the process.  For example, to calculate V(k),
        // we maintain two numbers in the sequence V(n) and V(n+1).
        //
        // To obtain V(2n), we use the identity
        //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
        // To obtain V(2n+1), we first write it as
        //      V(2n+1) = V((n+1) + n)
        // and use the identity
        //      V(m+n) = V(m) * V(n) - Q * V(m-n)
        // Hence,
        //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
        //                   = V(n+1) * V(n) - Q^n * V(1)
        //                   = V(n+1) * V(n) - Q^n * P
        //
        // We use k in its binary expansion and perform index doubling for each
        // bit position.  For each bit position that is set, we perform an
        // index doubling followed by an index addition.  This means that for V(n),
        // we need to update it to V(2n+1).  For V(n+1), we need to update it to
        // V((2n+1)+1) = V(2*(n+1))
        //
        // This function returns
        // [0] = U(k)
        // [1] = V(k)
        // [2] = Q^n
        //
        // Where U(0) = 0 % n, U(1) = 1 % n
        //       V(0) = 2 % n, V(1) = P % n
        // ***********************************************************************
        public static BigInteger[] LucasSequence(BigInteger p, BigInteger q, BigInteger k, BigInteger n)
        {
            if (k.dataLength == 1 && k.data[0] == 0)
            {
                BigInteger[] result = new BigInteger[3];

                result[0] = 0;
                result[1] = 2 % n;
                result[2] = 1 % n;
                return result;
            }

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            BigInteger constant = new BigInteger();

            int nLen = n.dataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.dataLength = nLen + 1;

            constant /= n;

            // calculate values of s and t
            int s = 0;

            for (int index = 0; index < k.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((k.data[index] & mask) != 0)
                    {
                        index = k.dataLength;      // to break the outer loop
                        break;
                    }

                    mask <<= 1;
                    s++;
                }
            }

            BigInteger t = k >> s;

            return LucasSequenceHelper(p, q, t, n, constant, s);
        }

        // ***********************************************************************
        // Performs the calculation of the kth term in the Lucas Sequence.
        // For details of the algorithm, see reference [9].
        //
        // k must be odd.  i.e LSB == 1
        // ***********************************************************************
        private static BigInteger[] LucasSequenceHelper(BigInteger p, BigInteger q, BigInteger k, BigInteger n, BigInteger constant, int s)
        {
            BigInteger[] result = new BigInteger[3];

            if ((k.data[0] & 0x00000001) == 0)
            {
                throw new ArgumentException("Argument k must be odd.");
            }

            int numbits = k.BitCount();
            uint mask = 0x1U << ((numbits & 0x1F) - 1);

            // v = v0, v1 = v1, u1 = u1, Q_k = Q^0
            BigInteger v = 2 % n, qK = 1 % n,
                       v1 = p % n, u1 = qK;
            bool flag = true;

            // iterate on the binary expansion of k
            for (int i = k.dataLength - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // last bit
                    if (i == 0 && mask == 0x00000001)
                    {
                        break;
                    }

                    // bit is set
                    if ((k.data[i] & mask) != 0)
                    {
                        // index doubling with addition
                        u1 = (u1 * v1) % n;

                        v = ((v * v1) - (p * qK)) % n;
                        v1 = n.BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((qK * q) << 1)) % n;

                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            qK = n.BarrettReduction(qK * qK, n, constant);
                        }

                        qK = (qK * q) % n;
                    }
                    else
                    {
                        // index doubling
                        u1 = ((u1 * v) - qK) % n;

                        v1 = ((v * v1) - (p * qK)) % n;
                        v = n.BarrettReduction(v * v, n, constant);
                        v = (v - (qK << 1)) % n;

                        if (flag)
                        {
                            qK = q % n;
                            flag = false;
                        }
                        else
                        {
                            qK = n.BarrettReduction(qK * qK, n, constant);
                        }
                    }

                    mask >>= 1;
                }

                mask = 0x80000000;
            }

            // at this point u1 = u(n+1) and v = v(n)
            // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)
            u1 = ((u1 * v) - qK) % n;
            v = ((v * v1) - (p * qK)) % n;
            if (flag)
            {
                flag = false;
            }
            else
            {
                qK = n.BarrettReduction(qK * qK, n, constant);
            }

            qK = (qK * q) % n;

            for (int i = 0; i < s; i++)
            {
                // index doubling
                u1 = (u1 * v) % n;
                v = ((v * v) - (qK << 1)) % n;

                if (flag)
                {
                    qK = q % n;
                    flag = false;
                }
                else
                {
                    qK = n.BarrettReduction(qK * qK, n, constant);
                }
            }

            result[0] = u1;
            result[1] = v;
            result[2] = qK;

            return result;
        }

        // least significant bits at lower part of buffer
        private static int ShiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                }

                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }

                count -= shiftAmount;
            }

            return bufLen;
        }

        private static int ShiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)buffer[i]) << invShift;
                    buffer[i] = (uint)val;
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            return bufLen;
        }

        // ***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has more than 1 digit.
        //
        // Algorithm taken from [1]
        // ***********************************************************************
        private static void MultiByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
        {
            bi1.ThrowIfNull(nameof(bi1));
            bi2.ThrowIfNull(nameof(bi2));

            outQuotient.ThrowIfNull(nameof(outQuotient));
            outRemainder.ThrowIfNull(nameof(outRemainder));

            uint[] result = new uint[MaxLength];

            int remainderLen = bi1.dataLength + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2.dataLength - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++;
                mask >>= 1;
            }

            for (int i = 0; i < bi1.dataLength; i++)
            {
                remainder[i] = bi1.data[i];
            }

            ShiftLeft(remainder, shift);
            bi2 <<= shift;

            int j = remainderLen - bi2.dataLength;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2.dataLength - 1];
            ulong secondDivisorByte = bi2.data[bi2.dataLength - 2];

            int divisorLen = bi2.dataLength + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];

                ulong qHat = dividend / firstDivisorByte;
                ulong rHat = dividend % firstDivisorByte;

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (qHat == 0x100000000 ||
                       (qHat * secondDivisorByte) > ((rHat << 32) + remainder[pos - 2]))
                    {
                        qHat--;
                        rHat += firstDivisorByte;

                        if (rHat < 0x100000000)
                        {
                            done = false;
                        }
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                {
                    dividendPart[h] = remainder[pos - h];
                }

                BigInteger kk = new BigInteger(dividendPart);
                BigInteger ss = bi2 * (long)qHat;

                while (ss > kk)
                {
                    qHat--;
                    ss -= bi2;
                }

                BigInteger yy = kk - ss;

                for (int h = 0; h < divisorLen; h++)
                {
                    remainder[pos - h] = yy.data[bi2.dataLength - h];
                }

                result[resultPos++] = (uint)qHat;

                pos--;
                j--;
            }

            outQuotient.dataLength = resultPos;
            int y = 0;
            for (int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
            {
                outQuotient.data[y] = result[x];
            }

            for (; y < MaxLength; y++)
            {
                outQuotient.data[y] = 0;
            }

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
            {
                outQuotient.dataLength--;
            }

            if (outQuotient.dataLength == 0)
            {
                outQuotient.dataLength = 1;
            }

            outRemainder.dataLength = ShiftRight(remainder, shift);

            for (y = 0; y < outRemainder.dataLength; y++)
            {
                outRemainder.data[y] = remainder[y];
            }

            for (; y < MaxLength; y++)
            {
                outRemainder.data[y] = 0;
            }
        }

        // ***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has only 1 digit.
        // ***********************************************************************
        private static void SingleByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
        {
            if (outQuotient == null)
            {
                throw new ArgumentNullException(nameof(outQuotient));
            }

            uint[] result = new uint[MaxLength];
            int resultPos = 0;

            // copy dividend to reminder
            for (int i = 0; i < MaxLength; i++)
            {
                outRemainder.data[i] = bi1.data[i];
            }

            outRemainder.dataLength = bi1.dataLength;

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
            {
                outRemainder.dataLength--;
            }

            ulong divisor = bi2.data[0];
            int pos = outRemainder.dataLength - 1;
            ulong dividend = outRemainder.data[pos];

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }

            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
            }

            outQuotient.dataLength = resultPos;
            int j = 0;
            for (int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
            {
                outQuotient.data[j] = result[i];
            }

            for (; j < MaxLength; j++)
            {
                outQuotient.data[j] = 0;
            }

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
            {
                outQuotient.dataLength--;
            }

            if (outQuotient.dataLength == 0)
            {
                outQuotient.dataLength = 1;
            }

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
            {
                outRemainder.dataLength--;
            }
        }

        // ***********************************************************************
        // Fast calculation of modular reduction using Barrett's reduction.
        // Requires x < b^(2k), where b is the base.  In this case, base is
        // 2^32 (uint).
        //
        // Reference [4]
        // ***********************************************************************
        private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
        {
            int k = n.dataLength,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            BigInteger q1 = new BigInteger();

            // q1 = x / b^(k-1)
            for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
            {
                q1.data[j] = x.data[i];
            }

            q1.dataLength = x.dataLength - kMinusOne;
            if (q1.dataLength <= 0)
            {
                q1.dataLength = 1;
            }

            BigInteger q2 = q1 * constant;
            BigInteger q3 = new BigInteger();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
            {
                q3.data[j] = q2.data[i];
            }

            q3.dataLength = q2.dataLength - kPlusOne;
            if (q3.dataLength <= 0)
            {
                q3.dataLength = 1;
            }

            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            BigInteger r1 = new BigInteger();
            int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
            for (int i = 0; i < lengthToCopy; i++)
            {
                r1.data[i] = x.data[i];
            }

            r1.dataLength = lengthToCopy;

            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n
            BigInteger r2 = new BigInteger();
            for (int i = 0; i < q3.dataLength; i++)
            {
                if (q3.data[i] == 0)
                {
                    continue;
                }

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
                {
                    // t = i + j
                    ulong val = (q3.data[i] * (ulong)n.data[j]) +
                                 r2.data[t] + mcarry;

                    r2.data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = val >> 32;
                }

                if (t < kPlusOne)
                {
                    r2.data[t] = (uint)mcarry;
                }
            }

            r2.dataLength = kPlusOne;
            while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
            {
                r2.dataLength--;
            }

            r1 -= r2;

            // negative
            if ((r1.data[MaxLength - 1] & 0x80000000) != 0)
            {
                BigInteger val = new BigInteger();
                val.data[kPlusOne] = 0x00000001;
                val.dataLength = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
            {
                r1 -= n;
            }

            return r1;
        }
    }
}
#pragma warning restore CA1303 // Do not pass literals as localized parameters