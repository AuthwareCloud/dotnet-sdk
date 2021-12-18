using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authware;

namespace AuthwareExample
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var app = await AuthwareStatic.InitializeApplicationAsync("APP_ID");
            var profile = await AuthwareStatic.LoginAsync("USERNAME", "PASSWORD");
            
            Console.WriteLine(profile.Response.Email);
            Console.WriteLine(profile.Response.Username);
            Console.WriteLine(profile.Response.PlanExpire);

            foreach (var (key, value) in (await AuthwareStatic.GrabApplicationVariablesAsync()).Response)
                Console.WriteLine($"Key: {key} | Value: {value}");

            var api = await AuthwareStatic.ExecuteApiAsync("API_ID",
                new Dictionary<string, object>
                {
                    { "PARAM_1", "VALUE_1" }
                }); 
            Console.WriteLine(api.Response.DecodedResponse);
        }
    }
}