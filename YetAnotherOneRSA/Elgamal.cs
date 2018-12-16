using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace YetAnotherOneRSA
{
    public class Elgamal
    {
        private BigInteger p, g, x;

        public BigInteger y;

        public Elgamal()
        {
            (p, g, x, y) = KeyGenerator.GetElgamalKeys();
        }

        //public Elgamal(BigInteger p, BigInteger g, BigInteger x)
        //{
        //    this.p = p;
        //    this.g = g;
        //    this.x = x;
        //    y = BigInteger.ModPow(g, x, p);
        //}

        public Elgamal(BigInteger g, BigInteger p, BigInteger y)
        {
            this.p = p;
            this.g = g;
            this.y = y;
       }

        public (BigInteger, BigInteger) Sign(byte[] M)
        {
            var m = GetDigest(M);
            var k = NumberTheoryUtils.RandomPrimeInRange(1, p - 1);
            var r = BigInteger.ModPow(g, k, p);
            var s = ((m - x * r) * k.ModInverse(p - 1)).EuclidianMod(p - 1);
            Check(M, (r, s));
            return (r, s);
        }

        public bool Check(byte[] M, (BigInteger, BigInteger) sign)
        {
            var (r, s) = sign;
            if (0 >= r || r >= p || 0 >= s || s >= p - 1)
                return false;
            var m = GetDigest(M);
            return (BigInteger.ModPow(y, r, p) * BigInteger.ModPow(r, s, p)) % p == BigInteger.ModPow(g, m, p);
        }

        private BigInteger GetDigest(byte[] M)
        {
            var sha = new SHA256Managed();
            var hash = sha.ComputeHash(M);
            return new BigInteger(hash.Extend());
        }

        public byte[] GetMessagePack(byte[] M)
        {
            var m = GetDigest(M);
            var (r, s) = Sign(M);
            if (!Check(M, (r, s)))
                throw new Exception("SHIT");
            var pack = p.ToByteArray().ExtendTo().Concat(
                g.ToByteArray().ExtendTo().Concat(
                    y.ToByteArray().ExtendTo().Concat(
                            r.ToByteArray().ExtendTo().Concat(
                                s.ToByteArray().ExtendTo()
                        )
                    )
                )
            ).Concat(m.ToByteArray().Trim()).ToArray();
            return pack;
        }
    }
}
