using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace YetAnotherOneRSA
{
    class Rsa
    {
        private readonly BigInteger _n, _e, _d;

        public Rsa(BigInteger n, BigInteger e, BigInteger d)
        {
            (_n, _e, _d) = (n, e, d);
        }

        public Rsa() { }

        public void Decrypt(BinaryReader reader, BinaryWriter writer)
        {
            var len = reader.BaseStream.Length;
            var pos = reader.BaseStream.Position;
            var zero = new byte[] { 0x00 };
            byte[] bytes;
            BigInteger c;
            while (pos < len - 32)
            {
                bytes = reader.ReadBytes(32);
                c = new BigInteger(bytes);
                bytes = BigInteger.ModPow(c, _d, _n).ToByteArray();
                writer.Write(bytes);
                pos += 32;
            }

            bytes = reader.ReadBytes(32).Concat(zero).ToArray();
            c = new BigInteger(bytes);
            bytes = BigInteger.ModPow(c, _d, _n).ToByteArray();
            if (bytes[7] != 0x08)
            {
                writer.Write(bytes, 0, 8 - bytes[7]);
            }
        }

        public void Encrypt(BinaryReader reader, BinaryWriter writer)
        {
            var len = reader.BaseStream.Length;
            var pos = reader.BaseStream.Position;
            var zero = new byte[] {0x00};
            writer.Write(Extend(_d.ToByteArray()));
            writer.Write(Extend(_n.ToByteArray()));
            while (pos < len)
            {
                var bytes = reader.ReadBytes(8);
                bytes = bytes.Length < 8 ? Extend(bytes, 8, (byte)Math.Abs(8 - bytes.Length)) : bytes.Concat(zero).ToArray();
                var m = new BigInteger(bytes);
                bytes = BigInteger.ModPow(m, _e, _n).ToByteArray();
                if (bytes.LongLength < 32)
                    bytes = Extend(bytes);
                writer.Write(bytes);
                pos += 8;
            }

            if (len % 8 == 0)
            {
                var m = new BigInteger(Enumerable.Repeat((byte)0x08, 8).Concat(zero).ToArray());
                writer.Write(Extend(BigInteger.ModPow(m, _e, _n).ToByteArray()));
            }
            writer.Close();
            reader.Close();
        }

        public byte[] Extend(byte[] bytes, int to = 32, byte with = 0x00)
        {
            return bytes.Concat(Enumerable.Repeat(with, to - bytes.Length)).ToArray();
        }
    }
}
