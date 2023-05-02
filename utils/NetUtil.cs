﻿using System.Security.Cryptography;
using System.Text;

namespace SquareSmash.Utils
{
    internal static class NetUtil
    {
        public static string MD5Hash(string s)
        {
            return BitConverter.ToString(MD5.HashData(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLower();
        }
    }
}