using System;
using System.IO;

namespace YetAnotherOneRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        public static void Test()
        {
            var (n, e, d) = KeyGenerator.GetRsaKeys();
            var rsa = new Rsa(n, e, d);
            var reader = new BinaryReader(File.Open("tests/1.test.in", FileMode.Open));
            var writer = new BinaryWriter(File.Open("tests/1.test.out", FileMode.OpenOrCreate));
            rsa.Encrypt(reader, writer);
        }
    }
}