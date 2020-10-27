using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace RegisterSimulationLab1
{
    
    class Program
    {
        static void Main(string[] args)
        {
            //ServerSimulation.EncryptFile("user.json", "encrypted.txt");
            ServerSimulation server = new ServerSimulation();
            //Console.WriteLine(server.hashToCompare);
            Console.WriteLine("Enter Password Please");
            string checkString = Console.ReadLine();

            bool check = Check(checkString,server);
            if (check) 
            {
                DecryptFile("encrypted.txt", "user.json");
            }
            else
            {
                Console.WriteLine("It is not your program");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            //Task task = SavetoJson();
            //task.Wait();
            //Task task2 = ReadFromJson();
            //task2.Wait();
            System.Menu();

            //string readText = File.ReadAllText(@"user.json");
            //File.WriteAllText(@"user.json", File.ReadAllText(@"user.json"));
            //Console.WriteLine(readText);
            Console.ReadKey();
        }

        public static bool Check(string password ,ServerSimulation s)
        {
            password += s.addString;
            string hash = GetHashString(password);
            if (hash.SequenceEqual(s.hashToCompare)) return true;
            else return false;
        }
        public static string GetHashString(string s)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            //создаем объект для получения средст шифрования  
            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();
            //вычисляем хеш-представление в байтах  
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = string.Empty;
            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);
            return hash;
        }

        public static void DecryptFile(string inputFile, string outputFile)
        {

            {
                string password = @"Key666"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }


        public static async Task SavetoJson()
        {
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                User u = new User { Login = "ADMIN", Password = "", Type ="Admin" };
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
                   // Console.WriteLine($"{item.ID} , {item.Login}");
                }
            }
        }
    }

    public class User
    {
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
            Console.WriteLine("Enter 4 to Show info");
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
                    DeleteFile();
                    Environment.Exit(-1);
                    break;
                case "4":
                    ShowInfo();
                    break;
                default:
                    Console.WriteLine("miss click");
                    Console.ReadKey();
                    Console.Clear();
                    Environment.Exit(-1);
                    break;
            }
        }
        public static void DeleteFile()
        {
            if (File.Exists(@"user.json"))
            {
                File.Delete(@"user.json");
            }
        }
        public static void ShowInfo()
        {
            Console.Clear();
            Console.WriteLine("Student : Maksakov Artem");
            Console.WriteLine("Group : IS -71");
            Console.WriteLine("Variant 9: Наявність рядкових і прописних букв, а також розділових знаків.");
            Console.WriteLine("Параметри адміністратора: логін: ADMIN   без пароля(натисніть enter)");
            Console.ReadLine();
            Console.Clear();
            Menu();
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
                // 
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                Console.WriteLine("Enter your login");
                string log = Console.ReadLine();
                Console.WriteLine("Enter your password");
                string pass = Console.ReadLine();
                List<User> restoredUsers = await JsonSerializer.DeserializeAsync<List<User>>(fs);
                    User foundUser = restoredUsers.Find(i => i.Login == log && i.Password == pass);
                bool isRight = true;;
                int counter = 0;
                while (isRight)
                {
                    if (foundUser == null)
                    {
                        counter++;
                        if (counter < 3)
                        {
                            Console.Clear();
                            Console.WriteLine("Enter your login");
                            log = Console.ReadLine();
                            Console.WriteLine("Enter your password");
                            pass = Console.ReadLine();
                            foundUser = restoredUsers.Find(i => i.Login == log && i.Password == pass);
                        }
                        else { Console.WriteLine("YOU SHALL NOT PASS"); Console.ReadKey(); Environment.Exit(-1); }
                        
                    }
                    else
                    {//to do authenteficate user function for future steps
                        isRight = false;
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
                            else
                            {
                                fs.Close();
                                UserCanDo(foundUser);
                            }
                        }
                    }

                }
               
                fs.Close();
                Console.ReadLine();
                Console.Clear();
                Menu();
                }    
        }

        public static void AdminCanDo(User user)
        {
            Console.WriteLine("Choose what You want to do");
            Console.WriteLine("Enter 1 to change password");
            Console.WriteLine("Enter 2 block users");
            Console.WriteLine("Enter 3 add name");
            Console.WriteLine("Enter 4 to block user by name");
            Console.WriteLine("Enter 5 to on  password limits");
            Console.WriteLine("Enter 6 to off  password limits");
            Console.WriteLine("Enter 7 to go to menu");
            Console.WriteLine("Enter 8 to check users list");

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
                    Task task4 = ChangePasswordRequirementOn(user);
                    task4.Wait();
                    break;
                case "6":
                    Task task6 = ChangePasswordRequirementOff(user);
                    task6.Wait();
                    break;
                case "7":
                    Console.Clear();
                    Menu();
                    break;
                case "8":
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

        public static async Task ChangePasswordRequirementOn(User user)
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
        public static async Task ChangePasswordRequirementOff(User user)
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
                restoredUsers.Where(x => x.Login == userName).ToList().ForEach(b => b.PasswordRequirement = 0);
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
                        else { Console.WriteLine("You have used smth bad use only a-z and A-Z letters and punctuation"); Console.ReadKey(); }
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
    
}
