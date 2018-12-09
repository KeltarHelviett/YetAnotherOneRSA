using System.Numerics;

namespace YetAnotherOneRSA
{
    public class DiffieHellman
    {
        private BigInteger a, b;

        public BigInteger g, p, A, B;

        public (BigInteger, BigInteger, BigInteger) AlicePass()
        {
            p = NumberTheoryUtils.RandomPrimeInRange(10, 10000000000000000);
            g = NumberTheoryUtils.GetGroupGenerator(p, true);
            a = NumberTheoryUtils.RandomIntegerInRange(10, 10000000000000000);
            A = BigInteger.ModPow(g, a, p);
            return (g, p, A);
        }

        public void AliceSetB(BigInteger B)
        {
            this.B = B;
        }

        public BigInteger BobPass((BigInteger, BigInteger, BigInteger) gpA)
        {
            this.g = gpA.Item1;
            this.p = gpA.Item2;
            this.A = gpA.Item3;
            b = NumberTheoryUtils.RandomIntegerInRange(10, 10000000000000000);
            B = BigInteger.ModPow(g, b, p);
            return B;
        }

        public BigInteger BobCulcK()
        {
            return BigInteger.ModPow(A, b, p);
        }

        public BigInteger AliceCulcK()
        {
            return BigInteger.ModPow(B, a, p);
        }
    }
}
