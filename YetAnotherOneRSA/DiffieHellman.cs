using System.Numerics;

namespace YetAnotherOneRSA
{
    public class DiffieHellman
    {
        private BigInteger a, b, K;

        public BigInteger g, p, A, B;

        public BigInteger GiveA()
        {
            p = NumberTheoryUtils.RandomPrime();
            g = NumberTheoryUtils.GetGroupGenerator(p);
            a = NumberTheoryUtils.RandomNumber();
            A = BigInteger.ModPow(g, a, p);
            return A;
        }

        public void TakeB(BigInteger B)
        {
            this.B = B;
            K = BigInteger.ModPow(B, a, p);
        }

        public void TakeA(BigInteger A, BigInteger g, BigInteger p)
        {
            this.g = g;
            this.p = p;
            this.A = A;
            b = NumberTheoryUtils.RandomNumber();
            B = BigInteger.ModPow(g, b, p);
            K = BigInteger.ModPow(A, b, p);
        }

        public BigInteger GiveB()
        {
            return B;
        }
    }
}
