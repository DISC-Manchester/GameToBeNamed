
using GameToBeNamed.src;
using System;

namespace GameToBeNamed
{
    static internal class Program
    {
        
        static void Main(string[] args)
        {
            GameSqlAccess access = new GameSqlAccess();
            // Method intentionally left empty.
            Console.WriteLine("Enter your username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();
            if(access.login(username, password))
            {
                Console.WriteLine("logged in");
            }
            else 
            {
                Console.WriteLine("login failed");
            }
        }
    }
}
