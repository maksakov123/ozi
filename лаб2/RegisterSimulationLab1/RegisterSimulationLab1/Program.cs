using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Management;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace RegisterSimulationLab1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool check = Check();
            if (check) { }
            else 
            {
                Console.WriteLine("It is not your program");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            Task task = SavetoJson();
            task.Wait();
            Task task2 = ReadFromJson();
            task2.Wait();
            System.Menu();

            Console.ReadKey();
        }
        public static bool Check()
        {
            String willBeEncryptedData = "";
            ManagementObjectSearcher searcher5 =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_OperatingSystem");
            // Console.WriteLine(Environment.CurrentDirectory);
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                willBeEncryptedData += queryObj["SystemDirectory"].ToString();
                willBeEncryptedData += queryObj["WindowsDirectory"].ToString();
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
                    s.Add(newS);
                }
            }
            foreach (var item in s)
            {

                willBeEncryptedData += item.Capacity.ToString();
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
            }
            //-------------------------------------------------------------Encryption
            Console.WriteLine(willBeEncryptedData);
            var key = "a30a1d7ad2852a92dcd7bc054fc3577b";
            var encryptedString = AesOperation.EncryptString(key, willBeEncryptedData);
            Console.WriteLine($"encrypted string = {encryptedString}");
            string fromReestr = ReadReestrInfo();
            if (encryptedString.SequenceEqual(fromReestr))
            { return true; }
            else return false;

        }
        public static string ReadReestrInfo() //Write value
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey myKey = currentUserKey.OpenSubKey("Maksakov", true);

            string data = myKey.GetValue("MaksakovArtem").ToString();
            myKey.Close();
            return data;

        }
        public static async Task SavetoJson()
        {
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                User u = new User { ID = 1, Login = "ADMIN", Password = "", Type ="Admin" };
                List<User> l = new List<User>();
                l.Add(u);
                await JsonSerializer.SerializeAsync<List<User>>(fs,l) ;
                Console.WriteLine("Data has been saved to file");
            }
        }

        public static async Task ReadFromJson()
        {
            // чтение данных
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"{item.ID} , {item.Login}");
                }
            }
        }
    }

    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int BanStatus { get; set; }
        public string Type { get; set; }
        public int PasswordRequirement { get; set; }
    }

    public class System 
    {
        public static void Menu()
        {
            Console.WriteLine("Choose what You want to do");
            Console.WriteLine("Enter 1 to registrate");
            Console.WriteLine("Enter 2 to authentificate");
            Console.WriteLine("Enter 3 to Exit Program");
            string selection = Console.ReadLine();
            switch (selection)
            {
                case "1":
                    Task task = Registrate();
                    task.Wait();
                    break;
                case "2":
                    Task task2 = Authentificate();

                    task2.Wait();
                    break;
                case "3":
                    Environment.Exit(-1);
                    break;
                default:
                    Console.WriteLine("miss click");
                    Console.ReadKey();
                    Console.Clear();
                    Environment.Exit(-1);
                    break;
            }
        }

        public static async Task Registrate()
        {
            Console.Clear();
            Console.WriteLine("Enter your login");
            string log = Console.ReadLine();
            Console.WriteLine("Enter your password");
            string pass = Console.ReadLine();
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                User foundUser = restoredUsers.Find(i => i.Login == log);
                if (foundUser == null && !log.Equals(""))
                {
                    User addUser = new User { Login = log, Password = pass, BanStatus = 0, Type = "User" };
                    restoredUsers.Add(addUser);
                    fs.SetLength(0);
                    await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                    Console.WriteLine("You are registrated");
                }
                else if (log.Equals(""))
                {
                    Console.WriteLine("Login can*t be null");
                    Console.ReadKey();
                }
                else { Console.WriteLine("Another user has such login try again"); Console.ReadKey(); }
                    // Show menu regiatrste authentificate Exit
                    // User addUser = new User { Login = log, Password = pass, BanStatus = 0, Type = "User" };
                    // await JsonSerializer.SerializeAsync<User>(fs, addUser);
                }
                Console.Clear();
                Menu();

         }
        public static async Task Authentificate()
        {
            Console.Clear();
            Console.WriteLine("Enter your login");
                string log = Console.ReadLine();
                Console.WriteLine("Enter your password");
                string pass = Console.ReadLine();
                // 
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                    List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                    User foundUser = restoredUsers.Find(i => i.Login == log && i.Password == pass);
                    if (foundUser == null)
                    {
                        Console.WriteLine("Smth wrong try again");         
                    }
                    else
                    {//to do authenteficate user function for future steps
                        if (foundUser.Type == "Admin")
                            {
                                fs.Close();
                                AdminCanDo(foundUser);

                            }
                        else if (foundUser.Type == "User")
                            {
                                //Logic for user
                                if (foundUser.BanStatus == 1)
                                { Console.Clear(); Console.WriteLine("You are banned contact our administrators"); Console.ReadKey(); Environment.Exit(-1); }
                                else {
                                        fs.Close();
                                        UserCanDo(foundUser);
                                     }
                            }
                    } 
                }    
        }

        public static void AdminCanDo(User user)
        {
            Console.WriteLine("Choose what You want to do");
            Console.WriteLine("Enter 1 to change password");
            Console.WriteLine("Enter 2 block users");
            Console.WriteLine("Enter 3 add name");
            Console.WriteLine("Enter 4 to block user by name");
            Console.WriteLine("Enter 5 to on off password limits");
            Console.WriteLine("Enter 6 to go to menu");
            Console.WriteLine("Enter 7 to check users list");

            string selection = Console.ReadLine();
            switch (selection)
            {
                case "1":
                    Task task = ChangePassword(user);
                    task.Wait();
                    break;
                case "2":
                    Task task2 = BlockAllUsers(user);
                    task2.Wait();
                    break;
                case "3":
                    Environment.Exit(-1);
                    break;
                case "4":
                    Task task3 = BlockUserByName(user);
                    task3.Wait();
                    break;
                case "5":
                    Task task4 = ChangePasswordRequirement(user);
                    task4.Wait();
                    break;
                case "6":
                    Console.Clear();
                    Menu();
                    break;
                case "7":
                    Task task5 = ShowUsersList(user);
                    task5.Wait();
                    break;
                default:
                    Console.WriteLine("miss click");
                    Console.ReadKey();
                    Console.Clear();
                    Environment.Exit(-1);
                    break;
            }
        }
        public static async Task ShowUsersList(User user)
        {
            Console.Clear();
            Console.WriteLine("Users List");
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                User newUser = restoredUsers.Find(i => i.Login ==user.Login );
                foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"Login: {item.Login},Type of user: {item.Type},Is banned: {item.BanStatus},has password requirements: {item.PasswordRequirement}");
                }
                Console.ReadLine();
                Console.Clear();
                fs.Close();
                AdminCanDo(newUser);
            }
            
        }
        public static async Task ChangePassword(User user)
        {
            bool isRight = true; string currPass;
            int counter = 0;
            while (isRight)
            {
                if (counter < 3)
                {
                    Console.WriteLine("Enter current Password");
                    currPass = Console.ReadLine();
                    if (currPass.SequenceEqual(user.Password.ToString()))
                    {
                        isRight = false;
                    }
                    else { counter++; Console.WriteLine("Try again"); }
                }
                else { Console.WriteLine("YOU SHALL NOT PASS"); Console.ReadKey(); Environment.Exit(-1); }
            }
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Enter new Password");
                string newpass = Console.ReadLine();
                Console.WriteLine("Enter new Password 1 more time");
                string newpass2 = Console.ReadLine();
                if (newpass.Equals(newpass2))
                {
                    Console.WriteLine("Its OK");
                    flag = false;
                    using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                    {
                        List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                        fs.SetLength(0);
                        restoredUsers.Where(x => x.Login == user.Login).ToList().ForEach(b => b.Password = newpass);
                        User newUser = restoredUsers.Find(i => i.Login == user.Login);
                        await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                        Console.WriteLine("Password Changed");
                        Console.ReadLine();
                        Console.Clear();
                        fs.Close();
                        AdminCanDo(newUser);
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Enter 1 try again");
                    Console.WriteLine("Enter 2 go to main menu");
                    string selection = Console.ReadLine();
                    switch (selection)
                    {
                        case "1":
                            flag = true;
                            break;
                        case "2":
                            Console.Clear();
                            AdminCanDo(user);
                            break;
                        default:
                            Console.WriteLine("miss click");
                            Console.ReadKey();
                            Console.Clear();
                            Environment.Exit(-1);
                            break;
                    }
                }
            }
        }
        public static async Task BlockAllUsers(User user)
        {
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                fs.SetLength(0);
                foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"Login: {item.Login},Type of user: {item.Type},Is banned: {item.BanStatus},has password requirements: {item.PasswordRequirement}");
                }
                restoredUsers.Where(x => x.Type == "User").ToList().ForEach(b => b.BanStatus = 1);
                User newUser = restoredUsers.Find(i => i.Login == user.Login);
                await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                Console.WriteLine("All users Blocked");
                Console.ReadLine();
                Console.Clear();
                fs.Close();
                AdminCanDo(newUser);
            }
        }
        public static async Task BlockUserByName(User user)
        {
            Console.WriteLine("Enter User Name");
            string userName = Console.ReadLine();
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                fs.SetLength(0);
                foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"Login: {item.Login},Type of user: {item.Type},Is banned: {item.BanStatus},has password requirements: {item.PasswordRequirement}");
                }
                restoredUsers.Where(x => x.Login == userName).ToList().ForEach(b => b.BanStatus = 1);
                User newUser = restoredUsers.Find(i => i.Login == user.Login);
                await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                Console.WriteLine("User was blocked");
                Console.ReadLine();
                Console.Clear();
                fs.Close();
                AdminCanDo(newUser);
            }
        }
        public static async Task UnblockUserByName(User user)
        {
            Console.WriteLine("Enter User Name");
            string userName = Console.ReadLine();
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                fs.SetLength(0);
                foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"Login: {item.Login},Type of user: {item.Type},Is banned: {item.BanStatus},has password requirements: {item.PasswordRequirement}");
                }
                restoredUsers.Where(x => x.Login == userName).ToList().ForEach(b => b.BanStatus = 0);
                User newUser = restoredUsers.Find(i => i.Login == user.Login);
                await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                Console.WriteLine("User was blocked");
                Console.ReadLine();
                Console.Clear();
                fs.Close();
                AdminCanDo(newUser);
            }
        }

        public static async Task ChangePasswordRequirement(User user)
        {
            Console.WriteLine("Enter User Name");
            string userName = Console.ReadLine();
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                fs.SetLength(0); foreach (User item in restoredUsers)
                {
                    Console.WriteLine($"Login: {item.Login},Type of user: {item.Type},Is banned: {item.BanStatus},has password requirements: {item.PasswordRequirement}");
                }
                restoredUsers.Where(x => x.Login == userName).ToList().ForEach(b => b.PasswordRequirement = 1);
                User newUser = restoredUsers.Find(i => i.Login == user.Login);
                await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                Console.ReadLine();
                Console.Clear();
                fs.Close();
                AdminCanDo(newUser);
            }
        }
        public static void UserCanDo(User user)
        {
            Console.WriteLine("Choose what You want to do");
            Console.WriteLine("Enter 1 to change password");
            Console.WriteLine("Enter 2 go to main menu");
            string selection = Console.ReadLine();
            switch (selection)
            {
                case "1":
                    Task task = ChangePasswordUser(user);
                    task.Wait();
                    break;
                case "2":
                    Console.Clear();
                    Menu();
                    break;
                default:
                    Console.WriteLine("miss click");
                    Console.ReadKey();
                    Console.Clear();
                    Environment.Exit(-1);
                    break;
            }
        }
        public static async Task ChangePasswordUser(User user)
        {
            bool isRight = true; string currPass;
            int counter = 0;
            while (isRight)
            {
                if (counter < 3)
                {
                    Console.WriteLine("Enter current Password");
                    Console.WriteLine(user.Password);
                    currPass = Console.ReadLine();
                    if (currPass.SequenceEqual(user.Password.ToString()))
                    {
                        isRight = false;
                    }
                    else { counter++; Console.WriteLine("Try again"); }
                }
                else { Console.WriteLine("YOU SHALL NOT PASS"); Console.ReadKey(); Environment.Exit(-1); }
            }
            Console.Clear();

            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Enter new Password");
                string newpass = Console.ReadLine();
                Console.WriteLine("Enter new Password 1 more time");
                string newpass2 = Console.ReadLine();
                if (newpass.Equals(newpass2))
                {
                    Console.WriteLine("Its OK");
                    int n = Regex.Matches(newpass, @"[a-z]|[A-Z]|['?', '!','-'.',']").Count; int l = newpass.Length;
                    if (user.PasswordRequirement == 1) // check for pass reuire
                    {
                        if (n == l)
                        {
                            flag = false;
                            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                            {
                                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                                fs.SetLength(0);
                                restoredUsers.Where(x => x.Login == user.Login).ToList().ForEach(b => b.Password = newpass);
                                User newUser = restoredUsers.Find(i => i.Login == user.Login);

                                await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                                Console.WriteLine("Password Changed");
                                Console.ReadLine();
                                Console.Clear();
                                fs.Close();
                                UserCanDo(newUser);
                            }
                        }
                        else { Console.WriteLine("You have used smth bad use only a-z and A-Z letters and punstuation"); Console.ReadKey(); }
                    }
                    else {// if pass require == 0
                        flag = false;
                        using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                        {
                            List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                            fs.SetLength(0);
                            restoredUsers.Where(x => x.Login == user.Login).ToList().ForEach(b => b.Password = newpass);
                            User newUser = restoredUsers.Find(i => i.Login == user.Login);
                            await JsonSerializer.SerializeAsync<List<User>>(fs, restoredUsers);
                            Console.WriteLine("Password Changed");
                            Console.ReadLine();
                            Console.Clear();
                            fs.Close();
                            UserCanDo(newUser);
                        }
                    }
                    
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Enter 1 try again");
                    Console.WriteLine("Enter 2 go to main menu");
                    string selection = Console.ReadLine();
                    switch (selection)
                    {
                        case "1":
                            flag = true;
                            break;
                        case "2":
                            Console.Clear();
                            UserCanDo(user);
                            break;
                        default:
                            Console.WriteLine("miss click");
                            Console.ReadKey();
                            Console.Clear();
                            Environment.Exit(-1);
                            break;
                    }
                }
            }
        }
        

    }
    public class SaveInfo
    {
        public object Letter { get; set; }
        public object Capacity { get; set; }
        public object FreeSpace { get; set; }
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
