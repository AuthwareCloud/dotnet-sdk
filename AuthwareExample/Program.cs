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
            var app = await AuthwareStatic.InitializeApplicationAsync("f8d10091-f11b-499f-997b-7c09f10b3038");

            var profile = await AuthwareStatic.LoginAsync("Khrysus", "OLMRR7G5XiOUvGgjvvKU");
            Console.WriteLine(profile.Response.Email);
            Console.WriteLine(profile.Response.Username);
            Console.WriteLine(profile.Response.Expiration);

            var api = await AuthwareStatic.ExecuteApiAsync("a9098b6f-2e4f-4a49-9926-3af10df8e6b8",
                new Dictionary<string, object>
                {
                    {"host", "51.222.26.41"},
                    {"port", "80"},
                    {"time", "10"},
                    {"method", "UDP"}
                });

            Console.WriteLine(api.Success ? api.Response.DecodedResponse : "Failed lol get rekt");
        }
    }
}