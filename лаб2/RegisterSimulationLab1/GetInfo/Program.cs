using System;
using System.Management;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace GetInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            String willBeEncryptedData = "";
            ManagementObjectSearcher searcher5 =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_OperatingSystem");

            Console.WriteLine(Environment.MachineName);
            Console.WriteLine(Environment.UserName);
           // Console.WriteLine(Environment.CurrentDirectory);
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                willBeEncryptedData += queryObj["SystemDirectory"].ToString() ;
                willBeEncryptedData += queryObj["WindowsDirectory"].ToString();
                Console.WriteLine("SystemDirectory: {0}", queryObj["SystemDirectory"]);          
                Console.WriteLine("WindowsDirectory: {0}", queryObj["WindowsDirectory"]);
            }

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Volume");

            List<SaveInfo> s = new List<SaveInfo>();
            foreach (ManagementObject queryObj in searcher.Get())
            {
                SaveInfo newS = new SaveInfo();
                if (queryObj["DriveLetter"] != null)
                {
                    newS.Letter = queryObj["DriveLetter"].ToString();
                    newS.Capacity = queryObj["Capacity"].ToString();
                    newS.FreeSpace = queryObj["FreeSpace"].ToString();
                    //Console.WriteLine("DriveLetter: {0}", queryObj["DriveLetter"]);
                    //Console.WriteLine("Capacity: {0}", queryObj["Capacity"]);
                    //Console.WriteLine("DriveType: {0}", queryObj["DriveType"]);
                    //Console.WriteLine("FreeSpace: {0}", queryObj["FreeSpace"]);
                    s.Add(newS);
                }
            }
            foreach (var item in s)
            {

                willBeEncryptedData += item.Capacity.ToString();
                Console.WriteLine($"letter = {item.Letter} cap = {item.Capacity}");
            }
           

            ManagementObjectSearcher searcher2 =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_VideoController");

            List<Scr> scr = new List<Scr>(); 
            foreach (ManagementObject queryObj in searcher2.Get())
            {
                if (queryObj["CurrentHorizontalResolution"] != null)
                {
                    Scr screen = new Scr();
                    screen.Width = queryObj["CurrentHorizontalResolution"].ToString();
                    screen.Height = queryObj["CurrentVerticalResolution"].ToString();
                    //Console.WriteLine("SystemDirectory: {0}", queryObj["CurrentHorizontalResolution"]);
                    //Console.WriteLine("WindowsDirectory: {0}", queryObj["CurrentVerticalResolution"]);
                    scr.Add(screen);
                }
            }
            foreach (var item in scr)
            {
                willBeEncryptedData += item.Width.ToString();
                Console.WriteLine($"Height = {item.Height}, Width = {item.Width} ");
            }

            ManagementObjectSearcher searcher6 =
       new ManagementObjectSearcher("root\\CIMV2",
           "SELECT * FROM Win32_Keyboard");

            foreach (ManagementObject queryObj in searcher6.Get())
            {
                
                    
                    Console.WriteLine("Name: {0}", queryObj["Name"]);
                    Console.WriteLine("Desc: {0}", queryObj["Description"]);
                  
                
            }

            //-------------------------------------------------------------Encryption
            Console.WriteLine(willBeEncryptedData);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            //Console.WriteLine("Please enter a secret key for the symmetric algorithm.");  
            //var key = Console.ReadLine();  
            var encryptedString = AesOperation.EncryptString(key, willBeEncryptedData);
            Console.WriteLine($"encrypted string = {encryptedString}");
            //var encryptedString1 = AesOperation.EncryptString(key, willBeEncryptedData);
            //Console.WriteLine($"encrypted string = {encryptedString1}");
            //var decryptedString = AesOperation.DecryptString(key, encryptedString);
            //Console.WriteLine($"decrypted string = {decryptedString}");


            //---------------------------------------------------------------Restr
            WriteValueToReestr(encryptedString);
            ReadReestrInfo();
            //DeleteKey();

            Console.ReadLine();
        }
        public static void WriteValueToReestr(string s) //Write value
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey myKey = currentUserKey.CreateSubKey("Maksakov");
            myKey.SetValue("MaskakovArtem", s);
            myKey.Close();
            Console.WriteLine("Value was Written");
        }
        public static void ReadReestrInfo() //Write value
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey myKey = currentUserKey.OpenSubKey("Maksakov", true);

            string data = myKey.GetValue("MaskakovArtem").ToString();
            myKey.Close();
            Console.WriteLine(data);
        }
        public static void DeleteKey()
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey helloKey = currentUserKey.OpenSubKey("Maksakov", true);
            // удаляем значение из ключа
            helloKey.DeleteValue("MaskakovArtem");
            // удаляем сам ключ
            currentUserKey.DeleteSubKey("Maksakov");
            Console.WriteLine("Value was deleted");
        }
    }
    
    public class SaveInfo
    {
        public object Letter { get; set; }
        public object Capacity { get; set; }
        public object  FreeSpace { get; set; }
    }
    class Scr
    {
        public string Height { get; set; }
        public string Width { get; set; }
    }
    public class AesOperation
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
