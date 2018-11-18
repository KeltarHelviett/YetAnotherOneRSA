using System;
using System.IO;
using System.Numerics;

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
            Console.WriteLine($"n = {n}, e = {e}, d = {d}");
            var reader = new BinaryReader(File.Open("tests/1.test.in", FileMode.Open));
            var writer = new BinaryWriter(File.Open("tests/1.test.out", FileMode.Truncate));
            rsa.Encrypt(reader, writer);
            reader = new BinaryReader(File.Open("tests/1.test.out", FileMode.Open));
            writer = new BinaryWriter(File.Open("tests/1.test.out.in", FileMode.Truncate));
            d = new BigInteger(reader.ReadBytes(32));
            n = new BigInteger(reader.ReadBytes(32));
            Console.WriteLine($"n = {n}, d = {d}");
            rsa = new Rsa(n, BigInteger.Zero, d);
            rsa.Decrypt(reader, writer);
        }
    }
}