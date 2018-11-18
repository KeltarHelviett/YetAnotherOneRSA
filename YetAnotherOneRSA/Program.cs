using System;

namespace YetAnotherOneRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            var (n, phi, e, d) = KeyGenerator.GetRsaKeys();
        }
    }
}
