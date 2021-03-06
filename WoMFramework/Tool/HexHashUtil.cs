﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WoMFramework.Tool
{
    public class HexHashUtil
    {
        public static int GetHexVal(char hex)
        {
            var val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(String hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static bool TryHexPosConversion(int position, int length, List<char[]> hexStrList, out double value)
        {
            value = 0;
            foreach (var hexStr in hexStrList)
            {
                if (hexStr.Length < position + length)
                {
                    return false;
                }
                for (var i = 0; i < length; i++)
                {
                    //Console.Write($"[{i}] x:{hexStr[position + i]}, 16^{length - 1 - i} * {HexUtil.GetHexVal(hexStr[position + i])} ");
                    value += Math.Pow(16, length - 1 - i) * GetHexVal(hexStr[position + i]);
                }
                //Console.WriteLine();
            }
            return true;
        }

        public static string HashSha256(string hexString)
        {
            var rawBytes = StringToByteArray(hexString);
            return ByteArrayToString(HashSha256(rawBytes));
        }

        public static byte[] HashSha256(byte[] rawBytes)
        {
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(rawBytes);
            }
        }
    }
}
