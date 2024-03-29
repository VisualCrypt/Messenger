﻿using System;
using System.Diagnostics;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Cryptography.Api.Implementations
{
    public static class Base64Encoder
    {
        /// <summary>
        /// Converts a 8-bit unsigned integer array to an equivalent subset of a Unicode character array encoded with base-64 digits. 
        /// </summary>
        public static char[] EncodeDataToBase64CharArray(byte[] data)
        {
            Guard.NotNull(data);

            var estimatedOutputCharCount = CalculateBase64EncodedLengthInChars(data.Length);
            var base64 = new char[estimatedOutputCharCount];

            //  actualOutputCharCount: A 32-bit signed integer containing the number of chars () in outArray.
            var actualOutputCharCount = Convert.ToBase64CharArray(data, 0, data.Length, base64, 0);

            Debug.Assert(actualOutputCharCount == estimatedOutputCharCount);

            return base64;
        }

        /// <summary>
        /// Decodes a Base64 string from the editor to its binary form.
        /// </summary>
        public static byte[] DecodeBase64StringToBinary(string base64)
        {
            Guard.NotNull(base64);
            return Convert.FromBase64String(base64);
        }

        public static int CalculateBase64EncodedLengthInChars(int rawBinaryLength)
        {
            // Each 3 byte sequence in the source data becomes a 4 char (not byte) 
            // sequence in the character array.  
            int arrayLengthInChars = (int)((4.0d / 3.0d) * rawBinaryLength);

            // If array length is not divisible by 4, go up to the next 
            // multiple of 4. 
            if (arrayLengthInChars % 4 != 0)
            {
                arrayLengthInChars += 4 - arrayLengthInChars % 4;
            }
            return arrayLengthInChars;
        }

        public static int SafeEstimateBytesNeededForNBase64Chars(int nBase64Chars)
        {
            return nBase64Chars * 3 / 4;
        }
    }
}