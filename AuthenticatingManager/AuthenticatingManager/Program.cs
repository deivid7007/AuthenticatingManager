using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatingManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
            Console.WriteLine("1 --> Login\n" +
                              "2 --> Register");

            Console.WriteLine("Choose option !");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
            }

            }
            catch (Exception)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input !!!");
                Console.WriteLine();
            }
        }

        private static void Login()
        {
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = GetPassword('*');
            using (UserManagerEntities db = new UserManagerEntities())
            {
                var user = db.User.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                   string dbPassword =  user.Password;
                    if (dbPassword == PasswordHasher(password))
                    {
                        Console.WriteLine("Logged in !!!");
                    }
                    else
                    {
                        Console.WriteLine("Wrong username or password !!!");
                    }
                }

                else
                {
                    Console.WriteLine("Invalid account !!!");
                }

            }
        }

        private static void Register()
        {
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            string password = GetPassword('*');
            string hashed = PasswordHasher(password);
            string id = Guid.NewGuid().ToString();


            using (UserManagerEntities db = new UserManagerEntities())
            {
                User user = new User(id, username, hashed);
                db.User.Add(user);
                db.SaveChanges();
            }

            Console.WriteLine();
            Console.WriteLine("Registration completed !!!");
        }

        private static string GetPassword(char mask)
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    sb.Append(keyInfo.KeyChar);
                    Console.Write(mask);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);

                    if (Console.CursorLeft == 0)
                    {
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                    }
                    else Console.Write("\b \b");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }

        public static string PasswordHasher(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }


    }
}
