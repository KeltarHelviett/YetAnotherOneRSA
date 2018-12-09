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
    }
}
