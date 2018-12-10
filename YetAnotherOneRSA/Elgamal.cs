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

        public Elgamal(BigInteger p, BigInteger g, BigInteger x)
        {
            this.p = p;
            this.g = g;
            this.x = x;
            y = BigInteger.ModPow(g, x, p);
        }

        public (BigInteger, BigInteger) Sign(string M)
        {
            var m = GetDigest(M);
            var k = NumberTheoryUtils.RandomPrimeInRange(1, p - 1);
            var r = BigInteger.ModPow(g, k, p);
            var s = ((m - x * r) * k.ModInverse(p - 1)).EuclidianMod(p - 1);
            Check(M, (r, s));
            return (r, s);
        }

        public bool Check(string M, (BigInteger, BigInteger) sign)
        {
            var (r, s) = sign;
            if (0 >= r || r >= p || 0 >= s || s >= p - 1)
                return false;
            var m = GetDigest(M);
            return (BigInteger.ModPow(y, r, p) * BigInteger.ModPow(r, s, p)) % p == BigInteger.ModPow(g, m, p);
        }

        private BigInteger GetDigest(string M)
        {
            var sha = new SHA256Managed();
            var bytes = Encoding.UTF8.GetBytes(M);
            var hash = sha.ComputeHash(bytes);
            return new BigInteger(hash.Extend());
        }

        public BigInteger GetMessangePack(string M)
        {
            var m = GetDigest(M);
            var (r, s) = Sign(M);
            var pack = p;
            pack = pack * BigInteger.Pow(10, g.Length()) + g;
            pack = pack * BigInteger.Pow(10, y.Length()) + y;
            pack = pack * BigInteger.Pow(10, r.Length()) + r;
            pack = pack * BigInteger.Pow(10, s.Length()) + s;
            pack = pack * BigInteger.Pow(10, m.Length()) + m;
            return pack;
        }
    }
}
