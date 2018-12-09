using System.Numerics;
using System.Security.Cryptography;

namespace YetAnotherOneRSA
{
    public static class KeyGenerator
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static (BigInteger, BigInteger, BigInteger) GetRsaKeys()
        {
            var p = RandomPrime();
            var q = RandomPrime();
            var n = p * q;
            var phi = (p - 1) * (q - 1);
            var e = RandomIntegerInRange(2, phi);
            while (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                e = RandomIntegerInRange(2, phi);
            }

            var d = e.ModInverse(phi, isCoPrime: true);
            return (n, e, d);
        }

        public static BigInteger RandomPrime(long nbytes = 16)
        {
            var bytes = new byte[nbytes];
            BigInteger res;
            do
            {
                _rng.GetBytes(bytes);
                res = new BigInteger(bytes);
            } while (!res.IsProbablePrime());
            return new BigInteger(bytes);
        }

        public static BigInteger RandomIntegerInRange(BigInteger lower, BigInteger upper)
        {
            var bytes = upper.ToByteArray();
            BigInteger res;
            do
            {
                _rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= 0x7F;
                res = new BigInteger(bytes);
            } while (res >= upper || res <= lower);

            return res;
        }
    }
}
