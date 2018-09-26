﻿using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace WoMFramework.Tool
{
    public static class Base58Encoding
    {
        public const int CheckSumSizeInBytes = 4;

        public static byte[] AddCheckSum(byte[] data)
        {
            //Contract.Requires<ArgumentNullException>(data != null);
            //Contract.Ensures(Contract.Result<byte[]>().Length == data.Length + CheckSumSizeInBytes);
            var checkSum = GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }

        //Returns null if the checksum is invalid
        public static byte[] VerifyAndRemoveCheckSum(byte[] data)
        {
            //Contract.Requires<ArgumentNullException>(data != null);
            //Contract.Ensures(Contract.Result<byte[]>() == null || Contract.Result<byte[]>().Length + CheckSumSizeInBytes == data.Length);
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
            var correctCheckSum = GetCheckSum(result);
            if (givenCheckSum.SequenceEqual(correctCheckSum))
                return result;
            return null;
        }

        public const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] data)
        {
            //Contract.Requires<ArgumentNullException>(data != null);
            //Contract.Ensures(Contract.Result<string>() != null);

            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            foreach (var t in data)
            {
                intData = intData * 256 + t;
            }

            // Encode BigInteger to Base58 string
            var result = "";
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        public static string EncodeWithCheckSum(byte[] data)
        {
            //Contract.Requires<ArgumentNullException>(data != null);
            //Contract.Ensures(Contract.Result<string>() != null);
            return Encode(AddCheckSum(data));
        }

        public static byte[] Decode(string s)
        {
            //Contract.Requires<ArgumentNullException>(s != null);
            //Contract.Ensures(Contract.Result<byte[]>() != null);

            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < s.Length; i++)
            {
                var digit = Digits.IndexOf(s[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", s[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = s.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        // Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        public static byte[] DecodeWithCheckSum(string s)
        {
            //Contract.Requires<ArgumentNullException>(s != null);
            //Contract.Ensures(Contract.Result<byte[]>() != null);
            var dataWithCheckSum = Decode(s);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
            if (dataWithoutCheckSum == null)
                throw new FormatException("Base58 checksum is invalid");
            return dataWithoutCheckSum;
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            //Contract.Requires<ArgumentNullException>(data != null);
            //Contract.Ensures(Contract.Result<byte[]>() != null);

            SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CheckSumSizeInBytes];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }
    }
}
