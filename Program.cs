using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.Console;

namespace App
{
    public class User
    {
        public Name Name { get; set; }
        public Dob Dob { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }

        public int Age => DateTime.Now.Year - Dob.Date.Year;
    }

    public class Name
    {
        public string First { get; set; }
        public string Last { get; set; }
    }

    public class Dob
    {
        public DateTime Date { get; set; }
    }

    public class Root
    {
        public List<User> Results { get; set; }
    }

    public class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var users = await FetchUsers();
            var maleUsers = FilteringMaleUsers(users);
            var femaleUsers = FilteringFemaleUsers(users);

            Console.WriteLine("Male Users Aged Between 45 and 60:");
            PrintUsers(maleUsers);
            Console.WriteLine("\nFemale Users Above Age 20:");
            PrintUsers(femaleUsers);
        }

        private static async Task<List<User>> FetchUsers()
        {
            var response = await client.GetStringAsync("https://randomuser.me/api/?results=100&seed=59a7d962d3167864");
            var root = JsonConvert.DeserializeObject<Root>(response);
            return root.Results;
        }

        private static List<User> FilteringMaleUsers(List<User> users)
        {
            return users.FindAll(user => user.Gender == "male" && user.Age >= 45 && user.Age <= 60);
        }

        private static List<User> FilteringFemaleUsers(List<User> users)
        {
            return users.FindAll(user => user.Gender == "female" && user.Age > 20);
        }

        private static void PrintUsers(List<User> users)
        {
            var table = new Table();

            table.AddColumn("First Name");
            table.AddColumn("Last Name");
            table.AddColumn("Date of Birth");
            table.AddColumn("Email");
            table.AddColumn("Age");

            foreach (var user in users)
            {
                table.AddRow(user.Name.First, user.Name.Last, user.Dob.Date.ToShortDateString(), user.Email, user.Age.ToString());
            }
            AnsiConsole.Render(table);
        }
    }
}
