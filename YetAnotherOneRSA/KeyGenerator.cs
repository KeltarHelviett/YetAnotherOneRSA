using System.Numerics;
using System.Security.Cryptography;

namespace YetAnotherOneRSA
{
    public static class KeyGenerator
    {
        public static (BigInteger, BigInteger, BigInteger) GetRsaKeys()
        {
            var p = NumberTheoryUtils.RandomPrime();
            var q = NumberTheoryUtils.RandomPrime();
            var n = p * q;
            var phi = (p - 1) * (q - 1);
            var e = NumberTheoryUtils.RandomIntegerInRange(2, phi);
            while (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                e = NumberTheoryUtils.RandomIntegerInRange(2, phi);
            }

            var d = e.ModInverse(phi, isCoPrime: true);
            return (n, e, d);
        }

        public static (BigInteger, BigInteger, BigInteger, BigInteger) GetElgamalKeys()
        {
            var p = NumberTheoryUtils.RandomPrime(1);
            var g = NumberTheoryUtils.GetGroupGenerator(p, isPrime: true);
            var x = NumberTheoryUtils.RandomIntegerInRange(1, p);
            var y = BigInteger.ModPow(g, x, p);
            return (p, g, x, y);
        }
    }
}
