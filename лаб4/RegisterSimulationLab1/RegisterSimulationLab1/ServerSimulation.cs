using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace RegisterSimulationLab1
{
    class ServerSimulation
    {
        public string hashToCompare { get; set; } //хеш з яким будемо порінвнювати
        
        private static string password;// приватна змынна в якій зберігається пароль(статична ініціалізується в конструкторі)
        public string addString { get; set; }// Випадково згенерована частина паролю

        private static Random random = new Random();
        public static string RandomString() // генерація addString
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetHashString(string pass,string add) // Cтворення хеша
        {
            string s = pass + add;  
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = string.Empty;
            
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }
        public ServerSimulation()
        {
            password = "NewPass";
            addString = RandomString();
            hashToCompare = GetHashString(password, addString);
        }

        public static void EncryptFile(string inputFile, string outputFile)
        {

            try
            {
                string password = @"12345678";
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {

            }
        }

    }
}
