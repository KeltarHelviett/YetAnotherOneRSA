﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOneRSA
{
    static class BytesExtensions
    {
        public static byte[] Extend(this byte[] bytes, byte with = 0x00, int times = 1)
        {
            return bytes.Concat(Enumerable.Repeat(with, times)).ToArray();
        }

        public static byte[] ExtendTo(this byte[] bytes, int to = 32, byte with = 0x00)
        {
            return bytes.Concat(Enumerable.Repeat(with, to - bytes.Length)).ToArray();
        }

        public static byte[] Trim(this byte[] bytes)
        {
            var zeros = 0;
            for (var i = bytes.Length - 1; i >= 0; --i)
            {
                if (bytes[i] != 0x00)
                    break;
                zeros++;
            }

            return bytes.Take(bytes.Length - zeros).ToArray();
        }

        public static byte[] Xor(this byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                throw new Exception("Bad xor");
            var c = new byte[a.Length];
            for (var i = 0; i < a.Length; ++i)
                c[i] = (byte) (a[i] ^ b[i]);

            return c;
        }
    }
}
