using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace YetAnotherOneRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            Bob();
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

        public static void Alice()
        {
            var reader = new BinaryReader(File.Open($"tests/tt.txt", FileMode.Open));
            var writer = new BinaryWriter(File.Open($"tests/ttt.txt", FileMode.Create));
            var (n, e, d) = KeyGenerator.GetRsaKeys();
            var rsa = new Rsa(n, e, d);
            rsa.Encrypt(reader, writer);
            reader = new BinaryReader(File.Open($"tests/ttt.txt", FileMode.Open));
            var M = reader.ReadBytes((int)reader.BaseStream.Length);
            var dh = new DiffieHellman();
            var (g, p, A) = dh.AlicePass();
            Console.WriteLine(g + " " + p + " " + A);
            var B = BigInteger.Parse(Console.ReadLine());
            dh.AliceSetB(B);
            var K = dh.AliceCulcK();
            var elgamal = new Elgamal();
            Console.WriteLine(elgamal.g + " " + elgamal.p + " " + elgamal.y);
            var sha = new SHA256Managed();
            var m = sha.ComputeHash(M);
            var pack = elgamal.GetMessagePack(m);
            var res = pack.Xor(K.ToByteArray());
            writer = new BinaryWriter(File.Open($"tests/tttt.txt", FileMode.Create));
            writer.Write(res);
            writer.Close();
        }

        public static void Bob()
        {
            var dh = new DiffieHellman();

            var input = Console.ReadLine();
            var gpA = input.Split(' ');
            var g = BigInteger.Parse(gpA[0]);
            var p = BigInteger.Parse(gpA[1]);
            var A = BigInteger.Parse(gpA[2]);

            var B = dh.BobPass((g, p, A));
            var k = dh.BobCulcK();
            Console.Write(B.ToString());

            input = Console.ReadLine();
            var gpy = input.Split(' ');
            g = BigInteger.Parse(gpy[0]);
            p = BigInteger.Parse(gpy[1]);
            var y = BigInteger.Parse(gpy[2]);

            var elg = new Elgamal(g, p, y);

            var reader = new BinaryReader(File.Open($"tests/tttt.txt", FileMode.Open));
            var bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            var ans = bytes.Xor(k.ToByteArray());
            var r = ans.Skip(32 * 3).Take(32).ToArray();
            var s = ans.Skip(32 * 4).Take(32).ToArray();
            var m = ans.Skip(32 * 5).ToArray();
            var f = elg.Check(m, (new BigInteger(r), new BigInteger(s)));
            Console.Write(f);
        }


    }
}