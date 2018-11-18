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
            while (pos < len)
            {
                var bytes = reader.ReadBytes(32);
                var c = new BigInteger(bytes);
                bytes = BigInteger.ModPow(c, _d, _n).ToByteArray();
                writer.Write(bytes);
                pos += 32;
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
                var bytes = reader.ReadBytes(8).Concat(zero).ToArray();
                var m = new BigInteger(bytes);
                bytes = BigInteger.ModPow(m, _e, _n).ToByteArray();
                if (bytes.LongLength < 32)
                    bytes = Extend(bytes);
                writer.Write(bytes);
                pos += 8;
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
