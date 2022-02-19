using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authware;

namespace AuthwareExample
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var app = await AuthwareStatic.InitializeApplicationAsync("abc");

            var register = await AuthwareStatic.RegisterAsync("abc", "abc", "abc",
                "abc");
            Console.WriteLine(register.Message);
            var profile = await AuthwareStatic.LoginAsync("Toshi", "");
            Console.WriteLine(profile.Response.Email);
            Console.WriteLine(profile.Response.Username);
            Console.WriteLine(profile.Response.PlanExpire);

            foreach (var (key, value) in (await AuthwareStatic.GrabApplicationVariablesAsync()).Response)
                Console.WriteLine($"Key: {key} | Value: {value}");

            var api = await AuthwareStatic.ExecuteApiAsync("abc",
                new Dictionary<string, object>());
            Console.WriteLine(api.Response.DecodedResponse);
        }
    }
}