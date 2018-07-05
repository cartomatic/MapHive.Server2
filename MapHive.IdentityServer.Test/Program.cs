using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace MapHive.IdentityServer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(
                Task.Run(() => MainAsync(args))
            );
        }

        static async Task MainAsync(string[] args)
        {

            // request the token from the Auth server
            var tokenClient = new TokenClient("http://localhost:5000/connect/token", "maphive-apis-client", "maphive-apis-client-test-secret");
            var response = await tokenClient.RequestClientCredentialsAsync("maphive_apis");

            var test = await MapHive.Core.Auth.LetMeInAsync("queen@maphive.net", "test");


            Console.ReadLine();
        }
    }
}
