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
            var app = await AuthwareStatic.InitializeApplicationAsync("4e0e4479-8755-4ecc-b7b0-87fe9e1d0b03");
            var profile = await AuthwareStatic.LoginAsync("Khrysus", "Y2FWU72SCwUJxye1wtmq");

            Console.WriteLine(profile.Response.Email);
            Console.WriteLine(profile.Response.Username);
            Console.WriteLine(profile.Response.PlanExpire);

            foreach (var (key, value) in (await AuthwareStatic.GrabApplicationVariablesAsync()).Response)
                Console.WriteLine($"Key: {key} | Value: {value}");

            var api = await AuthwareStatic.ExecuteApiAsync("5cea83c9-fb22-4f47-b0fc-ac264177cbc8",
                                                           new Dictionary<string,object>()); 
            Console.WriteLine(api.Response.DecodedResponse);
        }
    }
}