using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Encodings;
using System.Text;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;

namespace _3DESFile
{
    public class BlockEncryption
    {
        public BlockEncryption()
        {
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static void EncryptData(String inName, String outName, byte[] tdesKey, byte[] tdesIV)
        {
            //Create the file streams to handle the input and output files.
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            byte[] bin = new byte[100]; //This is intermediate storage for the encryption.
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = fin.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.ECB;
            tdes.Key = tdesKey;
            tdes.Padding = PaddingMode.PKCS7;

            CryptoStream encStream = new CryptoStream(fout, tdes.CreateEncryptor(), CryptoStreamMode.Write);
            //CryptoStream encStream = new CryptoStream(fout, tdes.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);

            Console.WriteLine("Encrypting...");

            //Read from the input file, then encrypt and write to the output file.
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
                Console.WriteLine("{0} bytes processed", rdlen);
            }

            encStream.Close();
        }


        private static byte[] GetIVBytes()
        {
            string IV = "0000018573BIUMQAMK220120";
            byte[] data = Convert.FromBase64String(IV);
            Console.WriteLine(data.Length);

            return data; //Convert.FromBase64String(IV);

            //byte[] key2 = Encoding.ASCII.GetBytes("0000018573BIUMQAMK220120"); //24characters
            //return key2;
        }

        private static void DecryptData(String inName, String outName, byte[] tdesKey, byte[] tdesIV)
        {
            //Create the file streams to handle the input and output files.
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            byte[] bin = new byte[80]; //This is intermediate storage for the encryption.
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = fin.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.ECB;
            tdes.Key = tdesKey;
            tdes.Padding = PaddingMode.PKCS7;
            //Console.WriteLine("is this valid key? " + tdes.ValidKeySize(128));
            CryptoStream encStream = new CryptoStream(fout, tdes.CreateDecryptor(), CryptoStreamMode.Write);
            //CryptoStream encStream = new CryptoStream(fout, tdes.CreateDecryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
            //CryptoStream encStream = new CryptoStream(fout, tdes.CreateDecryptor(tdes.Key, tdes.IV), CryptoStreamMode.Write);

            //tdes.CreateDecryptor()

            Console.WriteLine("Decrypting...");

            //Read from the input file, then decrypt and write to the output file.
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, bin.Length);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
                Console.WriteLine("{0} bytes processed", rdlen);
            }

            encStream.Close();
        }


        static void DecryptFile(string fileName)
        {
            var appDataDir = Environment.CurrentDirectory.ToString();

            var fileIn = Path.Combine(appDataDir, fileName + "_Encrypted.dat");

            Console.WriteLine(fileIn);
            Console.WriteLine(File.Exists(fileIn));

            var fileOut = Path.Combine(appDataDir, fileName + "_Decrypted.txt");

            //DecryptData(tempDir, tempDirOut, GetKeyBytes(), GetIVBytes());
            //DecryptData(fileIn, fileOut, GetKeyBytes(), GetKeyBytes());
            //DecryptData(fileIn, fileOut, GetKeyBytes());
        }

        private static byte[] GetKeyBytes()
        {
            string encodedKey = "0000018573BIUMQAMK220120";

            //byte[] keyArray;
            //SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            //keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes("0000018573BIUMQAMK220120"));

            //byte[] data = Convert.FromBase64String(encodedKey);
            //Console.WriteLine(data.Length);

            //byte[] data2 = Convert.FromBase64String("012345678901234567890123");
            //byte[] key = Encoding.ASCII.GetBytes(encodedKey); //24characters
            //byte[] key2 = Encoding.ASCII.GetBytes("012345678901234567890123"); //24characters

            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();

            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(encodedKey));
            //data = MysecurityKeyArray;
            MyMD5CryptoService.Clear();

            //return MysecurityKeyArray;
            return UTF8Encoding.UTF8.GetBytes(encodedKey);
        }

        static void EncryptFile(string fileName)
        {
            var appDataDir = Environment.CurrentDirectory.ToString();

            var fileIn = Path.Combine(appDataDir, fileName + ".txt");

            Console.WriteLine(fileIn);
            Console.WriteLine(File.Exists(fileIn));

            var fileOut = Path.Combine(appDataDir, fileName + "_Encrypted.dat");

            //DecryptData(tempDir, tempDirOut, GetKeyBytes(), GetIVBytes());
            //EncryptData(fileIn, fileOut, GetKeyBytes(), GetKeyBytes());
           // EncryptData(fileIn, fileOut, GetKeyBytes());
        }

        static void ReadFile()
        {
            //var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataDir = Environment.CurrentDirectory.ToString();
            Console.WriteLine(appDataDir);
            var tempDir = Path.Combine(appDataDir, "BIMB_13022020~encrypted by Bank Islam.dat");
            //Console.WriteLine(tempDir);
            //var s = Path.Combine(tempDir, "test.html");
            Console.WriteLine(tempDir);
            Console.WriteLine(File.Exists(tempDir));
            //Process.Start(s);
            var tempDirOut = Path.Combine(appDataDir, "FileOut.txt");

            //DecryptData(tempDir, tempDirOut, GetKeyBytes(), GetIVBytes());
            DecryptData(tempDir, tempDirOut, GetKeyBytes(), GetKeyBytes());

        }
    }
}
