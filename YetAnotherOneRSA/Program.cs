using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace YetAnotherOneRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            var dh = new DiffieHellman();
            dh.AliceSetB(dh.BobPass(dh.AlicePass()));
            var bK = dh.BobCulcK();
            var aK = dh.AliceCulcK();

            var e = new Elgamal();
            string M = "asdasd";
            var bytes = Encoding.UTF8.GetBytes(M);
            var pack = e.GetMessagePack(bytes);

            var writer = new BinaryWriter(File.Open("tests/tt.txt", FileMode.Open));
            writer.Write(pack);
            //Test();
            //PaddingTest();
        }

        public static void Test()
        {
            var reader = new BinaryReader(File.Open("tests/encrypted.txt", FileMode.Open));
            var writer = new BinaryWriter(File.Open("tests/decrypted.txt", FileMode.Create));
            var d = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
            var n = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
            if (d != BigInteger.Parse("6126098115646990325497939754751346680273131840506753785808675623744493660811"))
                throw new Exception("Invalid d");
            if (n != BigInteger.Parse("11714199531846391552190233593088023207845451372578311171850728357335472002497"))
                throw new Exception("Invalid n");
            var e = BigInteger.Parse("8486391357485896322440256533131770617523773615720428244581337759609685018543");
            var rsa = new Rsa(n, e, d);
            rsa.Decrypt(reader, writer);
            reader = new BinaryReader(File.Open("tests/decrypted.txt", FileMode.Open));
            writer = new BinaryWriter(File.Open("tests/reencrypted.txt", FileMode.Create));
            rsa.Encrypt(reader, writer);
            reader = new BinaryReader(File.Open("tests/reencrypted.txt", FileMode.Open));
            writer = new BinaryWriter(File.Open("tests/redecrypted.txt", FileMode.Create));
            d = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
            n = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
            rsa.Decrypt(reader, writer);
        }

        public static void PaddingTest()
        {
            for (var i = 1; i <= 8; ++i)
            {
                var reader = new BinaryReader(File.Open($"tests/padding_{i}.in.txt", FileMode.Open));
                var writer = new BinaryWriter(File.Open($"tests/padding_{i}.out.txt", FileMode.Create));
                var (n, e, d) = KeyGenerator.GetRsaKeys();
                var rsa = new Rsa(n, e, d);
                rsa.Encrypt(reader, writer);
                reader = new BinaryReader(File.Open($"tests/padding_{i}.out.txt", FileMode.Open));
                writer = new BinaryWriter(File.Open($"tests/padding_{i}.out.dec.txt", FileMode.Create));
                d = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
                n = new BigInteger(reader.ReadBytes(32).Reverse().ToArray().Extend());
                rsa.Decrypt(reader, writer);
            }
        }
    }
}