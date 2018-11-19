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
                bytes = reader.ReadBytes(32).Reverse().ToArray().Extend();
                c = new BigInteger(bytes);
                bytes = BigInteger.ModPow(c, _d, _n).ToByteArray();
                bytes = bytes.Length > 8 ? bytes.Take(8).ToArray() : bytes;
                bytes = bytes.Extend(times: 8 - bytes.Length).Reverse().ToArray();
                writer.Write(bytes);
                pos += 32;
            }

            bytes = reader.ReadBytes(32).Reverse().ToArray().Extend();
            c = new BigInteger(bytes);
            bytes = BigInteger.ModPow(c, _d, _n).ToByteArray();
            bytes = bytes.Length > 8 ? bytes.Take(8).ToArray() : bytes;
            bytes = bytes.Extend(times: 8 - bytes.Length).Reverse().ToArray();
            if (bytes[7] != 0x08)
            {
                writer.Write(bytes, 0, 8 - bytes[7]);
            }
            writer.Close();
            reader.Close();
        }

        public void Encrypt(BinaryReader reader, BinaryWriter writer)
        {
            var len = reader.BaseStream.Length;
            var pos = reader.BaseStream.Position;
            var zero = new byte[] {0x00};
            writer.Write(Extend(_d.ToByteArray()).Reverse().ToArray());
            writer.Write(Extend(_n.ToByteArray()).Reverse().ToArray());
            while (pos < len)
            {
                var bytes = reader.ReadBytes(8);
                if (bytes.Length < 8)
                {
                    bytes = Extend(bytes, 8, (byte) Math.Abs(8 - bytes.Length)).ToArray();
                }
                bytes = bytes.Reverse().Concat(zero).ToArray();
                var m = new BigInteger(bytes);
                bytes = BigInteger.ModPow(m, _e, _n).ToByteArray();
                if (bytes.LongLength < 32)
                    bytes = Extend(bytes);
                writer.Write(bytes.Reverse().ToArray());
                pos += 8;
            }

            if (len % 8 == 0)
            {
                var m = new BigInteger(Enumerable.Repeat((byte)0x08, 8).Concat(zero).ToArray());
                var bytes = BigInteger.ModPow(m, _e, _n).ToByteArray();
                if (bytes.LongLength < 32)
                    bytes = Extend(bytes);
                writer.Write(bytes.Reverse().ToArray());
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
