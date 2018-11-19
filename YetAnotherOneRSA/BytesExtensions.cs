using System;
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
    }
}
