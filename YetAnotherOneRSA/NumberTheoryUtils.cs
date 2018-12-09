using System.Numerics;
using System.Security.Cryptography;

namespace YetAnotherOneRSA
{
    public static class NumberTheoryUtils
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static BigInteger RandomPrime(long nbytes = 16)
        {
            var bytes = new byte[nbytes];
            BigInteger res;
            do
            {
                Rng.GetBytes(bytes);
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
                Rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= 0x7F;
                res = new BigInteger(bytes);
            } while (res >= upper || res <= lower);

            return res;
        }
    }
}
