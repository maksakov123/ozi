using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RegisterSimulationLab1
{
    class VerySimplePassGenerator
    {
        private static Random random = new Random();
        public static string GetHash(string input, string pass)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input + pass));

            return Convert.ToBase64String(hash);
        }
        public static string TrytoPredictThis(string username, int lengthMoreThanUsername)
        {
            if (lengthMoreThanUsername > username.Length)
            {

                string strToAdd = "";
                for (int i = 0; i < lengthMoreThanUsername - username.Length; i++)
                {
                    strToAdd += " ";
                }
                StringBuilder userpass = new StringBuilder(username+strToAdd);
                int itertions = random.Next(1, username.Length);
                int[] a = new int[itertions];
                for (int i = 0; i < itertions; i++)
                {
                    a[i] = random.Next(1,username.Length);
                }

                for (int i = 0; i < a.Length; i++)
                {
                    int rand = random.Next(1, 4);
                    if (rand == 1)
                    {
                        userpass[a[i]-1] = RandomLetter();
                    }
                    else if (rand == 2)
                    {
                        userpass[a[i]-1] = RandomSmallLetter();
                    }
                    else if (rand == 3)
                    {
                        userpass[a[i]-1] = RandomNumber();
                    }
                    else { userpass[a[i]-1] = RandomSymbol(); }
                }

                for (int i = username.Length-1; i < lengthMoreThanUsername; i++)
                {
                    int rand = random.Next(1, 4);
                    if (rand == 1)
                    {
                        userpass[i] = RandomLetter();
                    }
                    else if (rand == 2)
                    {
                        userpass[i] = RandomSmallLetter();
                    }
                    else if (rand == 3)
                    {
                        userpass[i] = RandomNumber();
                    }
                    else { userpass[i] = RandomSymbol(); }
                }

                return userpass.ToString();
            }
            else { Console.WriteLine("Choose Other Pass Length"); return null; }



        }
        public static char RandomLetter() // генерація addString
        {
           const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string s =  new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            char c = s[0];return c; 
        }
        public static char RandomSmallLetter() // генерація addString
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower(); ;
            string s = new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            char c = s[0]; return c;
        }
        public static char RandomNumber() // генерація addString
        {
            string chars = "0123456789" ;
            string s = new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            char c = s[0]; return c;
        }
        public static char RandomSymbol() // генерація addString
        {
            string chars = ".,/'!*&;:()_-@";
            string s = new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            char c = s[0]; return c;
        }

    }
}
