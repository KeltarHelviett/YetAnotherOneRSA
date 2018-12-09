using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace YetAnotherOneRSA
{
    public static class NumberTheoryUtils
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static BigInteger RandomNumber(long nbytes = 16)
        {
            var bytes = new byte[nbytes];
            Rng.GetBytes(bytes);
            return new BigInteger(bytes.Extend());
        }

        public static BigInteger RandomPrime(long nbytes = 16)
        {
            var bytes = new byte[nbytes];
            BigInteger res;
            do
            {
                Rng.GetBytes(bytes);
                res = new BigInteger(bytes.Extend());
            } while (!res.IsProbablePrime());
            return res;
        }

        public static BigInteger RandomPrimeInRange(BigInteger lower, BigInteger upper)
        {
            BigInteger res;
            do
            {
                res = RandomIntegerInRange(lower, upper);
            } while (!res.IsProbablePrime());
            return res;
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

        public static IEnumerable<BigInteger> Factorize(BigInteger number)
        {
            for (BigInteger i = 2; i * i < number; ++i)
                if (number % i == 0)
                {
                    yield return i;
                    while (number % i == 0)
                        number /= i;
                }
                
            if (number > 1) yield return number;
        }

        public static BigInteger GetGroupGenerator(BigInteger modulus, bool isPrime = false)
        {
            var phi = isPrime ? modulus - 1 : throw new NotImplementedException();
            var dividers = Factorize(phi);
            for (BigInteger res = 2; res <= modulus; ++res)
            {
                var isGenerator = true;
                foreach (var divider in dividers)
                {
                    isGenerator &= BigInteger.ModPow(res, phi / divider, modulus) != 1;
                    if (!isGenerator)
                        break;
                }

                if (isGenerator)
                    return res;
            }
            throw new Exception("Group generator not found.");
        }

    }
}
