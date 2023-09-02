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
            var app = await AuthwareStatic.InitializeApplicationAsync("App-ID");

            var profile = await AuthwareStatic.LoginAsync("Username", "Password");
            Console.WriteLine(profile.Response.Email);
            Console.WriteLine(profile.Response.Username);
            Console.WriteLine(profile.Response.Expiration);

            var api = await AuthwareStatic.ExecuteApiAsync("API-ID",
                new Dictionary<string, object>
                {
                    {"param1", "value1"},
                    {"param2", "value2"},
                });

            Console.WriteLine(api.Success ? api.Response.DecodedResponse : "Failed lol get rekt");
        }
    }
}
